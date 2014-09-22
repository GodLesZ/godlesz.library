using System.Xml.Serialization;

namespace GodLesZ.Library.Network.Packets {

	/// <summary>Represents a packet version, holding all packets used in this version.</summary>
	[XmlRoot("PacketVersion")]
	public class PacketVersion {

		/// <summary>Gets or sets the version.</summary>
		[XmlAttribute()]
		public int Version {
			get;
			set;
		}

		/// <summary>Gets or sets the packets.</summary>
		[XmlArray("Packets")]
		[XmlArrayItem("Packet")]
		public PacketDefinitionList Packets {
			get;
			set;
		}


		public PacketVersion() {
			Packets = new PacketDefinitionList();
		}
	}

}
