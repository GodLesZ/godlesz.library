using System;
using System.Net;
using System.IO;
using System.Text;

namespace GodLesZ.Library.Network.WebRequest {

    public class RequestResult {
        private readonly HttpWebResponse _baseResponse;
        private byte[] _responseData;
        private string _responseString;
        private readonly CookieCollection _cookies;

        public byte[] ResponseData {
            get { return _responseData; }
            set {
                _responseData = value;
                _responseString = WebUtil.ByteArrayToString(value);
            }
        }

        public string ResponseString {
            get { return _responseString; }
            set {
                _responseString = value;
                _responseData = WebUtil.StringToByteArray(value);
            }
        }

        public HttpWebResponse BaseResponse {
            get { return _baseResponse; }
        }

        public CookieCollection Cookies {
            get { return _cookies; }
        }


        public RequestResult(HttpWebResponse baseResp) {
            _baseResponse = baseResp;
            _cookies = BaseResponse.Cookies;

            using (var responseStream = BaseResponse.GetResponseStream()) {
                if (responseStream == null) {
                    throw new Exception("Failed to get response stream");
                }

                using (var readStream = new StreamReader(responseStream, Encoding.Default)) {
                    ResponseString = readStream.ReadToEnd();
                    switch (BaseResponse.ContentEncoding) {
                        case "gzip":
                            ResponseString = WebUtil.GZipDecompress(ResponseString);
                            break;
                        case "deflate":
                            throw new NotSupportedException("Deflate-compression income - not yet finished :/");
                    }
                }

            }
        }

    }

}
