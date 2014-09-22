
namespace GodLesZ.Library {

	public static class StringExtensions {

		public static string JoinArray(this string[] array) {
			return array.JoinArray(", ");
		}

		public static string JoinArray(this string[] array, string sep) {
			string result = "";

			for (int i = 0; i < array.Length; i++) {
				result += array[i];
				if ((i + 1) < array.Length) {
					result += sep;
				}
			}
			return result;
		}


		public static string FirstCharToUpper(this string s) {
			if (string.IsNullOrEmpty(s)) {
				return string.Empty;
			}
			char[] a = s.ToCharArray();
			a[0] = char.ToUpper(a[0]);
			return new string(a);
		}

		public static string FirstCharToLower(this string s) {
			if (string.IsNullOrEmpty(s)) {
				return string.Empty;
			}
			char[] a = s.ToCharArray();
			a[0] = char.ToLower(a[0]);
			return new string(a);
		}

	}

}
