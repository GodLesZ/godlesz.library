using System.Collections.Generic;
using System.Xml.Serialization;

namespace GodLesZ.Library.Network.Packets {

	/// <summary>List of packet definitions.</summary>
	[XmlRoot("Packets")]
	public class PacketDefinitionList : List<PacketDefinition> {

	}

}
