using System;
using System.Collections.Generic;

namespace GodLesZ.Library.Network.Packets {

	public class PacketHandlers {

		/// <summary>
		/// returnes the Array of Packet Handler
		/// <para>Index is the PacketID</para>
		/// </summary>
		public static Dictionary<short, PacketHandler> Handlers {
			get;
			private set;
		}


		static PacketHandlers() {
			Handlers = new Dictionary<short, PacketHandler>();
		}


		public static void Initialize() {
			Register("Default", 0x00, 0, EmptyHandler);
		}


		/// <summary>
		/// Registers a Packet to Listen for
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="packetId">PacketID</param>
		/// <param name="length">
		/// Length of the Incoming Data
		/// <para>NOTE: this must be EXACTLY the Length! Too much Data will be handled as a new Packet!</para>
		/// </param>
		/// <param name="onReceive">CallBack Function - will be called on Packet income</param>
		public static void Register(string name, short packetId, int length, OnPacketReceive onReceive) {
			if (Handlers.ContainsKey(packetId)) {
				throw new Exception(String.Format("Packet {0} already exists!", packetId));
			}

			Handlers.Add(packetId, new PacketHandler(name, packetId, length, onReceive));
		}

		/// <summary>
		/// returnes the Packet Handler for a PackeID
		/// </summary>
		/// <param name="packetId"></param>
		/// <returns></returns>
		public static PacketHandler GetHandler(short packetId) {
		    if (Handlers.ContainsKey(packetId)) {
		        return Handlers[packetId];
		    }

		    return null;
		}

		/// <summary>
		/// removes, or better "nullify" a Packet Handler
		/// </summary>
		/// <param name="packetId"></param>
		public static void RemoveHandler(short packetId) {
		    if (Handlers.ContainsKey(packetId)) {
		        Handlers[packetId] = null;
		    }
		}


	    /// <summary>
	    /// an Empty/Unused/not yet available Packet Handler
	    /// </summary>
	    /// <param name="netstate"></param>
	    /// <param name="reader"></param>
	    public static void EmptyHandler(NetState netstate, PacketReader reader) {
		}

	}

}
