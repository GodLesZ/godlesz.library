using System.Drawing;

namespace GodLesZ.Library {

	public static class GraphicsExtensions {

		public static void DrawImagePropertional(this Graphics g, Bitmap image, int x, int y, float percent) {
			// Shrink base image based on numeric input
			float newWidth = image.Width * percent;
			float newHeight = image.Height * percent;
			float ratio;

			if (image.Width > image.Height) {
				ratio = newWidth / image.Width;
				newHeight = image.Height * ratio;
			} else {
				ratio = newHeight / image.Height;
				newWidth = image.Width * ratio;
			}

			g.DrawImage(image, x, y, newWidth, newHeight);
		}

	}

}
