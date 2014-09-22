using System;
using System.ComponentModel;
using System.Drawing;

namespace GodLesZ.Library.Controls {
	/// <summary>
	/// The interface for an object which can draw itself over the top of
	/// an ObjectListView.
	/// </summary>
	public interface IOverlay {
		/// <summary>
		/// Draw this overlay
		/// </summary>
		/// <param name="olv">The ObjectListView that is being overlaid</param>
		/// <param name="g">The Graphics onto the given OLV</param>
		/// <param name="r">The content area of the OLV</param>
		void Draw(ObjectListView olv, Graphics g, Rectangle r);
	}

	/// <summary>
	/// An interface for an overlay that supports variable levels of transparency
	/// </summary>
	public interface ITransparentOverlay : IOverlay {
		/// <summary>
		/// Gets or sets the transparency of the overlay. 
		/// 0 is completely transparent, 255 is completely opaque.
		/// </summary>
		int Transparency { get; set; }
	}

	/// <summary>
	/// A null implementation of the IOverlay interface
	/// </summary>
	public class AbstractOverlay : ITransparentOverlay {
		#region IOverlay Members

		/// <summary>
		/// Draw this overlay
		/// </summary>
		/// <param name="olv">The ObjectListView that is being overlaid</param>
		/// <param name="g">The Graphics onto the given OLV</param>
		/// <param name="r">The content area of the OLV</param>
		public virtual void Draw(ObjectListView olv, Graphics g, Rectangle r) {
		}

		#endregion

		#region ITransparentOverlay Members

		/// <summary>
		/// How transparent should this overlay be?
		/// </summary>
		[Category("ObjectListView"),
		 Description("How transparent should this overlay be"),
		 DefaultValue(128),
		 NotifyParentProperty(true)]
		public int Transparency {
			get { return this.transparency; }
			set { this.transparency = Math.Min(255, Math.Max(0, value)); }
		}
		private int transparency = 128;

		#endregion
	}

	/// <summary>
	/// An overlay that will draw an image over the top of the ObjectListView
	/// </summary>
	[TypeConverter("GodLesZ.Library.Controls.Design.OverlayConverter")]
	public class ImageOverlay : ImageAdornment, ITransparentOverlay {
		/// <summary>
		/// Create an ImageOverlay
		/// </summary>
		public ImageOverlay() {
			this.Alignment = System.Drawing.ContentAlignment.BottomRight;
		}

		#region Public properties

		/// <summary>
		/// Gets or sets the horizontal inset by which the position of the overlay will be adjusted
		/// </summary>
		[Category("ObjectListView"),
		 Description("The horizontal inset by which the position of the overlay will be adjusted"),
		 DefaultValue(20),
		 NotifyParentProperty(true)]
		public int InsetX {
			get { return this.insetX; }
			set { this.insetX = Math.Max(0, value); }
		}
		private int insetX = 20;

		/// <summary>
		/// Gets or sets the vertical inset by which the position of the overlay will be adjusted
		/// </summary>
		[Category("ObjectListView"),
		 Description("Gets or sets the vertical inset by which the position of the overlay will be adjusted"),
		 DefaultValue(20),
		 NotifyParentProperty(true)]
		public int InsetY {
			get { return this.insetY; }
			set { this.insetY = Math.Max(0, value); }
		}
		private int insetY = 20;

		#endregion

		#region Commands

		/// <summary>
		/// Draw this overlay
		/// </summary>
		/// <param name="olv">The ObjectListView being decorated</param>
		/// <param name="g">The Graphics used for drawing</param>
		/// <param name="r">The bounds of the rendering</param>
		public virtual void Draw(ObjectListView olv, Graphics g, Rectangle r) {
			Rectangle insetRect = r;
			insetRect.Inflate(-this.InsetX, -this.InsetY);

			// We hard code a transparency of 255 here since transparency is handled by the glass panel
			this.DrawImage(g, insetRect, this.Image, 255);
		}

		#endregion
	}

