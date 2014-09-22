using System;

namespace GodLesZ.Library {

	public abstract class Singleton<T> where T : new() {
		protected static Lazy<T> mInstance = new Lazy<T>(() => new T());

		public static T Instance {
			get {
				return mInstance.Value;
			}
		}

	}

}