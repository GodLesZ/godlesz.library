
namespace GodLesZ.Library {

	public static class IntegerExtensions {

		public static string FormatFileSize(this int sizeInBytes) {
			string sizeString = "";

			if (sizeInBytes < 0) {
				uint sizeInBytesPositive = (uint)(sizeInBytes * -1);
				sizeString = sizeInBytesPositive.FormatFileSize();
				sizeString = "-" + sizeString;
			} else {
				sizeString = ((uint)sizeInBytes).FormatFileSize();
			}

			return sizeString;
		}

		public static string FormatFileSize(this uint sizeInBytes) {
			string sizeString = "";

			if (sizeInBytes >= 1073741824) {
				decimal size = decimal.Divide(sizeInBytes, 1073741824);
				sizeString = string.Format("{0:##.##} GB", size);
			} else if (sizeInBytes >= 1048576) {
				decimal size = decimal.Divide(sizeInBytes, 1048576);
				sizeString = string.Format("{0:##.##} MB", size);
			} else if (sizeInBytes >= 1024) {
				decimal size = decimal.Divide(sizeInBytes, 1024);
				sizeString = string.Format("{0:##.##} KB", size);
			} else {
				sizeString = string.Format("{0} B", sizeInBytes);
			}

			return sizeString;
		}

	}

}
