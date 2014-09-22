using System;
using System.Text;
using System.IO;

namespace GodLesZ.Library.Network.Packets {

	public class PacketReader : IDisposable {
		protected byte[] mData;
		protected int mSize;
		protected int mIndex;

		public byte[] Buf {
			get { return mData; }
		}

		public int Size {
			get { return mSize; }
		}

		public short PacketID {
			get;
			protected set;
		}

		public short PacketLength {
			get;
			protected set;
		}

		public int CurrentPosition {
			get { return mIndex; }
			protected set { mIndex = value; }
		}


        public PacketReader(byte[] data)
            : this(data, data.Length, 0, false, false) {
        }

        public PacketReader(byte[] data, int size, int startIndex)
            : this(data, size, startIndex, false, false) {
        }

		public PacketReader(byte[] data, int size, int startIndex, bool readPacketId)
			: this(data, size, startIndex, readPacketId, false) {
		}

		public PacketReader(byte[] data, int size, int startIndex, bool readPacketId, bool readPacketLength) {
			mData = data;
			mSize = size;
			mIndex = startIndex;

			if (readPacketId == true) {
				PacketID = ReadInt16();
			}
			if (readPacketLength == true) {
				PacketLength = ReadInt16();
			}
		}


		public int Seek(int offset, SeekOrigin origin) {
			switch (origin) {
				case SeekOrigin.Begin:
					mIndex = offset;
					break;
				case SeekOrigin.Current:
					mIndex += offset;
					break;
				case SeekOrigin.End:
					mIndex = mSize - offset;
					break;
			}

			return mIndex;
		}


		public bool IsSafeChar(int c) {
			// @TODO: rly need this range? [replace <int c> with <byte c> if not]
			return (c >= 32 && c < 65534);
		}



		public int ReadInt32() {
			if ((mIndex + 4) > mSize)
				return 0;

			int i = BitConverter.ToInt32(mData, mIndex);
			mIndex += 4;
			return i;
		}

		public short ReadInt16() {
			if ((mIndex + 2) > mSize)
				return 0;

			short i = BitConverter.ToInt16(mData, mIndex);
			mIndex += 2;
			return i;
		}

        public byte ReadByte() {
            if ((mIndex + 1) > mSize)
                return 0;

            byte b = mData[mIndex];
            mIndex++;
            return b;
        }

        public byte[] ReadBytes(int len) {
            if ((mIndex + len) > mSize)
                return new byte[]{};

            var buf = new Byte[len];
            Buffer.BlockCopy(mData, mIndex, buf, 0, buf.Length);
            mIndex += buf.Length;
            return buf;
        }

		public uint ReadUInt32() {
			if ((mIndex + 4) > mSize)
				return 0;

			uint i = BitConverter.ToUInt32(mData, mIndex);
			mIndex += 4;
			return i;
		}

		public ushort ReadUInt16() {
			if ((mIndex + 2) > mSize)
				return 0;

			ushort i = BitConverter.ToUInt16(mData, mIndex);
			mIndex += 2;
			return i;
		}

		public sbyte ReadSByte() {
			return (sbyte)ReadByte();
		}

		public bool ReadBoolean() {
			return (ReadByte() > 0);
		}



		public string ReadString() {
			// find leading \0 or end of stream
			int i = mIndex;
			while (i < mSize && mData[i++] > 0)
				;

			int stringLength = (i - mIndex);
			if (stringLength == 0) {
				return "";
			}
			string s = Encoding.Default.GetString(mData, mIndex, stringLength).TrimEnd('\0');
			mIndex += stringLength;
			return s;
		}

		public string ReadString(int fixedLength) {
			int stringLength = Math.Min(fixedLength, (mSize - mIndex));
			if (stringLength == 0) {
				return "";
			}

			string s = Encoding.Default.GetString(mData, mIndex, stringLength);
			int trailingZero = s.IndexOf('\0');
			if (trailingZero != -1) {
				if (trailingZero == 0) {
					return "";
				}
				s = s.Substring(0, trailingZero);
			}
			mIndex += stringLength;
			return s;
		}


		/// <summary>
		/// Führt anwendungsspezifische Aufgaben durch, die mit der Freigabe, der Zurückgabe oder dem Zurücksetzen von nicht verwalteten Ressourcen zusammenhängen.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose() {
			mData = null;
		}

	}

}
