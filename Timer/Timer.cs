using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace GodLesZ.Library.Timer {

	public class Timer {
		public static bool Closing = false;
		public static TimerCallback GlobalSignal;

		private static Queue<Timer> mQueue = new Queue<Timer>();
		private static int mBreakCount = 20000;

		private static int mQueueCountAtSlice;

		private bool mQueued;
		private DateTime mNext;
		private TimeSpan mDelay;
		private TimeSpan mInterval;
		private bool mRunning;
		private int mIndex, mCount;
		private ETimerPriority mPriority;
		private List<Timer> mList;

		public static int BreakCount {
			get { return mBreakCount; }
			set { mBreakCount = value; }
		}

		public static Queue<Timer> Queue {
			get { return mQueue; }
		}

		public static long Ticks {
			get { return DateTime.Now.Ticks; }
		}


		public ETimerPriority Priority {
			get { return mPriority; }
			set {
				if (mPriority != value) {
					mPriority = value;

					if (mRunning)
						TimerThread.PriorityChange(this, (int)mPriority);
				}
			}
		}

		public DateTime Next {
			get { return mNext; }
			set { mNext = value; }
		}

		public TimeSpan Delay {
			get { return mDelay; }
			set { mDelay = value; }
		}

		public TimeSpan Interval {
			get { return mInterval; }
			set { mInterval = value; }
		}

		public bool Running {
			get { return mRunning; }
			set {
				if (value == true) {
					Start();
				} else {
					Stop();
				}
			}
		}

		public int Index {
			get { return mIndex; }
			set { mIndex = value; }
		}

		public bool Queued {
			get { return mQueued; }
			set { mQueued = value; }
		}

		public List<Timer> List {
			get { return mList; }
			set { mList = value; }
		}

		public int Count {
			get { return mCount; }
		}


		public virtual bool DefRegCreation {
			get { return true; }
		}


		public Timer(TimeSpan delay, TimeSpan interval, int count) {
			mDelay = delay;
			mInterval = interval;
			mCount = count;

			if (DefRegCreation)
				RegCreation();
		}

		public Timer(TimeSpan delay)
			: this(delay, TimeSpan.Zero, 1) {
		}

		public Timer(TimeSpan delay, TimeSpan interval)
			: this(delay, interval, 0) {
		}


		public static void Slice() {
			lock (mQueue) {
				mQueueCountAtSlice = mQueue.Count;

				int index = 0;
				while (index < mBreakCount && mQueue.Count != 0) {
					Timer t = mQueue.Dequeue();
					t.OnTick();
					t.Queued = false;
					++index;
				}
			}
		}

		public static void DumpInfo(TextWriter tw) {
			TimerThread.DumpInfo(tw);
		}


		public virtual void RegCreation() {
			TimerProfile prof = GetProfile();

			if (prof != null)
				prof.Created++;
		}

		public static ETimerPriority ComputePriority(TimeSpan ts) {
			if (ts >= TimeSpan.FromMinutes(1.0))
				return ETimerPriority.FiveSeconds;

			if (ts >= TimeSpan.FromSeconds(10.0))
				return ETimerPriority.OneSecond;

			if (ts >= TimeSpan.FromSeconds(5.0))
				return ETimerPriority.TwoFiftyMS;

			if (ts >= TimeSpan.FromSeconds(2.5))
				return ETimerPriority.FiftyMS;

			if (ts >= TimeSpan.FromSeconds(1.0))
				return ETimerPriority.TwentyFiveMS;

			if (ts >= TimeSpan.FromSeconds(0.5))
				return ETimerPriority.TenMS;

			return ETimerPriority.EveryTick;
		}

		public static bool IsValid(Timer t) {
			return (t != null && t.Running == true);
		}


		private static string FormatDelegate(Delegate callback) {
			if (callback == null)
				return "null";

			return String.Format("{0}.{1}", callback.Method.DeclaringType.FullName, callback.Method.Name);
		}


		public TimerProfile GetProfile() {
			string name = ToString();
			if (string.IsNullOrEmpty(name)) {
				name = "null";
			}

			return TimerProfile.Acquire(name);
		}


		#region DelayCall(..)
		public static Timer DelayCall(TimeSpan delay, TimerCallback callback) {
			return DelayCall(delay, TimeSpan.Zero, 1, callback);
		}

		public static Timer DelayCall(TimeSpan delay, TimeSpan interval, TimerCallback callback) {
			return DelayCall(delay, interval, 0, callback);
		}

		public static Timer DelayCall(TimeSpan delay, TimeSpan interval, int count, TimerCallback callback) {
			Timer t = new DelayCallTimer(delay, interval, count, callback);

			if (count == 1)
				t.Priority = ComputePriority(delay);
			else
				t.Priority = ComputePriority(interval);

			t.Start();

			return t;
		}

		public static Timer DelayCall(TimeSpan delay, TimerStateCallback callback, object state) {
			return DelayCall(delay, TimeSpan.Zero, 1, callback, state);
		}

		public static Timer DelayCall(TimeSpan delay, TimeSpan interval, TimerStateCallback callback, object state) {
			return DelayCall(delay, interval, 0, callback, state);
		}

		public static Timer DelayCall(TimeSpan delay, TimeSpan interval, int count, TimerStateCallback callback, object state) {
			Timer t = new DelayStateCallTimer(delay, interval, count, callback, state);

			if (count == 1)
				t.Priority = ComputePriority(delay);
			else
				t.Priority = ComputePriority(interval);

			t.Start();

			return t;
		}
		#endregion

		#region DelayCall<T>(..)
		public static Timer DelayCall<T>(TimeSpan delay, TimerStateCallback<T> callback, T state) {
			return DelayCall(delay, TimeSpan.Zero, 1, callback, state);
		}

		public static Timer DelayCall<T>(TimeSpan delay, TimeSpan interval, TimerStateCallback<T> callback, T state) {
			return DelayCall(delay, interval, 0, callback, state);
		}

		public static Timer DelayCall<T>(TimeSpan delay, TimeSpan interval, int count, TimerStateCallback<T> callback, T state) {
			Timer t = new DelayStateCallTimer<T>(delay, interval, count, callback, state);

			if (count == 1)
				t.Priority = ComputePriority(delay);
			else
				t.Priority = ComputePriority(interval);

			t.Start();

			return t;
		}
		#endregion

		#region DelayCall Timers
		private class DelayCallTimer : Timer {
			private TimerCallback mCallback;

			public TimerCallback Callback {
				get { return mCallback; }
			}

			public override bool DefRegCreation {
				get { return false; }
			}


			public DelayCallTimer(TimeSpan delay, TimeSpan interval, int count, TimerCallback callback)
				: base(delay, interval, count) {
				mCallback = callback;
				RegCreation();
			}

			protected override void OnTick() {
				if (mCallback != null)
					mCallback();
			}

			public override string ToString() {
				return String.Format("DelayCallTimer[{0}]", FormatDelegate(mCallback));
			}
		}

		private class DelayStateCallTimer : Timer {
			private TimerStateCallback mCallback;
			private object mState;

			public TimerStateCallback Callback {
				get { return mCallback; }
			}

			public override bool DefRegCreation {
				get { return false; }
			}

			public DelayStateCallTimer(TimeSpan delay, TimeSpan interval, int count, TimerStateCallback callback, object state)
				: base(delay, interval, count) {
				mCallback = callback;
				mState = state;

				RegCreation();
			}

			protected override void OnTick() {
				if (mCallback != null)
					mCallback(mState);
			}

			public override string ToString() {
				return String.Format("DelayStateCall[{0}]", FormatDelegate(mCallback));
			}
		}

		private class DelayStateCallTimer<T> : Timer {
			private TimerStateCallback<T> mCallback;
			private T mState;

			public TimerStateCallback<T> Callback {
				get { return mCallback; }
			}

			public override bool DefRegCreation {
				get { return false; }
			}

			public DelayStateCallTimer(TimeSpan delay, TimeSpan interval, int count, TimerStateCallback<T> callback, T state)
				: base(delay, interval, count) {
				mCallback = callback;
				mState = state;

				RegCreation();
			}

			protected override void OnTick() {
				if (mCallback != null)
					mCallback(mState);
			}

			public override string ToString() {
				return String.Format("DelayStateCall[{0}]", FormatDelegate(mCallback));
			}
		}
		#endregion

		public void Start() {
			if (mRunning)
				return;

			mRunning = true;
			TimerThread.AddTimer(this);

			TimerProfile prof = GetProfile();
			if (prof != null)
				prof.Started++;
		}

		public void Stop() {
			if (!mRunning)
				return;
			mRunning = false;
			TimerThread.RemoveTimer(this);

			TimerProfile prof = GetProfile();
			if (prof != null)
				prof.Stopped++;
		}

		protected virtual void OnTick() {
		}


		public override string ToString() {
			return GetType().FullName;
		}

	}


}
