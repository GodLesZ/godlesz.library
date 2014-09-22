using System;
using System.IO;

namespace GodLesZ.Library.Formats {

	public abstract class GenericFileFormatData : IGenericFileFormatData, IDisposable {
		protected BinaryReader mReader;
		protected GenericFileFormatVersion mVersion;


		public GenericFileFormatData(BinaryReader reader, GenericFileFormatVersion version) {
			mReader = reader;
			mVersion = version;

			if (mReader != null) {
				Read();
			}
		}


		public virtual void Read() {

		}

		public virtual void Dispose() {

		}

	}

}
