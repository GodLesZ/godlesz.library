using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace GodLesZ.Library.Network {

	public class SocketConnector {
		private readonly Queue<NetState> mThrottled;

		private Queue<NetState> mQueue;
		private Queue<NetState> mWorkingQueue;

		public string IP {
			get;
			private set;
		}

		public int Port {
			get;
			private set;
		}

		public SocketListener Listener {
			get;
			protected set;
		}


		public SocketConnector(string ip, int port) {
			IP = ip;
			Port = port;
			IPHostEntry iphe;
			try {
				iphe = Dns.GetHostEntry(IP);
				if (iphe.AddressList.Length < 1) {
					throw new Exception("Unable to fetch IP Address from: " + IP + "::" + Port);
				}
			} catch (Exception e) {
				throw;
			}

			mQueue = new Queue<NetState>();
			mWorkingQueue = new Queue<NetState>();
			mThrottled = new Queue<NetState>();

			StartListen(iphe, port);
		}


		protected virtual void StartListen(IPHostEntry iphe, int port) {
			var endPoint = new IPEndPoint(iphe.AddressList[0], Port);
			Listener = new SocketListener(endPoint, this);
		}

		protected virtual void CheckListener() {
			var accepted = Listener.Slice();

			foreach (var ns in accepted.Select(socket => new NetState(socket, this))) {
				ns.Start();

				if (ns.Running) {
					CConsole.StatusLine("{0}: Connected. [{1} Online]", ns, NetState.Instances.Count);
				}
			}
		}

		/// <summary>
		/// Should be overwritten to reset a <see cref="AutoResetEvent"/> or something else
		/// </summary>
		/// <param name="ns"></param>
		public virtual void OnReceive(NetState ns) {
			lock (this)
				mQueue.Enqueue(ns);
		}

		public virtual void Slice() {
			CheckListener();

			lock (this) {
				Queue<NetState> temp = mWorkingQueue;
				mWorkingQueue = mQueue;
				mQueue = temp;
			}

			while (mWorkingQueue.Count > 0) {
				NetState ns = mWorkingQueue.Dequeue();

				if (ns.Running) {
					HandleReceive(ns);
				}
			}

			lock (this) {
				while (mThrottled.Count > 0) {
					mQueue.Enqueue(mThrottled.Dequeue());
				}
			}
		}

		public virtual bool HandleReceive(NetState ns) {
			ByteQueue buffer = ns.Buffer;
			if (buffer == null || buffer.Length <= 0) {
				return false;
			}

			return true;
		}
	}
}