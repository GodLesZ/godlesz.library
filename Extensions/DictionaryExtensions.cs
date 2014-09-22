using System.Collections.Generic;

namespace GodLesZ.Library.Extensions {

	public static class DictionaryExtensions {

		public static List<KeyValuePair<TKey, TValue>> ToList<TKey, TValue>(this Dictionary<TKey, TValue> dic) {
			List<KeyValuePair<TKey, TValue>> list = new List<KeyValuePair<TKey, TValue>>();
			foreach (KeyValuePair<TKey, TValue> pair in dic) {
				list.Add(pair);
			}
			return list;
		}

	}

}
