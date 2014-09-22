using System;

namespace GodLesZ.Library {

	/// <summary>
	/// Represents an item stored in a priority queue.
	/// </summary>
	/// <typeparam name="TValue">The type of object in the queue.</typeparam>
	/// <typeparam name="TPriority">The type of the priority field.</typeparam>
	[Serializable]
	public class PriorityQueueCollectionItem<TValue, TPriority> {

		/// <summary>
		/// Gets the value of this PriorityQueueItem.
		/// </summary>
		public TValue Value {
			get;
			private set;
		}

		/// <summary>
		/// Gets the priority associated with this PriorityQueueItem.
		/// </summary>
		public TPriority Priority {
			get;
			private set;
		}


		public PriorityQueueCollectionItem(TValue val, TPriority pri) {
			Value = val;
			Priority = pri;
		}

	}

}
