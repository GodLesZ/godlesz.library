using System.Collections.Generic;

namespace GodLesZ.Library.Network {

	public class BufferPool {
		private static readonly List<BufferPool> mPools = new List<BufferPool>();

		private readonly string mName;
		private readonly int mInitialCapacity;
		private readonly int mBufferSize;
		private int mMisses;
		private readonly Queue<byte[]> mFreeBuffers;

		/// <summary>
		/// Gets the internal BufferPool list
		/// </summary>
		public static List<BufferPool> Pools {
			get { return mPools; }
		}


		public BufferPool(string name, int initialCapacity, int bufferSize) {
			mName = name;

			mInitialCapacity = initialCapacity;
			mBufferSize = bufferSize;

			mFreeBuffers = new Queue<byte[]>(initialCapacity);

			for (int i = 0; i < initialCapacity; ++i)
				mFreeBuffers.Enqueue(new byte[bufferSize]);

			lock (mPools)
				mPools.Add(this);
		}



		public void GetInfo(out string name, out int freeCount, out int initialCapacity, out int currentCapacity, out int bufferSize, out int misses) {
			lock (this) {
				name = mName;
				freeCount = mFreeBuffers.Count;
				initialCapacity = mInitialCapacity;
				currentCapacity = mInitialCapacity * (1 + mMisses);
				bufferSize = mBufferSize;
				misses = mMisses;
			}
		}

		/// <summary>
		/// Create a new byte[] Buffer
		/// </summary>
		/// <returns></returns>
		public byte[] AcquireBuffer() {
			lock (this) {
				if (mFreeBuffers.Count > 0)
					return mFreeBuffers.Dequeue();

				++mMisses;
				for (int i = 0; i < mInitialCapacity; ++i)
					mFreeBuffers.Enqueue(new byte[mBufferSize]);

				return mFreeBuffers.Dequeue();
			}
		}

		/// <summary>
		/// Release the byte[] Data from the Buffer
		/// </summary>
		/// <param name="buffer"></param>
		public void ReleaseBuffer(byte[] buffer) {
			if (buffer == null)
				return;

			lock (this)
				mFreeBuffers.Enqueue(buffer);
		}

		/// <summary>
		/// Release this BufferPool instance from internal Pool
		/// </summary>
		public void Free() {
			lock (mPools)
				mPools.Remove(this);
		}

	}


}
