using System;
using System.Collections.Generic;

namespace GodLesZ.Library.Network {

	public class SendQueue {
		private const int PendingCap = 96 * 1024;
		private static int mCoalesceBufferSize = 512;

		private Queue<PendingData> mPendingQueue;
		private PendingData mBuffered;

		public bool IsFlushReady {
			get { return (mPendingQueue.Count == 0 && mBuffered != null); }
		}

		public bool IsEmpty {
			get { return (mPendingQueue.Count == 0 && mBuffered == null); }
		}

		public static int CoalesceBufferSize {
			get { return mCoalesceBufferSize; }
			set { mCoalesceBufferSize = value; }
		}


		public class PendingData {
			private static readonly Stack<PendingData> mPoolStack = new Stack<PendingData>();

			public byte[] Buffer {
				get;
				private set;
			}

			public int Length {
				get;
				private set;
			}

			public int Available {
				get { return (Buffer.Length - Length); }
			}

			public bool IsFull {
				get { return (Length == Buffer.Length); }
			}


			public static PendingData Acquire() {
				lock (mPoolStack) {
					PendingData gram;

					if (mPoolStack.Count > 0)
						gram = mPoolStack.Pop();
					else
						gram = new PendingData();

					gram.Buffer = AcquireBuffer();
					gram.Length = 0;

					return gram;
				}
			}


			public int Write(byte[] buffer, int offset, int length) {
				int write = Math.Min(length, this.Available);

				System.Buffer.BlockCopy(buffer, offset, Buffer, Length, write);

				Length += write;

				return write;
			}

			public void Release() {
				lock (mPoolStack) {
					mPoolStack.Push(this);
				}
			}

		}



		public SendQueue() {
			mPendingQueue = new Queue<PendingData>();
		}



		public static byte[] AcquireBuffer() {
			return new byte[mCoalesceBufferSize];
		}

		public PendingData CheckFlushReady() {
			PendingData gram = null;
			if (mPendingQueue.Count == 0 && mBuffered != null) {
				gram = mBuffered;

				mPendingQueue.Enqueue(mBuffered);
				mBuffered = null;
			}

			return gram;
		}

		public PendingData Dequeue() {
			PendingData gram = null;

			if (mPendingQueue.Count > 0) {
				mPendingQueue.Dequeue().Release();

				if (mPendingQueue.Count > 0)
					gram = mPendingQueue.Peek();
			}

			return gram;
		}

		public PendingData Enqueue(byte[] buffer, int length) {
			return Enqueue(buffer, 0, length);
		}

		public PendingData Enqueue(byte[] buffer, int offset, int length) {
			if (buffer == null) {
				throw new ArgumentNullException("buffer");
			}
			if (!(offset >= 0 && offset < buffer.Length)) {
				throw new ArgumentOutOfRangeException("offset", offset, "Offset must be greater than or equal to zero and less than the size of the buffer.");
			}
			if (length < 0 || length > buffer.Length) {
				throw new ArgumentOutOfRangeException("length", length, "Length cannot be less than zero or greater than the size of the buffer.");
			}
			if ((buffer.Length - offset) < length) {
				throw new ArgumentException("Offset and length do not point to a valid segment within the buffer.");
			}

			int existingBytes = (mPendingQueue.Count * mCoalesceBufferSize) + (mBuffered == null ? 0 : mBuffered.Length);
			if ((existingBytes + length) > PendingCap)
				throw new CapacityExceededException();

			PendingData gram = null;
			while (length > 0) {
				if (mBuffered == null)
					mBuffered = PendingData.Acquire();

				int bytesWritten = mBuffered.Write(buffer, offset, length);

				offset += bytesWritten;
				length -= bytesWritten;

				if (mBuffered.IsFull) {
					if (mPendingQueue.Count == 0) {
						gram = mBuffered;
						System.Diagnostics.Debug.WriteLine("Gram.Enqueue(): _buffered IsFull, will send, _buffered.Length is " + mBuffered.Length);
					} else {
						System.Diagnostics.Debug.WriteLine("Gram.Enqueue(): _buffered IsFull but _pending.Count is " + mPendingQueue.Count);
					}

					mPendingQueue.Enqueue(mBuffered);
					mBuffered = null;
				}
			}

			if (gram == null)
				System.Diagnostics.Debug.WriteLine("Gram.Enqueue(): no Packet send, still Available: " + mBuffered.Available);

			return gram;
		}

		public void Clear() {
			if (mBuffered != null) {
				mBuffered.Release();
				mBuffered = null;
			}

			while (mPendingQueue.Count > 0)
				mPendingQueue.Dequeue().Release();

		}
	}

	public sealed class CapacityExceededException : Exception {
		public CapacityExceededException()
			: base("Too much data pending.") {
		}
	}

}
