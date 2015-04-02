using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using GodLesZ.Library.Network.WebRequest;

namespace GodLesZ.Library.Network {

	public class WebUtil {
		public static CookieCollection Cookies = new CookieCollection();

		public static RequestResult GetPage(string Uri, string UriReferer, ERequestType Method, List<string> Params) {
			RequestHelper mClient;
			mClient = new RequestHelper(Uri, UriReferer);
			mClient.Type = Method;
			if (Cookies != null) {
				mClient.Cookies.Add(Cookies);
			}
			for (int i = 0; i < Params.Count; i += 2) {
				mClient.RequestUrlValues.Add(Params[i], Params[i + 1]);
			}

			return mClient.Request();
		}

		public static RequestResult GetPage(string Uri, string UriReferer, string Params) {
			List<string> newParams = new List<string>();
			Params = Params.Trim();
			if (Params.Length > 0) {
				foreach (string splitPair in Params.Split('&')) {
					newParams.AddRange(splitPair.Split('='));
				}
			}

			return GetPage(Uri, UriReferer, ERequestType.Get, newParams);
		}

		public static RequestResult GetPage(string Uri, string UriReferer, ERequestType Method, string Params) {
			List<string> newParams = new List<string>();
			Params = Params.Trim();
			if (Params.Length > 0) {
				foreach (string splitPair in Params.Split('&')) {
					newParams.AddRange(splitPair.Split('='));
				}
			}

			return GetPage(Uri, UriReferer, Method, newParams);
		}


		public static byte[] StringToByteArray(string str) {
			return Encoding.Default.GetBytes(str);
		}

		public static string ByteArrayToString(byte[] arr) {
			return Encoding.Default.GetString(arr);
		}


		public static byte[] ReadAllBytesFromStream(Stream stream) {
			int totalCount = 0;
			int bytesRead = 0;
			byte[] buffer = new byte[1024];

			using (MemoryStream ms = new MemoryStream()) {
				while (true) {
					if ((bytesRead = stream.Read(buffer, 0, buffer.Length)) == 0) {
						break;
					}
					ms.Write(buffer, 0, bytesRead);
					totalCount += bytesRead;
				}

				ms.Position = 0;
				buffer = new byte[(int)ms.Length];
				ms.Read(buffer, 0, buffer.Length);
			}
			return buffer;
		}

		public static string GZipDecompress(string StringData) {
			byte[] data = StringToByteArray(StringData);
			using (MemoryStream ms = new MemoryStream()) {
				ms.Write(data, 0, data.Length);
				ms.Position = 0;
				GZipStream zip = new GZipStream(ms, CompressionMode.Decompress);

				byte[] decompressedBuffer = ReadAllBytesFromStream(zip);

				StringData = ByteArrayToString(decompressedBuffer).TrimEnd(new char[] { '\0' });
				decompressedBuffer = null;
			}

			return StringData;
		}

	}

}
