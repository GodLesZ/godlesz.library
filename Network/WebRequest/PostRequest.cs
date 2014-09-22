using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace GodLesZ.Library.Network.WebRequest {

	public class WebInfo {
		public string Useragent = "Mozilla/5.0 (Windows NT 5.2; WOW64; rv:5.0) Gecko/20100101 Firefox/5.0";
		public string RequestAccept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
		public string AcceptLanguage = "de-de,de;q=0.8,en-us;q=0.5,en;q=0.3";
		public string AcceptEncoding = "gzip, deflate";
		public string AcceptCharset = "ISO-8859-1,utf-8;q=0.7,*;q=0.7";
		public int KeepAlive = 300;
	}

	public class PostRequest {

		public enum PostTypeEnum {
			Get,
			Post
		}

		public static WebInfo WebInfo = new WebInfo();


		/// <summary>
		/// Gets or sets the url to submit the post to.
		/// </summary>
		public string Url {
			get;
			set;
		}
		/// <summary>
		/// Gets or sets the url to submit the post to.
		/// </summary>
		public string UrlReferer {
			get;
			set;
		}
		/// <summary>
		/// Gets or sets the name value collection of items to post.
		/// </summary>
		public NameValueCollection PostItems {
			get;
			set;
		}
		/// <summary>
		/// Gets or sets the type of action to perform against the url.
		/// </summary>
		public PostTypeEnum Type {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the cookies used for the requests.
		/// </summary>
		public CookieCollection Cookies {
			get;
			set;
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
		public Dictionary<string, string> AdditionalHeader {
			get;
			set;
		}


		/// <summary>
		/// Default constructor.
		/// </summary>
		public PostRequest() {
			AutoRedirect = true;
			Cookies = null;
			IgnoreCookies = false;
			Type = PostTypeEnum.Get;
			PostItems = new NameValueCollection();
			AdditionalHeader = new Dictionary<string, string>();
		}

		/// <summary>
		/// Constructor that accepts a url as a parameter
		/// </summary>
		/// <param name="url">The url where the post will be submitted to.</param>
		public PostRequest(string url, string urlReferer)
			: this() {
			Url = url;
			UrlReferer = urlReferer;
		}

		/// <summary>
		/// Constructor allowing the setting of the url and items to post.
		/// </summary>
		/// <param name="url">the url for the post.</param>
		/// <param name="values">The values for the post.</param>
		public PostRequest(string url, string urlReferer, NameValueCollection values)
			: this(url, urlReferer) {
			PostItems = values;
		}


		/// <summary>
		/// Posts the supplied data to specified url.
		/// </summary>
		/// <returns>a string containing the result of the post.</returns>
		public PostResult Post() {
			StringBuilder parameters = new StringBuilder();
			for (int i = 0; i < PostItems.Count; i++) {
				EncodeAndAddItem(ref parameters, PostItems.GetKey(i), PostItems[i]);
			}

			return PostData(Url, parameters.ToString());
		}
		/// <summary>
		/// Posts the supplied data to specified url.
		/// </summary>
		/// <param name="url">The url to post to.</param>
		/// <returns>a string containing the result of the post.</returns>
		public PostResult Post(string url) {
			Url = url;
			return this.Post();
		}
		/// <summary>
		/// Posts the supplied data to specified url.
		/// </summary>
		/// <param name="url">The url to post to.</param>
		/// <param name="values">The values to post.</param>
		/// <returns>a string containing the result of the post.</returns>
		public PostResult Post(string url, NameValueCollection values) {
			PostItems = values;
			return this.Post(url);
		}



		/// <summary>
		/// Posts data to a specified url. Note that this assumes that you have already url encoded the post data.
		/// </summary>
		/// <param name="postData">The data to post.</param>
		/// <returns>Returns the result of the post.</returns>
		public PostResult PostData(string postData) {
			return PostData(Url, postData);
		}

		/// <summary>
		/// Posts data to a specified url. Note that this assumes that you have already url encoded the post data.
		/// </summary>
		/// <param name="url">the url to post to.</param>
		/// <param name="postData">The data to post.</param>
		/// <returns>Returns the result of the post.</returns>
		public PostResult PostData(string url, string postData) {
			PostResult result = null;
			HttpWebRequest request = null;
			if (WebInfo == null) {
				throw new ArgumentException("WebInfo cant be <null>!", "WebInfo");
			}
			if (Type == PostTypeEnum.Get & postData.Length > 0) {
				if (url.Contains("?")) {
					url += "&" + postData;
				} else {
					url += "?" + postData;
				}
			}

			Uri uri = new Uri(url);
			request = (HttpWebRequest)System.Net.WebRequest.Create(uri);
			request.Credentials = CredentialCache.DefaultCredentials;
			request.CookieContainer = new CookieContainer();
			if (IgnoreCookies == false) {
				request.CookieContainer.GetCookies(uri);
			}
			if (Cookies != null) {
				request.CookieContainer.Add(this.Cookies);
			}
			if (Proxy != null) {
				request.Proxy = Proxy;
			}
			if (Timeout > 0) {
				request.Timeout = Timeout;
			}

			if (url.StartsWith("https://") == true) {
				ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
			}

			request.UserAgent = WebInfo.Useragent;
			request.Accept = WebInfo.RequestAccept;
			request.Headers.Add(HttpRequestHeader.AcceptLanguage, WebInfo.AcceptLanguage);
			request.Headers.Add(HttpRequestHeader.AcceptEncoding, WebInfo.AcceptEncoding);
			request.Headers.Add(HttpRequestHeader.AcceptCharset, WebInfo.AcceptCharset);
			request.Headers.Add(HttpRequestHeader.KeepAlive, WebInfo.KeepAlive.ToString());
			foreach (KeyValuePair<string, string> pair in AdditionalHeader) {
				request.Headers.Add(pair.Key, pair.Value);
			}
			request.Referer = UrlReferer;
			request.KeepAlive = true;
			request.Method = Type.ToString().ToUpper();
			request.AllowAutoRedirect = AutoRedirect;

			ServicePointManager.Expect100Continue = false; // sonst error

			if (Type == PostTypeEnum.Post) {
				request.ContentType = "application/x-www-form-urlencoded";
				if (postData.Length > 0) {
					UTF8Encoding encoding = new UTF8Encoding();
					byte[] requestBuf = encoding.GetBytes(postData);

					request.ContentLength = requestBuf.Length;

					try {
						Stream writeStream = request.GetRequestStream();

						writeStream.Write(requestBuf, 0, requestBuf.Length);
						writeStream.Close();
					} catch { }
				} else {
					request.ContentLength = 0;
				}
			}

			try {
				using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
					result = new PostResult(response);
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
		/// <param name="dataItem">The data to encode.</param>
		/// <returns>A string containing the old data and the previously encoded data.</returns>
		private void EncodeAndAddItem(ref StringBuilder baseRequest, string key, string dataItem) {
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
		private bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors) {
			return true;
		}

	}
}
