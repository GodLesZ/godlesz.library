using System;
using System.Collections.Generic;
using System.Net;

namespace GodLesZ.Library.Network {

	public static class IPAdressExtensions {
		private static Dictionary<IPAddress, IPAddress> mIPAddressTable;
		
		public static IPAddress Intern(this IPAddress ipAddress) {
			if (mIPAddressTable == null) {
				mIPAddressTable = new Dictionary<IPAddress, IPAddress>();
			}

			IPAddress interned;
			if (mIPAddressTable.TryGetValue(ipAddress, out interned) == false) {
				interned = ipAddress;
				mIPAddressTable[ipAddress] = interned;
			}

			return interned;
		}

		public static void Intern(ref IPAddress ipAddress) {
			ipAddress = ipAddress.Intern();
		}

	}

}