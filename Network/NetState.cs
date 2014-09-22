using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using GodLesZ.Library.Network.Packets;

namespace GodLesZ.Library.Network {

	public interface IPacketEncoder {
		void EncodeOutgoingPacket(NetState to, ref byte[] buffer, ref int length);
		void DecodeIncomingPacket(NetState from, ref byte[] buffer, ref int length);
	}

	public delegate void NetStateCreatedCallback(NetState ns);

	public class NetState {
		[Flags]
		public enum AsyncNetState {
			Pending = 0x01,
			Paused = 0x02
		}

		protected const int BufferSize = 4096;

		protected static bool mPaused;
		protected static NetStateCreatedCallback mCreatedCallback;
		protected static readonly List<NetState> mInstances = new List<NetState>();
		protected static readonly Queue mDisposed = Queue.Synchronized(new Queue());
		protected static int mCoalesceSleep = -1;
		protected static readonly TimeSpan mCheckInterval = TimeSpan.FromSeconds(20);

		protected readonly IPAddress mAddress;
		protected readonly object mAsyncLock = new object();
		protected readonly DateTime mConnectedOn;
		protected readonly SocketConnector mMessagePump;
		protected readonly SendQueue mSendQueue;
		protected readonly string mToString;

		protected AsyncNetState mAsyncState;
		protected bool mBlockAllPackets;
		protected ByteQueue mBuffer;
		protected bool mDisposing;
		protected IPacketEncoder mEncoder;

		protected bool mHasPing;
		protected DateTime mNextCheckActivity;
		protected AsyncCallback mOnReceive, mOnSend;
		protected byte[] mRecvBuffer;
		protected bool mRunning;
		protected Socket mSocket;

		public NetState(Socket socket, SocketConnector messagePump) {
			mSocket = socket;
			mBuffer = new ByteQueue();
			Seeded = false;
			mRunning = false;
			mRecvBuffer = new byte[BufferSize];
			mMessagePump = messagePump;

			mSendQueue = new SendQueue();
			UpdateAcitivty();

			mInstances.Add(this);

			try {
				mAddress = ((IPEndPoint)mSocket.RemoteEndPoint).Address.Intern();
				mToString = mAddress.ToString();
			} catch (Exception ex) {
				throw;
			}

			mConnectedOn = DateTime.Now;

			if (mCreatedCallback != null) {
				mCreatedCallback(this);
			}
		}


		public static List<NetState> Instances {
			get { return mInstances; }
		}

		public static NetStateCreatedCallback CreatedCallback {
			get { return mCreatedCallback; }
			set { mCreatedCallback = value; }
		}

		public AsyncNetState AsyncState {
			get {
				lock (mAsyncLock)
					return mAsyncState;
			}
			set {
				lock (mAsyncLock)
					mAsyncState = value;
			}
		}

		public DateTime ConnectedOn {
			get { return mConnectedOn; }
		}

		public TimeSpan ConnectedFor {
			get { return (DateTime.Now - mConnectedOn); }
		}

		public IPAddress Address {
			get { return mAddress; }
		}

		public IPacketEncoder PacketEncoder {
			get { return mEncoder; }
			set { mEncoder = value; }
		}

		public bool SentFirstPacket { get; set; }

		public bool BlockAllPackets {
			get { return mBlockAllPackets; }
			set { mBlockAllPackets = value; }
		}

		public int Flags { get; set; }

		public int Sequence { get; set; }

		public static int CoalesceSleep {
			get { return mCoalesceSleep; }
			set { mCoalesceSleep = value; }
		}

		public bool Running {
			get { return mRunning; }
		}

		public bool Seeded { get; set; }

		public Socket Socket {
			get { return mSocket; }
		}

		public ByteQueue Buffer {
			get { return mBuffer; }
		}


		public virtual void Start() {
			mOnReceive = OnReceive;
			mOnSend = OnSend;

			mRunning = true;

			if (mSocket == null || mPaused) {
				return;
			}

			try {
				if ((AsyncState & (AsyncNetState.Pending | AsyncNetState.Paused)) == 0) {
					InternalBeginReceive();
				}
			} catch (Exception ex) {
				Dispose(false);
				throw;
			}
		}


		public static void Pause() {
			mPaused = true;

			foreach (NetState ns in mInstances) {
				ns.AsyncState |= AsyncNetState.Paused;
			}
		}

		public static void Resume() {
			mPaused = false;

			foreach (NetState ns in mInstances) {
				if (ns.mSocket == null) {
					continue;
				}

				ns.AsyncState &= ~AsyncNetState.Paused;
				try {
					if ((ns.AsyncState & AsyncNetState.Pending) == 0) {
						ns.InternalBeginReceive();
					}
				} catch (Exception ex) {
					ns.Dispose(false);
					throw;
				}
			}
		}

		public static void FlushAll() {
			foreach (NetState ns in mInstances) {
				ns.Flush();
			}
		}

		public static void Initialize() {
			Timer.Timer.DelayCall(mCheckInterval, mCheckInterval, CheckAllAlive);
		}

		public static void CheckAllAlive() {
			//CConsole.StatusLine("CheckAllAlive() for {0} Instances", mInstances.Count);
			try {
				foreach (NetState t in mInstances) {
					t.CheckAlive();
				}
			} catch (Exception ex) {
				Debug.WriteLine(ex);
			}
		}

