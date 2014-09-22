using System;
using System.Windows.Forms;

namespace GodLesZ.Library.Controls {
	/// <summary>
	/// This attribute is used to mark a field, property, or parameter-less method of a model
	/// class that should be noticed by Generator class.
	/// </summary>
	/// <remarks>
	/// All the attributes of this class match their equivilent properties on OLVColumn.
	/// </remarks>
	public class OLVColumnAttribute : Attribute {
		#region Constructor

		/// <summary>
		/// Create a new OLVColumnAttribute
		/// </summary>
		public OLVColumnAttribute() {
		}

		/// <summary>
		/// Create a new OLVColumnAttribute with the given title
		/// </summary>
		/// <param name="title">The title of the column</param>
		public OLVColumnAttribute(string title) {
			this.Title = title;
		}

		#endregion

		#region Public properties

		/// <summary>
		/// 
		/// </summary>
		public string AspectToStringFormat {
			get { return aspectToStringFormat; }
			set { aspectToStringFormat = value; }
		}
		private string aspectToStringFormat;

		/// <summary>
		/// 
		/// </summary>
		public bool CheckBoxes {
			get { return checkBoxes; }
			set { checkBoxes = value; }
		}
		private bool checkBoxes;

		/// <summary>
		/// 
		/// </summary>
		public int DisplayIndex {
			get { return displayIndex; }
			set { displayIndex = value; }
		}
		private int displayIndex;

		/// <summary>
		/// 
		/// </summary>
		public bool FillsFreeSpace {
			get { return fillsFreeSpace; }
			set { fillsFreeSpace = value; }
		}
		private bool fillsFreeSpace;

		/// <summary>
		/// 
		/// </summary>
		public int? FreeSpaceProportion {
			get { return freeSpaceProportion; }
			set { freeSpaceProportion = value; }
		}
		private int? freeSpaceProportion;

		/// <summary>
		/// An array of IComparables that mark the cutoff points for values when
		/// grouping on this column. 
		/// </summary>
		public object[] GroupCutoffs {
			get { return groupCutoffs; }
			set { groupCutoffs = value; }
		}
		private object[] groupCutoffs;

		/// <summary>
		/// 
		/// </summary>
		public string[] GroupDescriptions {
			get { return groupDescriptions; }
			set { groupDescriptions = value; }
		}
		private string[] groupDescriptions;

		/// <summary>
		/// 
		/// </summary>
		public string GroupWithItemCountFormat {
			get { return groupWithItemCountFormat; }
			set { groupWithItemCountFormat = value; }
		}
		private string groupWithItemCountFormat;

		/// <summary>
		/// 
		/// </summary>
		public string GroupWithItemCountSingularFormat {
			get { return groupWithItemCountSingularFormat; }
			set { groupWithItemCountSingularFormat = value; }
		}
		private string groupWithItemCountSingularFormat;

		/// <summary>
		/// 
		/// </summary>
		public bool Hyperlink {
			get { return hyperlink; }
			set { hyperlink = value; }
		}
		private bool hyperlink;

		/// <summary>
		/// 
		/// </summary>
		public string ImageAspectName {
			get { return imageAspectName; }
			set { imageAspectName = value; }
		}
		private string imageAspectName;

		// We actually want this to be bool? but it seems attribute properties can't be nullable types.
		// So we explicitly track if the property has been set.

		/// <summary>
		/// 
		/// </summary>
		public bool IsEditable {
			get { return isEditable; }
			set {
				isEditable = value;
				this.IsEditableSet = true;
			}
		}
		private bool isEditable = true;
		internal bool IsEditableSet = false;

		/// <summary>
		/// 
		/// </summary>
		public bool IsVisible {
			get { return isVisible; }
			set { isVisible = value; }
		}
		private bool isVisible = true;

		/// <summary>
		/// 
		/// </summary>
		public bool IsTileViewColumn {
			get { return isTileViewColumn; }
			set { isTileViewColumn = value; }
		}
		private bool isTileViewColumn;

		/// <summary>
		/// 
		/// </summary>
		public int MaximumWidth {
			get { return maximumWidth; }
			set { maximumWidth = value; }
		}
		private int maximumWidth = -1;

		/// <summary>
		/// 
		/// </summary>
		public int MinimumWidth {
			get { return minimumWidth; }
			set { minimumWidth = value; }
		}
		private int minimumWidth = -1;

		/// <summary>
		/// 
		/// </summary>
		public String Name {
			get { return name; }
			set { name = value; }
		}
		private String name;

		/// <summary>
		/// 
		/// </summary>
		public HorizontalAlignment TextAlign {
			get { return this.textAlign; }
			set { this.textAlign = value; }
		}
		private HorizontalAlignment textAlign = HorizontalAlignment.Left;

		/// <summary>
		/// 
		/// </summary>
		public String Tag {
			get { return tag; }
			set { tag = value; }
		}
		private String tag;

		/// <summary>
		/// 
		/// </summary>
		public String Title {
			get { return title; }
			set { title = value; }
		}
		private String title;

		/// <summary>
		/// 
		/// </summary>
		public String ToolTipText {
			get { return toolTipText; }
			set { toolTipText = value; }
		}
		private String toolTipText;

		/// <summary>
		/// 
		/// </summary>
		public bool TriStateCheckBoxes {
			get { return triStateCheckBoxes; }
			set { triStateCheckBoxes = value; }
		}
		private bool triStateCheckBoxes;

		/// <summary>
		/// 
		/// </summary>
		public bool UseInitialLetterForGroup {
			get { return useInitialLetterForGroup; }
			set { useInitialLetterForGroup = value; }
		}
		private bool useInitialLetterForGroup;

		/// <summary>
		/// 
		/// </summary>
		public int Width {
			get { return width; }
			set { width = value; }
		}
		private int width = 150;

		#endregion
	}
}
