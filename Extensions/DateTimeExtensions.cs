using System;

namespace GodLesZ.Library {

	public static class DateTimeExtensions {

		public static int UnixTimestamp(this DateTime Date) {
			TimeSpan span = Date - new DateTime(1970, 1, 1);
			return Convert.ToInt32(span.TotalSeconds);
		}


		/// <summary>
		/// Uses "dd.MM.yyyy" for dates and "HH:mm:ss" for time and "Gestern" as string for yesterday.
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static string ToStringToday(this DateTime date) {
			return date.ToStringToday("dd.MM.yyyy", "HH:mm:ss", "Gestern");
		}

		/// <summary>
		/// Uses "Gestern" as string for yesterday.
		/// </summary>
		/// <param name="date"></param>
		/// <param name="formatYear"></param>
		/// <param name="formatDay"></param>
		/// <returns></returns>
		public static string ToStringToday(this DateTime date, string formatYear, string formatDay) {
			return date.ToStringToday(formatYear, formatDay, "Gestern");
		}

		public static string ToStringToday(this DateTime date, string formatYear, string formatDay, string yesterday) {
			DateTime now = DateTime.Now;
			if (now.Year == date.Year) {
				if (now.DayOfYear == date.DayOfYear) {
					return date.ToString(formatDay);
				} else if ((now.DayOfYear - 1) == date.DayOfYear) {
					return string.Format("{0}, {1}", yesterday, date.ToString(formatDay));
				}
			}

			return date.ToString(formatYear + " " + formatDay);
		}

	}

}
