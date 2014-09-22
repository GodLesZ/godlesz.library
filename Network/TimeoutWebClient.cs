using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace GodLesZ.Library.Network {

	public class TimeoutWebClient : WebClient {
		private int mTimeOut = 10000;

		public int TimeOut {
			get { return mTimeOut; }
			set { mTimeOut = value; }
		}


		protected override System.Net.WebRequest GetWebRequest(Uri address) {
			System.Net.WebRequest webRequest = base.GetWebRequest(address);
			webRequest.Timeout = mTimeOut;
			return webRequest;
		}

	}

}
