using System;
using System.Net;
using System.Net.Sockets;

namespace GodLesZ.Library {

	public static class SocketExtensions {

		public static void Connect(this Socket socket, string host, int port, TimeSpan timeout) {
			AsyncConnect(socket, (s, a, o) => s.BeginConnect(host, port, a, o), timeout);
		}

		public static void Connect(this Socket socket, IPAddress[] addresses, int port, TimeSpan timeout) {
			AsyncConnect(socket, (s, a, o) => s.BeginConnect(addresses, port, a, o), timeout);
		}

		public static void Connect(this Socket socket, IPEndPoint ipe, TimeSpan timeout) {
			AsyncConnect(socket, (s, a, o) => s.BeginConnect(ipe, a, o), timeout);
		}

		private static void AsyncConnect(Socket socket, Func<Socket, AsyncCallback, object, IAsyncResult> connect, TimeSpan timeout) {
			var asyncResult = connect(socket, null, null);
			if (asyncResult.AsyncWaitHandle.WaitOne(timeout)) {
				return;
			}

			try {
				socket.EndConnect(asyncResult);
			} catch (SocketException) {

			} catch (ObjectDisposedException) {

			}
		}

	}

}