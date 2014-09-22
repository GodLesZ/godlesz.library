using System.Collections.Generic;

namespace GodLesZ.Library.Timer {

	public class TimerChangeEntry {
		private static Queue<TimerChangeEntry> mInstancePool = new Queue<TimerChangeEntry>();

		public Timer mTimer;
		public int mNewIndex;
		public bool mIsAdd;


		public TimerChangeEntry(Timer t, int newIndex, bool isAdd) {
			mTimer = t;
			mNewIndex = newIndex;
			mIsAdd = isAdd;
		}


		public void Free() {
			mInstancePool.Enqueue(this);
		}


		public static TimerChangeEntry GetInstance(Timer t, int newIndex, bool isAdd) {
			TimerChangeEntry e;

			if (mInstancePool.Count > 0) {
				e = mInstancePool.Dequeue();

				if (e == null)
					e = new TimerChangeEntry(t, newIndex, isAdd);
				else {
					e.mTimer = t;
					e.mNewIndex = newIndex;
					e.mIsAdd = isAdd;
				}
			} else {
				e = new TimerChangeEntry(t, newIndex, isAdd);
			}

			return e;
		}

	}

}
