using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace GodLesZ.Library.Controls {
	/// <summary>
	/// A FastObjectListView trades function for speed.
	/// </summary>
	/// <remarks>
	/// <para>On my mid-range laptop, this view builds a list of 10,000 objects in 0.1 seconds,
	/// as opposed to a normal ObjectListView which takes 10-15 seconds. Lists of up to 50,000 items should be
	/// able to be handled with sub-second response times even on low end machines.</para>
	/// <para>
	/// A FastObjectListView is implemented as a virtual list with some of the virtual modes limits (e.g. no sorting)
	/// fixed through coding. There are some functions that simply cannot be provided. Specifically, a FastObjectListView cannot:
	/// <list type="bullet">
	/// <item><description>use Tile view</description></item>
	/// <item><description>display images on subitems (though you can easily circumvent this limit by making the list owner drawn)</description></item>
	/// </list>
	/// </para>
	/// </remarks>
	public class FastObjectListView : VirtualObjectListView {
		/// <summary>
		/// Make a FastObjectListView
		/// </summary>
		public FastObjectListView() {
			this.DataSource = new FastObjectListDataSource(this);
			this.GroupingStrategy = new FastListGroupingStrategy();
		}

		/// <summary>
		/// Get/set the collection of objects that this list will show
		/// </summary>
		/// <remarks>
		/// <para>
		/// The contents of the control will be updated immediately after setting this property.
		/// </para>
		/// <para>This method preserves selection, if possible. Use SetObjects() if
		/// you do not want to preserve the selection. Preserving selection is the slowest part of this
		/// code and performance is O(n) where n is the number of selected rows.</para>
		/// <para>This method is not thread safe.</para>
		/// </remarks>
		[Browsable(false),
		 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override IEnumerable Objects {
			get {
				// This is much faster than the base method
				return ((FastObjectListDataSource)this.DataSource).ObjectList;
			}
			set { base.Objects = value; }
		}

		/// <summary>
		/// Remove any sorting and revert to the given order of the model objects
		/// </summary>
		public override void Unsort() {
			this.ShowGroups = false;
			this.PrimarySortColumn = null;
			this.PrimarySortOrder = SortOrder.None;
			this.SetObjects(this.Objects);
		}
	}

	/// <summary>
	/// Provide a data source for a FastObjectListView
	/// </summary>
	/// <remarks>
	/// This class isn't intended to be used directly, but it is left as a public
	/// class just in case someone wants to subclass it.
	/// </remarks>
	public class FastObjectListDataSource : AbstractVirtualListDataSource {
		/// <summary>
		/// Create a FastObjectListDataSource
		/// </summary>
		/// <param name="listView"></param>
		public FastObjectListDataSource(FastObjectListView listView)
			: base(listView) {
		}

		internal ArrayList ObjectList {
			get { return fullObjectList; }
		}

		internal ArrayList FilteredObjectList {
			get { return filteredObjectList; }
		}

		#region IVirtualListDataSource Members

		/// <summary>
		/// Get n'th object
		/// </summary>
		/// <param name="n"></param>
		/// <returns></returns>
		public override object GetNthObject(int n) {
			return this.filteredObjectList[n];
		}

		/// <summary>
		/// How many items are in the data source
		/// </summary>
		/// <returns></returns>
		public override int GetObjectCount() {
			return this.filteredObjectList.Count;
		}

		/// <summary>
		/// Get the index of the given model
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public override int GetObjectIndex(object model) {
			int index;

			if (model != null && this.objectsToIndexMap.TryGetValue(model, out index))
				return index;
			else
				return -1;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="first"></param>
		/// <param name="last"></param>
		/// <param name="column"></param>
		/// <returns></returns>
		public override int SearchText(string value, int first, int last, OLVColumn column) {
			return DefaultSearchText(value, first, last, column, this);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="column"></param>
		/// <param name="sortOrder"></param>
		public override void Sort(OLVColumn column, SortOrder sortOrder) {
			if (sortOrder != SortOrder.None)
				this.filteredObjectList.Sort(new ModelObjectComparer(column, sortOrder, this.listView.SecondarySortColumn, this.listView.SecondarySortOrder));
			this.RebuildIndexMap();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="modelObjects"></param>
		public override void AddObjects(ICollection modelObjects) {
			foreach (object modelObject in modelObjects) {
				if (modelObject != null)
					this.fullObjectList.Add(modelObject);
			}
			this.FilterObjects();
			this.RebuildIndexMap();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="modelObjects"></param>
		public override void RemoveObjects(ICollection modelObjects) {
			List<int> indicesToRemove = new List<int>();
			foreach (object modelObject in modelObjects) {
				int i = this.GetObjectIndex(modelObject);
				if (i >= 0)
					indicesToRemove.Add(i);
			}

			RemoveIndizies(indicesToRemove);
		}

		public override void RemoveIndizies(List<int> indicesToRemove) {
			// Sort the indices from highest to lowest so that we
			// remove latter ones before earlier ones. In this way, the
			// indices of the rows doesn't change after the deletes.
			indicesToRemove.Sort();
			indicesToRemove.Reverse();

			foreach (int i in indicesToRemove)
				this.listView.SelectedIndices.Remove(i);

			foreach (int i in indicesToRemove)
				this.fullObjectList.RemoveAt(i);

			this.FilterObjects();
			this.RebuildIndexMap();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="collection"></param>
		public override void SetObjects(IEnumerable collection) {
			ArrayList newObjects = EnumerableToArray(collection);

			this.fullObjectList = newObjects;
			this.FilterObjects();
			this.RebuildIndexMap();
		}

		private static ArrayList EnumerableToArray(IEnumerable collection) {
			ArrayList newObjects = new ArrayList();
			if (collection != null) {
				if (collection is ICollection)
					newObjects = new ArrayList((ICollection)collection);
				else {
					foreach (object x in collection)
						newObjects.Add(x);
				}
			}
			return newObjects;
		}

		protected ArrayList fullObjectList = new ArrayList();
		protected ArrayList filteredObjectList = new ArrayList();
		protected IModelFilter modelFilter;
		protected IListFilter listFilter;

		#endregion

		#region IFilterableDataSource Members

		/// <summary>
		/// 
		/// </summary>
		/// <param name="modelFilter"></param>
		/// <param name="listFilter"></param>
		public override void ApplyFilters(IModelFilter modelFilter, IListFilter listFilter) {
			this.modelFilter = modelFilter;
			this.listFilter = listFilter;
			this.SetObjects(this.fullObjectList);
		}

		#endregion


		#region Implementation

		/// <summary>
		/// Rebuild the map that remembers which model object is displayed at which line
		/// </summary>
		protected void RebuildIndexMap() {
			this.objectsToIndexMap.Clear();
			for (int i = 0; i < this.filteredObjectList.Count; i++)
				this.objectsToIndexMap[this.filteredObjectList[i]] = i;
		}
		Dictionary<Object, int> objectsToIndexMap = new Dictionary<Object, int>();

		/// <summary>
		/// Build our filtered list from our full list.
		/// </summary>
		public void FilterObjects() {
			if (!this.listView.UseFiltering || (this.modelFilter == null && this.listFilter == null)) {
				this.filteredObjectList = new ArrayList(this.fullObjectList);
				return;
			}

			IEnumerable objects = (this.listFilter == null) ?
				this.fullObjectList : this.listFilter.Filter(this.fullObjectList);

			// Apply the object filter if there is one
			if (this.modelFilter == null) {
				this.filteredObjectList = EnumerableToArray(objects);
			} else {
				this.filteredObjectList = new ArrayList();
				foreach (object model in objects) {
					if (this.modelFilter.Filter(model))
						this.filteredObjectList.Add(model);
				}
			}
		}

		#endregion
	}

}
