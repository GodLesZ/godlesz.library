namespace GodLesZ.Library.Network.Packets {

	public delegate void OnPacketReceive(NetState netstate, PacketReader reader);

	public class PacketHandler {
		/// <summary>
		/// returnes the PacketID
		/// </summary>
		public short PacketID {
			get;
			private set;
		}

		/// <summary>
		/// returnes the Length of byte Code/Data
		/// </summary>
		public int Length {
			get;
			private set;
		}

		/// <summary>
		/// CallBack Function - triggers on Packet receive
		/// </summary>
		public OnPacketReceive OnReceive {
			get;
			private set;
		}

		public string Name {
			get;
			private set;
		}


		/// <summary>
		/// Registers a new PacketHandler for a single Packet
		/// </summary>
		/// <param name="name"></param>
		/// <param name="packetID"></param>
		/// <param name="length"></param>
		/// <param name="onReceive"></param>
		public PacketHandler(string name, short packetID, int length, OnPacketReceive onReceive) {
			Name = name;
			PacketID = packetID;
			Length = length;
			OnReceive = onReceive;
		}

	}

}