	/// <summary>
	/// An overlay that will draw text over the top of the ObjectListView
	/// </summary>
	[TypeConverter("GodLesZ.Library.Controls.Design.OverlayConverter")]
	public class TextOverlay : TextAdornment, ITransparentOverlay {
		/// <summary>
		/// Create a TextOverlay
		/// </summary>
		public TextOverlay() {
			this.Alignment = System.Drawing.ContentAlignment.BottomRight;
		}

		#region Public properties

		/// <summary>
		/// Gets or sets the horizontal inset by which the position of the overlay will be adjusted
		/// </summary>
		[Category("ObjectListView"),
		 Description("The horizontal inset by which the position of the overlay will be adjusted"),
		 DefaultValue(20),
		 NotifyParentProperty(true)]
		public int InsetX {
			get { return this.insetX; }
			set { this.insetX = Math.Max(0, value); }
		}
		private int insetX = 20;

		/// <summary>
		/// Gets or sets the vertical inset by which the position of the overlay will be adjusted
		/// </summary>
		[Category("ObjectListView"),
		 Description("Gets or sets the vertical inset by which the position of the overlay will be adjusted"),
		 DefaultValue(20),
		 NotifyParentProperty(true)]
		public int InsetY {
			get { return this.insetY; }
			set { this.insetY = Math.Max(0, value); }
		}
		private int insetY = 20;

		/// <summary>
		/// Gets or sets whether the border will be drawn with rounded corners
		/// </summary>
		[Browsable(false),
		 Obsolete("Use CornerRounding instead", false),
		 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool RoundCorneredBorder {
			get { return this.CornerRounding > 0; }
			set {
				if (value)
					this.CornerRounding = 16.0f;
				else
					this.CornerRounding = 0.0f;
			}
		}

		#endregion

		#region Commands

		/// <summary>
		/// Draw this overlay
		/// </summary>
		/// <param name="olv">The ObjectListView being decorated</param>
		/// <param name="g">The Graphics used for drawing</param>
		/// <param name="r">The bounds of the rendering</param>
		public virtual void Draw(ObjectListView olv, Graphics g, Rectangle r) {
			Rectangle insetRect = r;
			insetRect.Inflate(-this.InsetX, -this.InsetY);
			// We hard code a transparency of 255 here since transparency is handled by the glass panel
			this.DrawText(g, insetRect, this.Text, 255);
		}

		#endregion
	}

	/// <summary>
	/// A Billboard overlay is a TextOverlay positioned at an absolute point
	/// </summary>
	public class BillboardOverlay : TextOverlay {
		/// <summary>
		/// Create a BillboardOverlay
		/// </summary>
		public BillboardOverlay() {
			this.Transparency = 255;
			this.BackColor = Color.PeachPuff;
			this.TextColor = Color.Black;
			this.BorderColor = Color.Empty;
			this.Font = new Font("Tahoma", 10);
		}

		/// <summary>
		/// Gets or sets where should the top left of the billboard be placed
		/// </summary>
		public Point Location {
			get { return this.location; }
			set { this.location = value; }
		}
		private Point location;

		/// <summary>
		/// Draw this overlay
		/// </summary>
		/// <param name="olv">The ObjectListView being decorated</param>
		/// <param name="g">The Graphics used for drawing</param>
		/// <param name="r">The bounds of the rendering</param>
		public override void Draw(ObjectListView olv, Graphics g, Rectangle r) {
			if (String.IsNullOrEmpty(this.Text))
				return;

			// Calculate the bounds of the text, and then move it to where it should be
			Rectangle textRect = this.CalculateTextBounds(g, r, this.Text);
			textRect.Location = this.Location;

			// Make sure the billboard is within the bounds of the List, as far as is possible
			if (textRect.Right > r.Width)
				textRect.X = Math.Max(r.Left, r.Width - textRect.Width);
			if (textRect.Bottom > r.Height)
				textRect.Y = Math.Max(r.Top, r.Height - textRect.Height);

			this.DrawBorderedText(g, textRect, this.Text, 255);
		}
	}
}
