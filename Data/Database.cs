using System;
using System.Configuration;
using System.Data;

namespace GodLesZ.Library.Data {

	/// <summary>
	/// Convenience class for opening/executing data
	/// </summary>
	public static class Database {

		/// <exception cref="InvalidOperationException">Need a connection string name - can't determine what it is</exception>
		public static DynamicModel Current {
			get {
				if (ConfigurationManager.ConnectionStrings.Count > 1) {
					return new DynamicModel(ConfigurationManager.ConnectionStrings[1].Name);
				}
				throw new InvalidOperationException("Need a connection string name - can't determine what it is");
			}
		}

	}

}