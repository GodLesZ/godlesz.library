using System.Net;
using System.IO;
using System.Text;

namespace GodLesZ.Library.Network.WebRequest {

	public class PostResult {
		private HttpWebResponse mBaseResponse;
		private byte[] mResponseData;
		private string mResponseString;
		private CookieCollection mCookies;

		public byte[] ResponseData {
			get { return mResponseData; }
			set {
				mResponseData = value;
				mResponseString = WebUtil.ByteArrayToString(value);
			}
		}

		public string ResponseString {
			get { return mResponseString; }
			set {
				mResponseString = value;
				mResponseData = WebUtil.StringToByteArray(value);
			}
		}

		public HttpWebResponse BaseResponse {
			get { return mBaseResponse; }
		}

		public CookieCollection Cookies {
			get { return mCookies; }
		}


		public PostResult(HttpWebResponse baseResp) {
			mBaseResponse = baseResp;
			mCookies = BaseResponse.Cookies;

			using(Stream responseStream = BaseResponse.GetResponseStream()) {
				using(StreamReader readStream = new StreamReader(responseStream, Encoding.Default)) {
					ResponseString = readStream.ReadToEnd();
					if(BaseResponse.ContentEncoding == "gzip") {
						ResponseString = WebUtil.GZipDecompress(ResponseString);
					} else if(BaseResponse.ContentEncoding == "deflate") {
						throw new System.NotSupportedException("Deflate-compression income - not yet finished :/");
					}
				}

			}
		}

	}

}
