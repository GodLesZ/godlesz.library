using Color = System.Drawing.Color;

namespace GodLesZ.Library {

	public static class ColorExtensions {

		public static string ToHex(this Color col) {
			return System.Drawing.ColorTranslator.ToHtml(col);
		}

		public static Color HexToColor(this string col) {
			return System.Drawing.ColorTranslator.FromHtml(col);
		}

	}

}
