using System.Collections.Generic;
using System.IO;
using GodLesZ.Library.Diagnostics;

namespace GodLesZ.Library.Timer {

	public class TimerProfile : BaseProfile {
		private static Dictionary<string, TimerProfile> mProfiles = new Dictionary<string, TimerProfile>();

		private long mCreated;
		private long mStarted;
		private long mStopped;


		public static IEnumerable<TimerProfile> Profiles {
			get { return mProfiles.Values; }
		}

		public long Created {
			get { return mCreated; }
			set { mCreated = value; }
		}

		public long Started {
			get { return mStarted; }
			set { mStarted = value; }
		}

		public long Stopped {
			get { return mStopped; }
			set { mStopped = value; }
		}



		public TimerProfile(string name)
			: base(name) {
		}




		public static TimerProfile Acquire(string name) {
			if (BaseProfile.Profiling == false) {
				return null;
			}

			TimerProfile prof;
			if (mProfiles.TryGetValue(name, out prof) == false) {
				mProfiles.Add(name, prof = new TimerProfile(name));
			}

			return prof;
		}



		public override void WriteTo(TextWriter op) {
			base.WriteTo(op);

			op.Write("\t{0,12:N0} {1,12:N0} {2,-12:N0}", mCreated, mStarted, mStopped);
		}

	}

}
