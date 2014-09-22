using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace GodLesZ.Library.Network {

	public class SocketListener : IDisposable {
		private static readonly Socket[] mEmptySockets = new Socket[0];

		private readonly Queue<Socket> mAccepted;
		private readonly object mAcceptedSyncRoot;
		private readonly AsyncCallback mOnAccept;
		private Socket mListener;


		public SocketListener(IPEndPoint ipep, SocketConnector connector) {
			mAccepted = new Queue<Socket>();
			mAcceptedSyncRoot = ((ICollection)mAccepted).SyncRoot;
			mOnAccept = OnAccept;
			mListener = Bind(ipep, connector);
			if (mListener == null) {
				throw new Exception("Could not bind IP to Socket! Please check your Network Configuration!");
			}
		}

		public void Dispose() {
			Socket socket = Interlocked.Exchange(ref mListener, null);

			if (socket != null) {
				socket.Close();
			}
		}


		protected Socket Bind(IPEndPoint ipep, SocketConnector connector) {
			Socket s = SocketPool.AcquireSocket();

			try {
				s.LingerState.Enabled = false;
				s.ExclusiveAddressUse = false;

				s.Bind(ipep);
				s.Listen(8);

				if (ipep.Address.Equals(IPAddress.Any)) {
					try {
						CConsole.StatusLine(String.Format("start listen on {0}:{1}", IPAddress.Loopback, ipep.Port));

						IPHostEntry iphe = Dns.GetHostEntry(Dns.GetHostName());
						IPAddress[] ipList = iphe.AddressList;
						foreach (IPAddress ip in ipList) {
							CConsole.StatusLine(String.Format("# {0}:{1}", ip, ipep.Port));
						}
					} catch { }
				} else {
					if (ipep.Address.ToString() != connector.IP) {
						CConsole.StatusLine(String.Format("start listen on {0} -> {1}:{2}", connector.IP, ipep.Address, ipep.Port));
					} else {
						CConsole.StatusLine(String.Format("start listen on {0}:{1}", ipep.Address, ipep.Port));
					}
				}

				IAsyncResult res = s.BeginAccept(SocketPool.AcquireSocket(), 0, mOnAccept, s);
				return s;
			} catch (Exception e) {
				/* TODO
				 * throws more Exceptions like this
				 */
				var se = e as SocketException;
				if (se != null) {
					if (se.ErrorCode == 10048) {
						// WSAEADDRINUSE
						CConsole.ErrorLine(String.Format("Listener Failed: {0} -> {1}:{2} (In Use)", connector.IP, ipep.Address, ipep.Port));
					} else if (se.ErrorCode == 10049) {
						// WSAEADDRNOTAVAIL
						CConsole.ErrorLine(String.Format("Listener Failed: {0} -> {1}:{2} (Unavailable)", connector.IP, ipep.Address, ipep.Port));
					} else {
						CConsole.ErrorLine("Listener Exception:");
						CConsole.WriteLine(e);
					}
				}

				return null;
			}
		}

		private void OnAccept(IAsyncResult asyncResult) {
			var listener = (Socket)asyncResult.AsyncState;
			Socket accepted = null;

			try {
				accepted = listener.EndAccept(asyncResult);
			} catch (SocketException ex) {
				Debug.WriteLine(ex);
			} catch (ObjectDisposedException) {
				return;
			}

			if (accepted != null) {
				if (VerifySocket(accepted)) {
					Enqueue(accepted);
				} else {
					Release(accepted);
				}
			}

			try {
				listener.BeginAccept(SocketPool.AcquireSocket(), 0, mOnAccept, listener);
			} catch (SocketException ex) {
				Debug.WriteLine(ex);
			} catch (ObjectDisposedException) { }
		}

		protected virtual bool VerifySocket(Socket socket) {
			return true;
		}

		/// <summary>
		/// Should be overwritten to reset a <see cref="AutoResetEvent"/> or soemthing else
		/// </summary>
		/// <param name="socket"></param>
		protected virtual void Enqueue(Socket socket) {
			lock (mAcceptedSyncRoot) {
				mAccepted.Enqueue(socket);
			}
		}

		private static void Release(Socket socket) {
			try {
				socket.Shutdown(SocketShutdown.Both);
			} catch (SocketException ex) {
				Debug.WriteLine(ex);
			}

			try {
				socket.Close();

				SocketPool.ReleaseSocket(socket);
			} catch (SocketException ex) {
				Debug.WriteLine(ex);
			}
		}

		public Socket[] Slice() {
			Socket[] array;

			lock (mAcceptedSyncRoot) {
				if (mAccepted.Count == 0) {
					return mEmptySockets;
				}

				array = mAccepted.ToArray();
				mAccepted.Clear();
			}

			return array;
		}
	}
}