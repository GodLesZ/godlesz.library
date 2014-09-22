using System;
using System.Collections.Generic;

namespace GodLesZ.Library {

	public class SafeDictionary<TKey, TValue> : Dictionary<TKey, TValue> {
		private Object mLock = new Object();

		public new ValueCollection Values {
			get {
				lock (mLock) {
					return base.Values;
				}
			}
		}

		public new KeyCollection Keys {
			get {
				lock (mLock) {
					return base.Keys;
				}
			}
		}


		public SafeDictionary()
			: base() {
		}

		public SafeDictionary(int cap)
			: base(cap) {
		}

		public SafeDictionary(IDictionary<TKey, TValue> dictionary)
			: base(dictionary) {
		}


		new public void Add(TKey key, TValue value) {
			lock (mLock) {
				base.Add(key, value);
			}
		}

		new public bool Remove(TKey key) {
			lock (mLock) {
				return base.Remove(key);
			}
		}

		public void CopyValuesTo(TValue[] array, int index) {
			lock (mLock) {
				base.Values.CopyTo(array, index);
			}
		}

		public void CopyKeysTo(TKey[] array, int index) {
			lock (mLock) {
				base.Keys.CopyTo(array, index);
			}
		}

	}

}
