using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GodLesZ.Library.PixelSearch {

	public static class Extensions {

		public static Color FromHex(string Hexstring) {
			return ColorTranslator.FromHtml(Hexstring);
		}

		public static string ToHex(this Color Color) {
			return ColorTranslator.ToHtml(Color);
		}

		public static bool WithinShade(this Color baseColor, Color compareColor, Color shade) {
			if (baseColor.R < (compareColor.R - shade.R) || baseColor.R > (compareColor.R + shade.R)) {
				return false;
			}
			if (baseColor.G < (compareColor.G - shade.G) || baseColor.G > (compareColor.G + shade.G)) {
				return false;
			}
			if (baseColor.B < (compareColor.B - shade.B) || baseColor.B > (compareColor.B + shade.B)) {
				return false;
			}

			return true;
		}

	}

}
