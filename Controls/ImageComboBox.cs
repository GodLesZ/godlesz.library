using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GodLesZ.Library.Controls {

	public class ImageComboBox : ComboBox {
		private ImageList mImageList = null;
		private int mImagePlace = 0;

		[DefaultValue(null)]
		public ImageList ImageList {
			get { return mImageList; }
			set {
				mImageList = value;
				if (mImageList != null) {
					ItemHeight = mImageList.ImageSize.Height;
				}
			}
		}

		[DefaultValue(typeof(Color), "SystemColors.ControlLight")]
		public Color BackgroundColorItemFocused {
			get;
			set;
		}

		[DefaultValue(typeof(Color), "SystemColors.ControlLightLight")]
		public Color BackgroundColorItemSelected {
			get;
			set;
		}

		public int ImagePlace {
			get { return mImagePlace; }
			set { mImagePlace = value; }
		}


		public ImageComboBox() {
			DrawMode = DrawMode.OwnerDrawFixed;
			DropDownStyle = ComboBoxStyle.DropDownList;
		}


		public int IndexOf(string Text) {
			for (int i = 1; i < Items.Count; i++) {
				ImageComboBoxItem item = (Items[i] as ImageComboBoxItem);
				if (item.Text == Text)
					return i;
			}
			return -1;
		}

		public bool Contains(string Text) {
			return IndexOf(Text) != -1;
		}

		protected override void OnDrawItem(System.Windows.Forms.DrawItemEventArgs e) {
			base.OnDrawItem(e);

			//DrawItemState stateFocus = (/*DrawItemState.Focus | */DrawItemState.Selected);
			//DrawItemState stateSelected = DrawItemState.ComboBoxEdit;
			DrawItemState stateFocus = DrawItemState.Focus;
			DrawItemState stateSelected = DrawItemState.Selected;
			if ((e.State & stateSelected) == stateSelected) {
				e.Graphics.FillRectangle(new SolidBrush(BackgroundColorItemSelected), e.Bounds);
			} else if ((e.State & stateFocus) == stateFocus) {
				e.Graphics.FillRectangle(new SolidBrush(BackgroundColorItemFocused), e.Bounds);
			} else {
				e.Graphics.FillRectangle(new SolidBrush(Parent.BackColor), e.Bounds);
			}
			//e.DrawFocusRectangle();

			if (e.Index < 0)
				return;

			if (!(this.Items[e.Index] is ImageComboBoxItem)) {
				//System.Diagnostics.Debug.WriteLine(this.Items[e.Index] + ": " + e.State);
				e.Graphics.DrawString(Items[e.Index].ToString(), e.Font, new SolidBrush(ForeColor), e.Bounds.Left, e.Bounds.Top);
				return;
			}
			//System.Diagnostics.Debug.WriteLine((this.Items[e.Index] as ImageComboBoxItem).Text + ": " + e.State);

			ImageComboBoxItem CurrItem = this.Items[e.Index] as ImageComboBoxItem;
			SizeF fontSize = e.Graphics.MeasureString(CurrItem.Text, CurrItem.Font);
			SolidBrush brush = new SolidBrush(CurrItem.ForeColor);
			int imageX = e.Bounds.Left;
			if (CurrItem.Text != string.Empty && CurrItem.TextAlign == EImageComboBoxTextAlign.Left)
				e.Graphics.DrawString(CurrItem.Text, CurrItem.Font, brush, e.Bounds.Left, e.Bounds.Top + (mImageList.ImageSize.Height / 2) - fontSize.Height / 2);

			if (mImageList != null && CurrItem.ImageIndex != -1) {
				if (CurrItem.TextAlign == EImageComboBoxTextAlign.Left)
					imageX += (int)fontSize.Width;

				if (mImagePlace > imageX)
					imageX = mImagePlace;
				ImageList.Draw(e.Graphics, imageX, e.Bounds.Top, CurrItem.ImageIndex);
			}

			if (CurrItem.Text != string.Empty && CurrItem.TextAlign == EImageComboBoxTextAlign.Right) {
				imageX += ImageList.ImageSize.Width + 10;
				e.Graphics.DrawString(CurrItem.Text, CurrItem.Font, brush, imageX, e.Bounds.Top + (mImageList.ImageSize.Height / 2) - fontSize.Height / 2);
			}

		}

	}

}
