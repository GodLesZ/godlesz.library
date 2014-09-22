using System.IO;

namespace GodLesZ.Library {

	public static class StreamExtensions {

		public static string ReadWord(this BinaryReader bin, int max) {
			string word = "";

			while (word.Length < max && bin.BaseStream.CanRead == true && bin.PeekChar() != '\0') {
				word += bin.ReadChar();
			}

			if (word.Length < max) {
				bin.BaseStream.Position += (max - word.Length);
			}
			return word;
		}

	}

}
