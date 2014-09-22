using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GodLesZ.Library.Controls {

	public class ColorToolTip : ToolTip {
		private static Dictionary<Color, Brush> mBrushCache = new Dictionary<Color, Brush>();
		private static Dictionary<Color, Pen> mPenCache = new Dictionary<Color, Pen>();

		protected SolidBrush mBackgroundBrush;
		protected Pen mBorderPen;

		protected SizeF mDrawSize = SizeF.Empty;
		protected float mMaxDrawWidth = 0;

		public Padding TooltipPadding {
			get;
			set;
		}

		public Font ForeFont {
			get;
			set;
		}

		public Font ForeFontBold {
			get;
			set;
		}


		public ColorToolTip() {
			OwnerDraw = true;

			TooltipPadding = new Padding(0, 0, 4, 4);
			ForeFont = new Font("Tahoma", 9);
			ForeFontBold = new Font("Tahoma", 9, FontStyle.Bold);

			Draw += new DrawToolTipEventHandler(ColorToolTip_Draw);
			Popup += new PopupEventHandler(ColorToolTip_Popup);
		}


		public virtual SizeF DrawTooltip(Graphics g, bool mesureSize) {
			return new SizeF(0, 0);
		}


		#region Tooltip Events
		private void ColorToolTip_Draw(object sender, DrawToolTipEventArgs e) {
			if (mBackgroundBrush == null) {
				mBackgroundBrush = (SolidBrush)GetBrush(BackColor);
			}
			if (mBorderPen == null) {
				mBorderPen = GetPen(Color.Black, 2);
			}

			Size size = new Size(e.Bounds.Width, e.Bounds.Height);

			e.Graphics.FillRectangle(mBackgroundBrush, 0, 0, size.Width, size.Height);
			e.Graphics.DrawRectangle(mBorderPen, 0, 0, size.Width, size.Height);
			DrawTooltip(e.Graphics, false);
		}

		private void ColorToolTip_Popup(object sender, PopupEventArgs e) {
			// fake a Draw to get final Size

			SizeF finalSize;
			using (Image img = new Bitmap(e.ToolTipSize.Width, e.ToolTipSize.Height)) {
				using (Graphics g = Graphics.FromImage(img)) {
					finalSize = DrawTooltip(g, true);
				}
			}

			e.ToolTipSize = new Size((int)finalSize.Width + TooltipPadding.Right, (int)finalSize.Height + TooltipPadding.Bottom);
		}
		#endregion


		protected SizeF DrawText(Graphics g, Font font, Color col, float x, float y, string Text) {
			return DrawText(g, font, col, x, y, Text, false);
		}

		protected SizeF DrawText(Graphics g, Font font, Color col, float x, float y, string Text, bool mesureSize) {
			if (mesureSize == false) {
				g.DrawString(Text, font, new SolidBrush(col), x, y);
			}

			SizeF drawSize = g.MeasureString(Text, font);
			//if( font.Style == FontStyle.Bold )
			//	width -= 4;
			mDrawSize += drawSize;
			mMaxDrawWidth = Math.Max(mMaxDrawWidth, drawSize.Width);

			return drawSize;
		}


		private Brush GetBrush(Color color) {
			if (mBrushCache.ContainsKey(color) == false) {
				mBrushCache.Add(color, new SolidBrush(color));
			}
			return mBrushCache[color];
		}

		private Pen GetPen(Color color) {
			return GetPen(color, 1);
		}

		private Pen GetPen(Color color, float penSize) {
			if (mPenCache.ContainsKey(color) == false) {
				mPenCache.Add(color, new Pen(color, penSize));
			}
			return mPenCache[color];
		}

	}

}
