using System.Collections.Generic;

namespace GodLesZ.Library {

	public static class ListExtensions {

		public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IList<KeyValuePair<TKey, TValue>> list) {
			Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
			foreach (KeyValuePair<TKey, TValue> pair in list) {
				dictionary.Add(pair.Key, pair.Value);
			}
			return dictionary;
		}

	}

}
