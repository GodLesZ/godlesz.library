using System.IO;

namespace GodLesZ.Library.Formats {

	public interface IFileFormat {

		bool Read();
		bool Read(string filepath);
		bool Read(Stream stream);
		bool Write(string filePath, bool overwrite);
		void Flush();

	}

}
