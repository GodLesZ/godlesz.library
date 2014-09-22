using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace GodLesZ.Library.Diagnostics {

	public abstract class BaseProfile {
		public static bool Profiling = false;


		private string mName;
		private long mCount;
		private TimeSpan mTotalTime;
		private TimeSpan mPeakTime;

		private Stopwatch mStopwatch;


		public string Name {
			get { return mName; }
		}

		public long Count {
			get { return mCount; }
		}

		public TimeSpan AverageTime {
			get { return TimeSpan.FromTicks(mTotalTime.Ticks / Math.Max(1, mCount)); }
		}

		public TimeSpan PeakTime {
			get { return mPeakTime; }
		}

		public TimeSpan TotalTime {
			get { return mTotalTime; }
		}



		public BaseProfile(string name) {
			mName = name;
			mStopwatch = new Stopwatch();
		}



		public static void WriteAll<T>(TextWriter op, IEnumerable<T> profiles) where T : BaseProfile {
			List<T> list = new List<T>(profiles);

			list.Sort(delegate(T a, T b) {
				return -a.TotalTime.CompareTo(b.TotalTime);
			});

			foreach (T prof in list) {
				prof.WriteTo(op);
				op.WriteLine();
			}
		}


		public virtual void Start() {
			if (mStopwatch.IsRunning) {
				mStopwatch.Reset();
			}

			mStopwatch.Start();
		}

		public virtual void Finish() {
			TimeSpan elapsed = mStopwatch.Elapsed;

			mTotalTime += elapsed;

			if (elapsed > mPeakTime) {
				mPeakTime = elapsed;
			}

			mCount++;

			mStopwatch.Reset();
		}


		public virtual void WriteTo(TextWriter op) {
			op.Write("{0,-100} {1,12:N0} {2,12:F5} {3,-12:F5} {4,12:F5}", Name, Count, AverageTime.TotalSeconds, PeakTime.TotalSeconds, TotalTime.TotalSeconds);
		}

	}

}
