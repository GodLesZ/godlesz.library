using System;

namespace GodLesZ.Library.Network.Packets {

	[Flags()]
	public enum EPacketState {
		/// <summary>
		/// Not yet used/sent
		/// </summary>
		Inactive = 0x00,
		/// <summary>
		/// May used/sent multiple times
		/// </summary>
		Static = 0x01,
		/// <summary>
		/// Add to working queue
		/// </summary>
		Acquired = 0x02,
		/// <summary>
		/// Data/infos has been used
		/// </summary>
		Accessed = 0x04,
		/// <summary>
		/// Unused
		/// </summary>
		Buffered = 0x08,
		/// <summary>
		/// Was access multiple times or similar errors
		/// </summary>
		Warned = 0x10
	}

}
