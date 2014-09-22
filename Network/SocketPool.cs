using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;

namespace GodLesZ.Library.Network {
	
	public class SocketPool {
		private static int mInitialCapacity = 32;
		private static Queue<Socket> mFreeSockets;

		private static AddressFamily mDefaultAddressFamily;
		private static SocketType mDefaultSocketType;
		private static ProtocolType mDefaultProtocolType;

		static SocketPool() {
			Misses = 0;
		}

		public static bool Created { get; private set; }

		public static int InitialCapacity {
			get { return mInitialCapacity; }
			set {
				if (Created) {
					return;
				}
				mInitialCapacity = value;
			}
		}

		public static int Misses { get; private set; }


		public static void Create(AddressFamily addressFamily = AddressFamily.InterNetwork, SocketType socketType = SocketType.Stream, ProtocolType protocolType = ProtocolType.Tcp) {
			if (Created) {
				return;
			}

			mDefaultAddressFamily = addressFamily;
			mDefaultSocketType = socketType;
			mDefaultProtocolType = protocolType;

			CConsole.StatusLine("Creating {0} Sockets...", mInitialCapacity);

			mFreeSockets = new Queue<Socket>(mInitialCapacity);
			LoadPool(addressFamily, socketType, protocolType);

			Created = true;
		}

		protected static void LoadPool(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType) {
			for (int i = 0; i < mInitialCapacity; ++i) {
				mFreeSockets.Enqueue(new Socket(addressFamily, socketType, protocolType));
			}
		}

		public static void Destroy() {
			if (Created == false || mFreeSockets == null) {
				return;
			}

			CConsole.StatusLine("Closing {0} Sockets...", mFreeSockets.Count);

			while (mFreeSockets.Count > 0) {
				mFreeSockets.Dequeue().Close();
			}

			mFreeSockets = null;
		}

		public static Socket AcquireSocket() {
			lock (mFreeSockets) {
				if (mFreeSockets.Count > 0) {
					return mFreeSockets.Dequeue();
				}

				++Misses;

				for (int i = 0; i < mInitialCapacity; ++i) {
					mFreeSockets.Enqueue(new Socket(mDefaultAddressFamily, mDefaultSocketType, mDefaultProtocolType));
				}

				return mFreeSockets.Dequeue();
			}
		}

		public static void ReleaseSocket(Socket s) {
			if (s == null) {
				return;
			}

			try {
				s.Close();
				s = null;
			} catch (Exception ex) {
				Debug.WriteLine(ex);
			}
		}

	}

}