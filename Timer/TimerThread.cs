using System;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using System.IO;

namespace GodLesZ.Library.Timer {

	public class TimerThread {
		private static Queue mChangeQueue = Queue.Synchronized(new Queue());
		private static AutoResetEvent mSignal = new AutoResetEvent(false);

		private static DateTime[] mNextPriorities = new DateTime[8];
		private static TimeSpan[] mPriorityDelays = new TimeSpan[8] {
				TimeSpan.Zero,
				TimeSpan.FromMilliseconds( 10.0 ),
				TimeSpan.FromMilliseconds( 25.0 ),
				TimeSpan.FromMilliseconds( 50.0 ),
				TimeSpan.FromMilliseconds( 250.0 ),
				TimeSpan.FromSeconds( 1.0 ),
				TimeSpan.FromSeconds( 5.0 ),
				TimeSpan.FromMinutes( 1.0 )
			};

		private static List<Timer>[] mTimers = new List<Timer>[8] {
				new List<Timer>(),
				new List<Timer>(),
				new List<Timer>(),
				new List<Timer>(),
				new List<Timer>(),
				new List<Timer>(),
				new List<Timer>(),
				new List<Timer>(),
			};


		public TimerThread() {
		}


		public static void DumpInfo(TextWriter tw) {
			for (int i = 0; i < 8; ++i) {
				tw.WriteLine("Priority: {0}", (ETimerPriority)i);
				tw.WriteLine();

				Dictionary<string, List<Timer>> hash = new Dictionary<string, List<Timer>>();

				for (int j = 0; j < mTimers[i].Count; ++j) {
					Timer t = mTimers[i][j];

					string key = t.ToString();

					List<Timer> list;
					hash.TryGetValue(key, out list);

					if (list == null)
						hash[key] = list = new List<Timer>();

					list.Add(t);
				}

				foreach (KeyValuePair<string, List<Timer>> kv in hash) {
					string key = kv.Key;
					List<Timer> list = kv.Value;

					tw.WriteLine("Type: {0}; Count: {1}; Percent: {2}%", key, list.Count, (int)(100 * (list.Count / (double)mTimers[i].Count)));
				}

				tw.WriteLine();
				tw.WriteLine();
			}
		}

		public static void Change(Timer t, int newIndex, bool isAdd) {
			mChangeQueue.Enqueue(TimerChangeEntry.GetInstance(t, newIndex, isAdd));
			mSignal.Set();
		}

		public static void AddTimer(Timer t) {
			Change(t, (int)t.Priority, true);
		}

		public static void PriorityChange(Timer t, int newPrio) {
			Change(t, newPrio, false);
		}

		public static void RemoveTimer(Timer t) {
			Change(t, -1, false);
		}


		private static void ProcessChangeQueue() {
			while (mChangeQueue.Count > 0) {
				TimerChangeEntry tce = (TimerChangeEntry)mChangeQueue.Dequeue();
				Timer timer = tce.mTimer;
				int newIndex = tce.mNewIndex;

				if (timer.List != null)
					timer.List.Remove(timer);

				if (tce.mIsAdd) {
					timer.Next = DateTime.Now + timer.Delay;
					timer.Index = 0;
				}

				if (newIndex >= 0) {
					timer.List = mTimers[newIndex];
					timer.List.Add(timer);
				} else {
					timer.List = null;
				}

				tce.Free();
			}
		}

		public static void Set() {
			mSignal.Set();
		}

		public void TimerMain() {
			DateTime now;
			int i, j;
			bool loaded;

			while (!Timer.Closing) {
				ProcessChangeQueue();

				loaded = false;

				for (i = 0; i < mTimers.Length; i++) {
					now = DateTime.Now;
					if (now < mNextPriorities[i])
						break;

					mNextPriorities[i] = now + mPriorityDelays[i];

					for (j = 0; j < mTimers[i].Count; j++) {
						Timer t = mTimers[i][j];

						if (!t.Queued && now > t.Next) {
							t.Queued = true;

							lock (Timer.Queue) {
								Timer.Queue.Enqueue(t);
							}

							loaded = true;

							if (t.Count != 0 && (++t.Index >= t.Count)) {
								t.Stop();
							} else {
								t.Next = now + t.Interval;
							}
						}
					}
				}

				if (loaded && Timer.GlobalSignal != null) {
					Timer.GlobalSignal();
				}

				mSignal.WaitOne(10, false);
			}
		}
	}


}
