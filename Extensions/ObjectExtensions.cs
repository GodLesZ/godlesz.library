using System;
using System.Globalization;

namespace GodLesZ.Library {

	public static class ObjectExtensions {

		public static bool IsNumeric(this object obj) {
			double retNum;
			bool isNumeric = Double.TryParse(obj.ToString(), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out retNum);
			return isNumeric;
		}

	}

}
