using System;
using System.Collections.Generic;
using GodLesZ.Library.Diagnostics;

namespace GodLesZ.Library.Network.Packets {

	public class PacketReceiveProfile : BasePacketProfile {
		private static Dictionary<int, PacketReceiveProfile> mProfiles = new Dictionary<int, PacketReceiveProfile>();

		public static IEnumerable<PacketReceiveProfile> Profiles {
			get { return mProfiles.Values; }
		}

		public PacketReceiveProfile(int packetId)
			: base(String.Format("0x{0:X2}", packetId)) {
		}


		public static PacketReceiveProfile Acquire(int packetId) {
			if (!BaseProfile.Profiling) {
				return null;
			}

			PacketReceiveProfile prof;

			if (!mProfiles.TryGetValue(packetId, out prof)) {
				mProfiles.Add(packetId, prof = new PacketReceiveProfile(packetId));
			}

			return prof;
		}

	}

}
