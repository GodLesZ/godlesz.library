using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using GodLesZ.Library.Json.Utilities;

namespace GodLesZ.Library.Network.WebRequest {

    public class RequestHelper {


        /// <summary>
        /// Gets or sets the url to submit the request to.
        /// </summary>
        public string Url {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the url to submit the request to.
        /// </summary>
        public string UrlReferer {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the name value collection of items to request.
        /// </summary>
        public NameValueCollection RequestUrlValues {
            get;
            protected set;
        }
        /// <summary>
        /// Gets or sets the type of action to perform against the url.
        /// </summary>
        public ERequestType Type {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the cookies used for the requests.
        /// </summary>
        public CookieCollection Cookies {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets a Proxy.
        /// </summary>
        public WebProxy Proxy {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a timeout for the Request
        /// Default: 10sec
        /// </summary>
        public int Timeout {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a flag for auto fetching saved cookies for the url.
        /// Default: false
        /// <para>
        /// Note: This dos not affaect the response cookies!
        /// </para>
        /// </summary>
        public bool IgnoreCookies {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a flag to allow auto redirecting from the response.
        /// Default: true
        /// </summary>
        public bool AutoRedirect {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a list of addional headers used for every request.
        /// </summary>
        public Dictionary<string, string> Headers {
            get;
            protected set;
        }


        /// <summary>
        /// Default constructor.
        /// </summary>
        public RequestHelper() {
            AutoRedirect = true;
            Cookies = null;
            IgnoreCookies = false;
            Type = ERequestType.Get;
            RequestUrlValues = new NameValueCollection();
            Headers = new Dictionary<string, string> {
                {"User-Agent", "Mozilla/5.0 (Windows NT 5.2; WOW64; rv:5.0) Gecko/20100101 Firefox/5.0"},
                {"Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8"},
                {"Accept-Language", "de-de,de;q=0.8,en-us;q=0.5,en;q=0.3"},
                {"Accept-Encoding", "gzip, deflate"},
                {"Accept-Charset", "ISO-8859-1,utf-8;q=0.7,*;q=0.7"}
            };
        }

        /// <summary>
        /// Constructor that accepts a url as a parameter
        /// </summary>
        /// <param name="url">The url where the request will be submitted to.</param>
        /// <param name="urlReferer"></param>
        public RequestHelper(string url, string urlReferer)
            : this() {
            Url = url;
            UrlReferer = urlReferer;
        }

        /// <summary>
        /// Constructor allowing the setting of the url and items to request.
        /// </summary>
        /// <param name="url">The url for the request</param>
        /// <param name="urlReferer">The url to reffer to</param>
        /// <param name="values">The key,value pairs used as url parameters</param>
        public RequestHelper(string url, string urlReferer, NameValueCollection values)
            : this(url, urlReferer) {
            RequestUrlValues = values;
        }


        public void MergeHeaders(IEnumerable<KeyValuePair<string, string>> additionalHeaders) {
            foreach (var pair in additionalHeaders) {
                if (Headers.ContainsKey(pair.Key)) {
                    Headers[pair.Key] = pair.Value;
                } else {
                    Headers.Add(pair.Key, pair.Value);
                }
            }
        }


        /// <summary>
        /// Starts a request using the supplied data to specified url.
        /// </summary>
        /// <returns>a string containing the result of the request.</returns>
        public RequestResult Request() {
            var parameters = new StringBuilder();
            for (var i = 0; i < RequestUrlValues.Count; i++) {
                EncodeAndAddItem(ref parameters, RequestUrlValues.GetKey(i), RequestUrlValues[i]);
            }

            return RequestWithData(Url, parameters.ToString());
        }
        /// <summary>
        /// Starts a request using the supplied data to specified url.
        /// </summary>
        /// <param name="url">The url to request to.</param>
        /// <returns>a string containing the result of the request.</returns>
        public RequestResult Request(string url) {
            Url = url;
            return Request();
        }
        /// <summary>
        /// Starts a request using the supplied data to specified url.
        /// </summary>
        /// <param name="url">The url to request to.</param>
        /// <param name="values">The values to request.</param>
        /// <returns>a string containing the result of the request.</returns>
        public RequestResult Request(string url, NameValueCollection values) {
            RequestUrlValues = values;
            return Request(url);
        }



        /// <summary>
        /// Starts a requests using the supplied data. Note that this assumes that you have already url encoded the data.
        /// </summary>
        /// <param name="requestData">The data to request.</param>
        /// <returns>Returns the result of the request.</returns>
        public RequestResult RequestWithData(string requestData) {
            return RequestWithData(Url, requestData);
        }

        /// <summary>
        /// Starts a requests to a specified url using the supplied data. Note that this assumes that you have already url encoded the data.
        /// </summary>
        /// <param name="url">the url to request to.</param>
        /// <param name="requestData">The data to request.</param>
        /// <returns>Returns the result of the request.</returns>
        public RequestResult RequestWithData(string url, string requestData) {
            RequestResult result;

            if (Type == ERequestType.Get & requestData.Length > 0) {
                if (url.Contains("?")) {
                    url += "&" + requestData;
                } else {
                    url += "?" + requestData;
                }
            }

            var uri = new Uri(url);
            var request = (HttpWebRequest)System.Net.WebRequest.Create(uri);
            request.Credentials = CredentialCache.DefaultCredentials;
            request.CookieContainer = new CookieContainer();
            if (IgnoreCookies == false) {
                request.CookieContainer.GetCookies(uri);
            }
            if (Cookies != null) {
                request.CookieContainer.Add(Cookies);
            }
            if (Proxy != null) {
                request.Proxy = Proxy;
            }
            if (Timeout > 0) {
                request.Timeout = Timeout;
            }

            if (url.StartsWith("https://")) {
                ServicePointManager.ServerCertificateValidationCallback = AcceptAllCertifications;
            }

            foreach (var pair in Headers) {
                var keyLower = pair.Key.ToLower();
                if (keyLower == "user-agent") {
                    request.UserAgent = pair.Value;
                } else if (keyLower == "accept") {
                    request.Accept = pair.Value;
                } else if (keyLower == "content-type") {
                    request.ContentType = pair.Value;
                } else if (keyLower == "connection") {
                    request.Connection = pair.Value;
                } else {
                    request.Headers.Add(pair.Key, pair.Value);
                }
            }
            request.Referer = UrlReferer;
            request.Method = Type.ToString().ToUpper();
            request.AllowAutoRedirect = AutoRedirect;

            ServicePointManager.Expect100Continue = false;

            if (Type == ERequestType.Post) {
                if (Headers.Keys.Any(k => k.ToLower() == "content-type") == false) {
                    request.ContentType = "application/x-www-form-urlencoded";
                }
                if (requestData.Length > 0) {
                    var encoding = new UTF8Encoding();
                    byte[] requestBuf = encoding.GetBytes(requestData);

                    request.ContentLength = requestBuf.Length;

                    try {
                        var writeStream = request.GetRequestStream();

                        writeStream.Write(requestBuf, 0, requestBuf.Length);
                        writeStream.Close();
                    } catch { }
                } else {
                    request.ContentLength = 0;
                }
            }

            try {
                using (var response = (HttpWebResponse)request.GetResponse()) {
                    result = new RequestResult(response);
                }
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e);
                result = null;
            }

            return result;
        }


        /// <summary>
        /// Encodes an item and ads it to the string.
        /// </summary>
        /// <param name="baseRequest">The previously encoded data.</param>
        /// <param name="key"></param>
        /// <param name="dataItem">The data to encode.</param>
        /// <returns>A string containing the old data and the previously encoded data.</returns>
        protected void EncodeAndAddItem(ref StringBuilder baseRequest, string key, string dataItem) {
            if (baseRequest == null) {
                baseRequest = new StringBuilder();
            }
            if (baseRequest.Length != 0) {
                baseRequest.Append("&");
            }

            baseRequest.Append(HttpUtility.UrlEncode(key));
            baseRequest.Append("=");
            baseRequest.Append(HttpUtility.UrlEncode(dataItem));
        }

        /// <summary>
        /// Callback for fetching
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certification"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        protected virtual bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors) {
            return true;
        }

    }
}
