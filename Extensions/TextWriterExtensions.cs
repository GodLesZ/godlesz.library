using System;
using System.IO;
using System.Text;

namespace GodLesZ.Library.Extensions {
	
	public static class TextWriterExtensions {

		public static void WriteFormattedBuffer(this TextWriter output, Stream input, int lengthInBytes) {
			// Hex-blocks of 1 byte per line
			const int bytesPerLine = 16;
			// Amount of spaces in front of each line
			// Should be at least 5, cuz line numbers are padded until 4
			const int preSpaceCount = 7;

			// Header
			output.WriteLine("== Data  - " + string.Format("{0:0000}", lengthInBytes) + " bytes @ " + DateTime.Now);
			output.WriteLine("        0  1  2  3  4  5  6  7   8  9  A  B  C  D  E  F");
			output.WriteLine("       -- -- -- -- -- -- -- --  -- -- -- -- -- -- -- --");

			var byteIndex = 0;
			var amountOfLines = lengthInBytes / bytesPerLine;
			var reminder = lengthInBytes & (bytesPerLine - 1);

			// 1 byte = 2 hex digits + 1 space each block + 1 single space after 7 digits
			const int byteBufferSize = (3 * bytesPerLine) + 1;

			for (var i = 0; i < amountOfLines; ++i, byteIndex += bytesPerLine) {
				var bytes = new StringBuilder(byteBufferSize);
				var chars = new StringBuilder(bytesPerLine);

				for (var j = 0; j < bytesPerLine; ++j) {
					var c = input.ReadByte();

					bytes.Append(c.ToString("X2"));

					// Split each hex sign
					bytes.Append(' ');
					// Some space after 7 blocks
					if (j == 7) {
						bytes.Append(' ');
					}

					// Build blob display
					if (c >= 32 && c < 128) {
						chars.Append((char)c);
					} else {
						// @TODO: Unicode display?
						chars.Append('.');
					}
				}

				// Number of line
				output.Write(byteIndex.ToString("X4"));
				// Some space before hex blocks
				if (preSpaceCount > 4) {
					output.Write(new string(' ', preSpaceCount - 4));
				}
				// Current data (per line)
				output.Write(bytes.ToString());
				// 2 spaces
				output.Write("  ");
				// Data as blob
				output.WriteLine(chars.ToString());
			}

			if (reminder != 0) {
				var bytes = new StringBuilder(byteBufferSize);
				var chars = new StringBuilder(reminder);

				for (var j = 0; j < bytesPerLine; ++j) {
					// Reached max amount of bits?
					if (j >= reminder) {
						bytes.Append("   ");
						continue;
					}

					var c = input.ReadByte();
					bytes.Append(c.ToString("X2"));

					// Split each hex sign
					bytes.Append(' ');
					// Some space after 7 blocks
					if (j == 7) {
						bytes.Append(' ');
					}

					// Build blob display
					if (c >= 32 && c < 128) {
						chars.Append((char)c);
					} else {
						chars.Append('.');
					}
				}

				// Number of line
				output.Write(byteIndex.ToString("X4"));
				// Some space before hex blocks
				if (preSpaceCount > 4) {
					output.Write(new string(' ', preSpaceCount - 4));
				}
				// Current data (per line)
				output.Write(bytes.ToString());
				// 2 spaces
				output.Write("  ");
				// Data as blob
				output.WriteLine(chars.ToString());
			}

		}

	}

}