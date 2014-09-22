﻿

using System.Collections;
using System.Collections.Generic;

namespace GodLesZ.Library.Win7.Net {
	/// <summary>
	/// An enumerable collection of <see cref="NetworkConnection"/> objects.
	/// </summary>
	public class NetworkConnectionCollection : IEnumerable<NetworkConnection> {
		#region Private Fields

		IEnumerable networkConnectionEnumerable;

		#endregion // Private Fields

		internal NetworkConnectionCollection(IEnumerable networkConnectionEnumerable) {
			this.networkConnectionEnumerable = networkConnectionEnumerable;
		}

		#region IEnumerable<NetworkConnection> Members

		/// <summary>
		/// Returns the strongly typed enumerator for this collection.
		/// </summary>
		/// <returns>A <see cref="System.Collections.Generic.IEnumerator{T}"/> object.</returns>
		public IEnumerator<NetworkConnection> GetEnumerator() {
			foreach (INetworkConnection networkConnection in networkConnectionEnumerable) {
				yield return new NetworkConnection(networkConnection);
			}
		}

		#endregion

		#region IEnumerable Members

		/// <summary>
		/// Returns the enumerator for this collection.
		/// </summary>
		///<returns>A <see cref="System.Collections.IEnumerator"/> object.</returns> 
		IEnumerator IEnumerable.GetEnumerator() {
			foreach (INetworkConnection networkConnection in networkConnectionEnumerable) {
				yield return new NetworkConnection(networkConnection);
			}
		}

		#endregion
	}
}