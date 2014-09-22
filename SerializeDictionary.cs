using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace GodLesZ.Library {

	public class SerializeDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable {

		public XmlSchema GetSchema() {
			return null;
		}

		public void ReadXml(XmlReader reader) {
			Type t = typeof(TKey);

			var fields = t.GetFields(BindingFlags.Public | BindingFlags.SetField);

		}

		public void WriteXml(XmlWriter writer) {

		}

	}

}
