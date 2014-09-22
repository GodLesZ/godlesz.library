using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GodLesZ.Library.Controls {
	/// <summary>
	/// ColumnComparer is the workhorse for all comparison between two values of a particular column.
	/// If the column has a specific comparer, use that to compare the values. Otherwise, do
	/// a case insensitive string compare of the string representations of the values.
	/// </summary>
	/// <remarks><para>This class inherits from both IComparer and its generic counterpart
	/// so that it can be used on untyped and typed collections.</para></remarks>
	public class ColumnComparer : IComparer, IComparer<OLVListItem> {
		/// <summary>
		/// Create a ColumnComparer that will order the rows in a list view according
		/// to the values in a given column
		/// </summary>
		/// <param name="col">The column whose values will be compared</param>
		/// <param name="order">The ordering for column values</param>
		public ColumnComparer(OLVColumn col, SortOrder order) {
			this.mColumn = col;
			this.mSortOrder = order;
		}

		/// <summary>
		/// Create a ColumnComparer that will order the rows in a list view according
		/// to the values in a given column, and by a secondary column if the primary
		/// column is equal.
		/// </summary>
		/// <param name="col">The column whose values will be compared</param>
		/// <param name="order">The ordering for column values</param>
		/// <param name="col2">The column whose values will be compared for secondary sorting</param>
		/// <param name="order2">The ordering for secondary column values</param>
		public ColumnComparer(OLVColumn col, SortOrder order, OLVColumn col2, SortOrder order2)
			: this(col, order) {
			// There is no point in secondary sorting on the same column
			if (col != col2)
				mSecondComparer = new ColumnComparer(col2, order2);
		}

		/// <summary>
		/// Compare two rows
		/// </summary>
		/// <param name="x">row1</param>
		/// <param name="y">row2</param>
		/// <returns>An ordering indication: -1, 0, 1</returns>
		public int Compare(object x, object y) {
			return this.Compare((OLVListItem)x, (OLVListItem)y);
		}

		/// <summary>
		/// Compare two rows
		/// </summary>
		/// <param name="x">row1</param>
		/// <param name="y">row2</param>
		/// <returns>An ordering indication: -1, 0, 1</returns>
		public int Compare(OLVListItem x, OLVListItem y) {
			if (this.mSortOrder == SortOrder.None)
				return 0;

			int result = 0;
			object x1 = this.mColumn.GetValue(x.RowObject);
			object y1 = this.mColumn.GetValue(y.RowObject);

			// Handle nulls. Null values come last
			bool xIsNull = (x1 == null || x1 == System.DBNull.Value);
			bool yIsNull = (y1 == null || y1 == System.DBNull.Value);
			if (xIsNull || yIsNull) {
				if (xIsNull && yIsNull)
					result = 0;
				else
					result = (xIsNull ? -1 : 1);
			} else {
				result = this.CompareValues(x1, y1);
			}

			if (this.mSortOrder == SortOrder.Descending)
				result = 0 - result;

			// If the result was equality, use the secondary comparer to resolve it
			if (result == 0 && mSecondComparer != null)
				result = mSecondComparer.Compare(x, y);

			return result;
		}

		/// <summary>
		/// Compare the actual values to be used for sorting
		/// </summary>
		/// <param name="x">The aspect extracted from the first row</param>
		/// <param name="y">The aspect extracted from the second row</param>
		/// <returns>An ordering indication: -1, 0, 1</returns>
		public int CompareValues(object x, object y) {
			if (x is IComparable) {
				return (x as IComparable).CompareTo(y);
			}

			// Force case insensitive compares on strings
			String xAsString = x as String;
			if (xAsString != null) {
				return String.Compare(xAsString, (String)y, StringComparison.CurrentCultureIgnoreCase);
			} else {
				IComparable comparable = x as IComparable;
				if (comparable != null)
					return comparable.CompareTo(y);
				else
					return 0;
			}
		}

		protected OLVColumn mColumn;
		protected SortOrder mSortOrder;
		protected ColumnComparer mSecondComparer;
	}


	/// <summary>
	/// This comparer sort list view groups. OLVGroups have a "SortValue" property,
	/// which is used if present. Otherwise, the titles of the groups will be compared.
	/// </summary>
	public class OLVGroupComparer : IComparer<OLVGroup> {
		/// <summary>
		/// Create a group comparer
		/// </summary>
		/// <param name="order">The ordering for column values</param>
		public OLVGroupComparer(SortOrder order) {
			this.sortOrder = order;
		}

		/// <summary>
		/// Compare the two groups. OLVGroups have a "SortValue" property,
		/// which is used if present. Otherwise, the titles of the groups will be compared.
		/// </summary>
		/// <param name="x">group1</param>
		/// <param name="y">group2</param>
		/// <returns>An ordering indication: -1, 0, 1</returns>
		public int Compare(OLVGroup x, OLVGroup y) {
			// If we can compare the sort values, do that.
			// Otherwise do a case insensitive compare on the group header.
			int result;
			if (x.SortValue != null && y.SortValue != null)
				result = x.SortValue.CompareTo(y.SortValue);
			else
				result = String.Compare(x.Header, y.Header, StringComparison.CurrentCultureIgnoreCase);

			if (this.sortOrder == SortOrder.Descending)
				result = 0 - result;

			return result;
		}

		private SortOrder sortOrder;
	}

	/// <summary>
	/// This comparer can be used to sort a collection of model objects by a given column
	/// </summary>
	public class ModelObjectComparer : IComparer, IComparer<object> {
		/// <summary>
		/// Create a model object comparer
		/// </summary>
		/// <param name="col"></param>
		/// <param name="order"></param>
		public ModelObjectComparer(OLVColumn col, SortOrder order) {
			this.column = col;
			this.sortOrder = order;
		}

		/// <summary>
		/// Create a model object comparer with a secondary sorting column
		/// </summary>
		/// <param name="col"></param>
		/// <param name="order"></param>
		/// <param name="col2"></param>
		/// <param name="order2"></param>
		public ModelObjectComparer(OLVColumn col, SortOrder order, OLVColumn col2, SortOrder order2)
			: this(col, order) {
			// There is no point in secondary sorting on the same column
			if (col != col2 && col2 != null && order2 != SortOrder.None)
				this.secondComparer = new ModelObjectComparer(col2, order2);
		}

		/// <summary>
		/// Compare the two model objects
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public int Compare(object x, object y) {
			int result = 0;
			object x1 = this.column.GetValue(x);
			object y1 = this.column.GetValue(y);

			if (this.sortOrder == SortOrder.None)
				return 0;

			// Handle nulls. Null values come last
			bool xIsNull = (x1 == null || x1 == System.DBNull.Value);
			bool yIsNull = (y1 == null || y1 == System.DBNull.Value);
			if (xIsNull || yIsNull) {
				if (xIsNull && yIsNull)
					result = 0;
				else
					result = (xIsNull ? -1 : 1);
			} else {
				result = this.CompareValues(x1, y1);
			}

			if (this.sortOrder == SortOrder.Descending)
				result = 0 - result;

			// If the result was equality, use the secondary comparer to resolve it
			if (result == 0 && this.secondComparer != null)
				result = this.secondComparer.Compare(x, y);

			return result;
		}

		/// <summary>
		/// Compare the actual values
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public int CompareValues(object x, object y) {
			if (x is IComparable) {
				return (x as IComparable).CompareTo(y);
			}

			// Force case insensitive compares on strings
			String xStr = x as String;
			if (xStr != null)
				return String.Compare(xStr, (String)y, StringComparison.CurrentCultureIgnoreCase);
			else {
				IComparable comparable = x as IComparable;
				if (comparable != null)
					return comparable.CompareTo(y);
				else
					return 0;
			}
		}

		private OLVColumn column;
		private SortOrder sortOrder;
		private ModelObjectComparer secondComparer;

		#region IComparer<object> Members

		#endregion
	}

}