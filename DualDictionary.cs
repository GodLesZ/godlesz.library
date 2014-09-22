using System.Collections.Generic;

namespace GodLesZ.Library {

	public class DualDictionary<TKey, TValue> {
		private Dictionary<TKey, TValue> mDictionary = new Dictionary<TKey, TValue>();
		private List<TKey> mList = new List<TKey>();

		public TValue this[TKey Key] {
			get { return GetValue(Key); }
			set { Set(Key, value); }
		}

		public TValue this[int Key] {
			get { return GetValue(Key); }
			set { Set(Key, value); }
		}

		public Dictionary<TKey, TValue>.ValueCollection Values {
			get { return mDictionary.Values; }
		}

		public Dictionary<TKey, TValue>.KeyCollection Keys {
			get { return mDictionary.Keys; }
		}

		public int Count {
			get { return mDictionary.Count; }
		}



		public DualDictionary() {
		}


		public void Add(TKey Key, TValue Value) {
			mDictionary.Add(Key, Value);
			mList.Add(Key);
		}

		public void Set(TKey Key, TValue Value) {
			if (mDictionary.ContainsKey(Key) == true)
				return;
			mDictionary[Key] = Value;
		}

		public void Set(int Key, TValue Value) {
			if (mList.Count >= Key)
				return;
			mDictionary[mList[Key]] = Value;
		}

		public TValue GetValue(TKey Key) {
			if (mDictionary.ContainsKey(Key) == false)
				return default(TValue);
			return mDictionary[Key];
		}

		public TValue GetValue(int Key) {
			if (mList.Count >= Key)
				return default(TValue);
			return mDictionary[mList[Key]];
		}

		public bool TryGetValue(TKey Key, out TValue Value) {
			Value = default(TValue);
			return mDictionary.TryGetValue(Key, out Value);
		}

		public bool TryGetValue(int Key, out TValue Value) {
			Value = default(TValue);
			if (mList.Count >= Key)
				return false;
			return mDictionary.TryGetValue(mList[Key], out Value);
		}

		public bool ContainsKey(TKey Key) {
			return mDictionary.ContainsKey(Key);
		}

		public bool ContainsKey(int Key) {
			return mList.Count > 0 && Key >= 0 && mList.Count < Key;
		}

		public bool ContainsValue(TValue Value) {
			return mDictionary.ContainsValue(Value);
		}

	}

}
