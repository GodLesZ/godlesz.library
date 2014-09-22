using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace GodLesZ.Library.Network.Packets {

	public class PacketLoader {
		private PacketList mPackets;

		public string Filepath {
			get;
			set;
		}


		public PacketLoader(string filepath) {
			Filepath = filepath;
		}


		public bool LoadPacketDefintions() {
			Assembly lookupAsm = Assembly.GetExecutingAssembly();
			return LoadPacketDefintions("GodLesZ.Library.Network.Packets", lookupAsm);
		}

		public bool LoadPacketDefintions(string packetsBaseNamespace) {
			Assembly lookupAsm = Assembly.GetExecutingAssembly();
			return LoadPacketDefintions(packetsBaseNamespace, lookupAsm);
		}

		public bool LoadPacketDefintions(string packetsBaseNamespace, Assembly lookupAsm) {
			if (File.Exists(Filepath) == false) {
				throw new Exception("Failed to load packet definition file (file not found): " + Filepath);
			}

			var xml = new XmlSerializer(typeof(PacketList));
			using (Stream fs = File.OpenRead(Filepath)) {
				mPackets = (PacketList)xml.Deserialize(fs);
			}

			// TODO: Make a defaul packet version, holding base definitions
			//		 and let them be overritten from higher versions


			// Load all types from script assemblies
			//List<Type> scriptTypes = new List<Type>();
			/*
			foreach (Assembly asm in Scripting.ScriptCompiler.Assemblies) {
				scriptTypes.AddRange(asm.GetTypes());
			}
			*/

			// Attach all packet handlers
			foreach (PacketVersion packets in mPackets) {
				foreach (PacketDefinition p in packets.Packets) {
					string asmName = string.Format("{0}.{1}", packetsBaseNamespace, p.HandlerType);
					/*
					foreach (Type type in scriptTypes) {
						if (type.FullName == asmName) {
							MethodInfo info = type.GetMethod(p.HandlerName, BindingFlags.Public | BindingFlags.Static);
							Delegate dele = Delegate.CreateDelegate(typeof(OnPacketReceive), info);
							PacketHandlers.Register(p.ID, p.Length, (OnPacketReceive)dele);
							
							found = true;
							break;
						}
					}
					*/
					var t = lookupAsm.GetType(asmName);
					if (t == null) {
						throw new Exception("Unable to find namespace: " + asmName);
					}

					var info = t.GetMethod(p.HandlerName, BindingFlags.Public | BindingFlags.Static);
					if (info == null) {
						throw new Exception("Unable to find Packet handler for definition: " + string.Format("{0}.{1}", p.HandlerType, p.HandlerName));
					}
					var dele = Delegate.CreateDelegate(typeof (OnPacketReceive), info);
					PacketHandlers.Register(p.HandlerName, p.ID, p.Length, (OnPacketReceive)dele);
				}
			}

			return true;
		}
	}

}
