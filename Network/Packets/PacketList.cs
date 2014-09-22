using System.Collections.Generic;
using System.Xml.Serialization;

namespace GodLesZ.Library.Network.Packets {

	/// <summary>List of packet versions.</summary>
	[XmlRoot("PacketList")]
	public class PacketList : List<PacketVersion> {

	}

}
