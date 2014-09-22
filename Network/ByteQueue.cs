using System;
using System.IO;
using GodLesZ.Library.Network.Packets;

namespace GodLesZ.Library.Network {

	public class ByteQueue {
		private int mHead;
		private int mTail;
		private int mSize;
		private byte[] mBuffer;

		public int Length {
			get { return mSize; }
		}

		public byte[] ByteBuffer {
			get { return mBuffer; }
		}



		public ByteQueue() {
			mBuffer = new byte[2048];
		}


		public void Clear() {
			mHead = 0;
			mTail = 0;
			mSize = 0;
		}

		private void SetCapacity(int capacity) {
			var newBuffer = new byte[capacity];

			if (mSize > 0) {
				if (mHead < mTail) {
					Buffer.BlockCopy(mBuffer, mHead, newBuffer, 0, mSize);
				} else {
					Buffer.BlockCopy(mBuffer, mHead, newBuffer, 0, mBuffer.Length - mHead);
					Buffer.BlockCopy(mBuffer, 0, newBuffer, mBuffer.Length - mHead, mTail);
				}
			}

			mHead = 0;
			mTail = mSize;
			mBuffer = newBuffer;
		}

		public short GetPacketID() {
			if (mSize >= 2) {
				return BitConverter.ToInt16(mBuffer, mHead);
			}

			return -1;
		}

		public int GetPacketLength() {
			if (mSize >= 3) {
				return BitConverter.ToInt16(mBuffer, mHead + 2);
			}

			return -1;
		}

		public PacketReader GetPacketReader() {
			var reader = new PacketReader(mBuffer, mSize, 0);
			if (mSize >= 2) {
				reader.Seek(2, SeekOrigin.Begin);
			}
			return reader;
		}

		public int Dequeue(int size) {
			var buffer = new byte[size];
			return Dequeue(buffer, 0, size);
		}

		public int Dequeue(byte[] buffer, int offset, int size) {
			if (size > mSize)
				size = mSize;

			if (size == 0)
				return 0;

			if (mHead < mTail) {
				Buffer.BlockCopy(mBuffer, mHead, buffer, offset, size);
			} else {
				int rightLength = (mBuffer.Length - mHead);

				if (rightLength >= size) {
					Buffer.BlockCopy(mBuffer, mHead, buffer, offset, size);
				} else {
					Buffer.BlockCopy(mBuffer, mHead, buffer, offset, rightLength);
					Buffer.BlockCopy(mBuffer, 0, buffer, offset + rightLength, size - rightLength);
				}
			}

			mHead = (mHead + size) % mBuffer.Length;
			mSize -= size;

			if (mSize == 0) {
				Clear();
			}

			return size;
		}

		public void Enqueue(byte[] buffer, int offset, int size) {
			if ((mSize + size) > mBuffer.Length)
				SetCapacity((mSize + size + 2047) & ~2047);

			if (mHead < mTail) {
				int rightLength = (mBuffer.Length - mTail);

				if (rightLength >= size) {
					Buffer.BlockCopy(buffer, offset, mBuffer, mTail, size);
				} else {
					Buffer.BlockCopy(buffer, offset, mBuffer, mTail, rightLength);
					Buffer.BlockCopy(buffer, offset + rightLength, mBuffer, 0, size - rightLength);
				}
			} else {
				Buffer.BlockCopy(buffer, offset, mBuffer, mTail, size);
			}

			mTail = (mTail + size) % mBuffer.Length;
			mSize += size;
		}

	}

}
