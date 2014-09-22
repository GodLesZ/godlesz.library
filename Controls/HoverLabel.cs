using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GodLesZ.Library.Controls {

	public class HoverLabel : Label {
		protected Color mBaseForeColor;

		[Description("Hover color")]
		public Color ForeColorHover {
			get;
			set;
		}

		public Image ArrowImage {
			get;
			set;
		}

		public EHoverLabelArrow ArrowDirection {
			get;
			set;
		}

		public Padding ArrowPadding {
			get;
			set;
		}


		public HoverLabel() {
			ForeColorHover = Color.White;
			Cursor = Cursors.Hand;
			ArrowImage = null;
			ArrowDirection = EHoverLabelArrow.None;
			ArrowPadding = Padding.Empty;
		}


		protected override void OnMouseHover(EventArgs e) {
			mBaseForeColor = ForeColor;
			ForeColor = ForeColorHover;

			base.OnMouseHover(e);
		}

		protected override void OnMouseLeave(EventArgs e) {
			ForeColor = mBaseForeColor;

			base.OnMouseLeave(e);
		}

		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);

			if (ArrowDirection == EHoverLabelArrow.None || ArrowImage == null) {
				return;
			}

			if (ArrowDirection == EHoverLabelArrow.Left) {
				e.Graphics.DrawImageUnscaled(ArrowImage, new Point(ArrowPadding.Left, ClientSize.Height / 2 - ArrowImage.Height / 2 + ArrowPadding.Top));
			} else {
				e.Graphics.DrawImageUnscaled(ArrowImage, new Point(ClientSize.Width - ArrowImage.Width + ArrowPadding.Right, ClientSize.Height / 2 - ArrowImage.Height / 2 + ArrowPadding.Top));
			}
		}

	}

}
