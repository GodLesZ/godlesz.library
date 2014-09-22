using System;
using System.Collections.Generic;
using System.IO;
using GodLesZ.Library.Diagnostics;

namespace GodLesZ.Library.Network.Packets {

	public class PacketSendProfile : BasePacketProfile {
		private static Dictionary<Type, PacketSendProfile> mProfiles = new Dictionary<Type, PacketSendProfile>();
		private long mCreated;

		public static IEnumerable<PacketSendProfile> Profiles {
			get { return mProfiles.Values; }
		}

		public long Created {
			get { return mCreated; }
			set { mCreated = value; }
		}


		public PacketSendProfile(Type type)
			: base(type.FullName) {
		}


		public static PacketSendProfile Acquire(Type type) {
			if (!BaseProfile.Profiling)
				return null;

			PacketSendProfile prof;
			if (!mProfiles.TryGetValue(type, out prof))
				mProfiles.Add(type, prof = new PacketSendProfile(type));

			return prof;
		}

		public override void WriteTo(TextWriter op) {
			base.WriteTo(op);

			op.Write("\t{0,12:N0}", Created);
		}
	}

}