		public static void ProcessDisposedQueue() {
			int breakout = 0;
			while (breakout < 200 && mDisposed.Count > 0) {
				++breakout;

				var ns = (NetState)mDisposed.Dequeue();

				mInstances.Remove(ns);

				CConsole.ErrorLine("{0}: Disconnected. [{1} Online]", ns, mInstances.Count);
			}
		}

		public virtual void Send(Packet p) {
			if (mSocket == null || mBlockAllPackets) {
				p.OnSend();
				return;
			}

			PacketSendProfile prof = PacketSendProfile.Acquire(p.GetType());

			int length;
			byte[] buffer = p.Compile(out length);
			if (buffer == null) {
				CConsole.ErrorLine("{0}: null buffer send, disconnecting...", this);

				Dispose();
				return;
			}

			if (buffer.Length <= 0 || length <= 0) {
				p.OnSend();
				return;
			}

			if (prof != null) {
				prof.Start();
			}

			if (mEncoder != null) {
				mEncoder.EncodeOutgoingPacket(this, ref buffer, ref length);
			}

			try {
				Int16 pID = BitConverter.ToInt16(buffer, 0);
				CConsole.DebugLine("{0}: sending Packet 0x{1:X4}", this, pID);
				mSocket.BeginSend(buffer, 0, length, SocketFlags.None, mOnSend, mSocket);
			} catch (Exception ex) {
				Dispose(false);
				throw;
			}

			p.OnSend();

			if (prof != null) {
				prof.Finish(length);
			}
		}

		public bool Flush() {
			if (mSocket == null || !mSendQueue.IsFlushReady) {
				return false;
			}

			SendQueue.PendingData gram;
			lock (mSendQueue) {
				gram = mSendQueue.CheckFlushReady();
			}

			if (gram != null) {
				try {
					mSocket.BeginSend(gram.Buffer, 0, gram.Length, SocketFlags.None, mOnSend, mSocket);
					return true;
				} catch (Exception ex) {
					Dispose(false);
					throw;
				}
			}

			return false;
		}

		public bool CheckAlive() {
			if (mSocket == null) {
				return false;
			}

			if (IsActive()) {
				Flush();
				return true;
			}

			CConsole.InfoLine("{0} = Disconnecting due to inactivity...", this);

			Dispose();
			return false;
		}


		protected void InternalBeginReceive() {
			AsyncState |= AsyncNetState.Pending;
			if (mSocket != null) {
				mSocket.BeginReceive(mRecvBuffer, 0, mRecvBuffer.Length, SocketFlags.None, mOnReceive, mSocket);
			}
		}

		protected void OnSend(IAsyncResult asyncResult) {
			var s = (Socket)asyncResult.AsyncState;

			try {
				int bytes = s.EndSend(asyncResult);

				if (bytes <= 0) {
					Dispose(false);
					return;
				}

				UpdateAcitivty();

				if (mCoalesceSleep >= 0) {
					Thread.Sleep(mCoalesceSleep);
				}
			} catch (Exception ex) {
				Dispose(false);
				throw;
			}
		}

		protected void OnReceive(IAsyncResult asyncResult) {
			var s = (Socket)asyncResult.AsyncState;

			try {
				int byteCount = s.EndReceive(asyncResult);
				if (byteCount > 0) {
					UpdateAcitivty();

					byte[] buffer = mRecvBuffer;
					if (mEncoder != null) {
						mEncoder.DecodeIncomingPacket(this, ref buffer, ref byteCount);
					}

					lock (mBuffer)
						mBuffer.Enqueue(buffer, 0, byteCount);

					mMessagePump.OnReceive(this);

					lock (mAsyncLock) {
						mAsyncState &= ~AsyncNetState.Pending;

						if ((mAsyncState & AsyncNetState.Paused) == 0) {
							try {
								InternalBeginReceive();
							} catch (Exception ex) {
								Dispose(false);
								throw;
							}
						}
					}
				} else {
					Dispose(false);
				}
			} catch {
				Dispose(false);
			}
		}

		public void Dispose() {
			Dispose(true);
		}


		public virtual void Dispose(bool flush) {
			if (mSocket == null || mDisposing) {
				return;
			}

			mDisposing = true;

			//##PACKET## Logout
			Thread.Sleep(100);

			if (flush) {
				Flush();
			}

			try {
				mSocket.Shutdown(SocketShutdown.Both);
			} catch (SocketException ex) {
				Debug.WriteLine(ex);
			}

			try {
				mSocket.Close();
				SocketPool.ReleaseSocket(mSocket);
			} catch (SocketException ex) {
				Debug.WriteLine(ex);
			}

			mSocket = null;

			mBuffer = null;
			mRecvBuffer = null;
			mOnReceive = null;
			mOnSend = null;
			mRunning = false;

			mDisposed.Enqueue(this);

			if (!mSendQueue.IsEmpty) {
				lock (mSendQueue)
					mSendQueue.Clear();
			}
		}


		public override string ToString() {
			string ret = String.Format("[{0}]", mToString);

			return ret;
		}


		public void UpdateAcitivty() {
			UpdateAcitivty(true);
		}

		public void UpdateAcitivty(bool updatePing) {
			mNextCheckActivity = DateTime.Now + mCheckInterval;
			if (updatePing) {
				mHasPing = false;
			}
		}

		public bool IsActive() {
			if (DateTime.Now >= mNextCheckActivity) {
				return true;
			}

			// ever pinged?
			if (mHasPing == false) {
				mHasPing = true;
				//##PACKET## PingPong?

				// update last time without Ping update
				UpdateAcitivty(false);
				return true;
			}

			return false;
		}
	}
}