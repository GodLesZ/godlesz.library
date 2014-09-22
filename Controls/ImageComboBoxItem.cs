using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GodLesZ.Library.Controls {

	public class ImageComboBoxItem {

		public Color ForeColor {
			get;
			set;
		}

		public int ImageIndex {
			get;
			set;
		}

		public object Tag {
			get;
			set;
		}

		public string Text {
			get;
			set;
		}

		public Font Font {
			get;
			set;
		}

		public EImageComboBoxTextAlign TextAlign {
			get;
			set;
		}


		public ImageComboBoxItem(string text, Font font, Color foreColor)
			: this(text, font, foreColor, -1, EImageComboBoxTextAlign.Left, null) {
		}

		public ImageComboBoxItem(string text, Font font, Color foreColor, EImageComboBoxTextAlign textAlign)
			: this(text, font, foreColor, -1, textAlign, null) {
		}

		public ImageComboBoxItem(string text, Font font, Color foreColor, int imageIndex)
			: this(text, font, foreColor, imageIndex, EImageComboBoxTextAlign.Left, null) {
		}

		public ImageComboBoxItem(string text, Font font, Color foreColor, int imageIndex, EImageComboBoxTextAlign textAlign)
			: this(text, font, foreColor, imageIndex, textAlign, null) {
		}

		public ImageComboBoxItem(string text, Font font, Color foreColor, int imageIndex, EImageComboBoxTextAlign textAlign, object tag) {
			Text = text;
			Font = font;
			ForeColor = foreColor;
			ImageIndex = imageIndex;
			TextAlign = textAlign;
			Tag = tag;
		}


		public override string ToString() {
			return Text;
		}

	}

}
