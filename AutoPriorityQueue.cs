using System;
using System.Collections.Generic;

namespace GodLesZ.Library {

	/// <summary>
	/// Priority Queue
	/// </summary>
	/// <typeparam name="T"></typeparam>    
	public class AutoPriorityQueue<T> where T : IComparable<T> {
		protected List<T> mStoredValues;

		/// <summary>
		/// Gets the number of values stored within the Priority Queue
		/// </summary>
		public int Count {
			get { return mStoredValues.Count - 1; }
		}


		public AutoPriorityQueue() {
			//Initialize the array that will hold the values
			mStoredValues = new List<T>();

			//Fill the first cell in the array with an empty value
			mStoredValues.Add(default(T));
		}


		public bool Contains(T value) {
			return mStoredValues.Contains(value);
		}

		public T Find(T value) {
			int index = mStoredValues.IndexOf(value);
			if (index < 0)
				return default(T);
			return mStoredValues[index];
		}

		/// <summary>
		/// Returns the value at the head of the Priority Queue without removing it.
		/// </summary>
		public T Peek() {
			if (this.Count == 0)
				return default(T); //Priority Queue empty
			else
				return mStoredValues[1]; //head of the queue
		}

		/// <summary>
		/// Adds a value to the Priority Queue
		/// </summary>
		public void Enqueue(T value) {
			//Add the value to the internal array
			mStoredValues.Add(value);

			//Bubble up to preserve the heap property,
			//starting at the inserted value
			this.BubbleUp(mStoredValues.Count - 1);
		}

		/// <summary>
		/// Returns the minimum value inside the Priority Queue
		/// </summary>
		public T Dequeue() {
			if (this.Count == 0)
				return default(T); //queue is empty
			else {
				//The smallest value in the Priority Queue is the first item in the array
				T minValue = this.mStoredValues[1];

				//If there's more than one item, replace the first item in the array with the last one
				if (this.mStoredValues.Count > 2) {
					T lastValue = this.mStoredValues[mStoredValues.Count - 1];

					//Move last node to the head
					this.mStoredValues.RemoveAt(mStoredValues.Count - 1);
					this.mStoredValues[1] = lastValue;

					//Bubble down
					this.BubbleDown(1);
				} else {
					//Remove the only value stored in the queue
					mStoredValues.RemoveAt(1);
				}

				return minValue;
			}
		}

		/// <summary>
		/// When item was changed this restores the heap condition.
		/// This works only for reference types or types that implement
		/// IEquatable correctly, since it searches for the given item
		/// in the queue. To change an item you would typically obtain
		/// it by calling Find(item), change any property and finally call
		/// Update(item).
		/// </summary>
		/// <param name="item"></param>
		public void Update(T item) {
			int index = mStoredValues.IndexOf(item);

			if (IsParentBigger(index))
				BubbleUp(index);
			else if (IsLeftChildSmaller(index) || IsRightChildSmaller(index))
				BubbleDown(index);
		}

		/// <summary>
		/// Restores the heap-order property between child and parent values going up towards the head
		/// </summary>
		protected void BubbleUp(int startCell) {
			int cell = startCell;

			//Bubble up as long as the parent is greater
			while (this.IsParentBigger(cell)) {
				//Get values of parent and child
				T parentValue = this.mStoredValues[cell / 2];
				T childValue = this.mStoredValues[cell];

				//Swap the values
				this.mStoredValues[cell / 2] = childValue;
				this.mStoredValues[cell] = parentValue;

				cell /= 2; //go up parents
			}
		}

		/// <summary>
		/// Restores the heap-order property between child and parent values going down towards the bottom
		/// </summary>
		protected void BubbleDown(int startCell) {
			int cell = startCell;

			//Bubble down as long as either child is smaller
			while (this.IsLeftChildSmaller(cell) || this.IsRightChildSmaller(cell)) {
				int child = this.CompareChild(cell);

				if (child == -1) //Left Child
                {
					//Swap values
					T parentValue = mStoredValues[cell];
					T leftChildValue = mStoredValues[2 * cell];

					mStoredValues[cell] = leftChildValue;
					mStoredValues[2 * cell] = parentValue;

					cell = 2 * cell; //move down to left child
				} else if (child == 1) {
					//Right Child

					//Swap values
					T parentValue = mStoredValues[cell];
					T rightChildValue = mStoredValues[2 * cell + 1];

					mStoredValues[cell] = rightChildValue;
					mStoredValues[2 * cell + 1] = parentValue;

					cell = 2 * cell + 1; //move down to right child
				}
			}
		}

		/// <summary>
		/// Returns if the value of a parent is greater than its child
		/// </summary>
		protected bool IsParentBigger(int childCell) {
			if (childCell == 1)
				return false; //top of heap, no parent
			else
				return mStoredValues[childCell / 2].CompareTo(mStoredValues[childCell]) > 0;
			//return storedNodes[childCell / 2].Key > storedNodes[childCell].Key;
		}

		/// <summary>
		/// Returns whether the left child cell is smaller than the parent cell.
		/// Returns false if a left child does not exist.
		/// </summary>
		protected bool IsLeftChildSmaller(int parentCell) {
			if (2 * parentCell >= mStoredValues.Count)
				return false; //out of bounds
			else
				return mStoredValues[2 * parentCell].CompareTo(mStoredValues[parentCell]) < 0;
			//return storedNodes[2 * parentCell].Key < storedNodes[parentCell].Key;
		}

		/// <summary>
		/// Returns whether the right child cell is smaller than the parent cell.
		/// Returns false if a right child does not exist.
		/// </summary>
		protected bool IsRightChildSmaller(int parentCell) {
			if (2 * parentCell + 1 >= mStoredValues.Count)
				return false; //out of bounds
			else
				return mStoredValues[2 * parentCell + 1].CompareTo(mStoredValues[parentCell]) < 0;
			//return storedNodes[2 * parentCell + 1].Key < storedNodes[parentCell].Key;
		}

		/// <summary>
		/// Compares the children cells of a parent cell. -1 indicates the left child is the smaller of the two,
		/// 1 indicates the right child is the smaller of the two, 0 inidicates that neither child is smaller than the parent.
		/// </summary>
		protected int CompareChild(int parentCell) {
			bool leftChildSmaller = this.IsLeftChildSmaller(parentCell);
			bool rightChildSmaller = this.IsRightChildSmaller(parentCell);

			if (leftChildSmaller || rightChildSmaller) {
				if (leftChildSmaller && rightChildSmaller) {
					//Figure out which of the two is smaller
					int leftChild = 2 * parentCell;
					int rightChild = 2 * parentCell + 1;

					T leftValue = this.mStoredValues[leftChild];
					T rightValue = this.mStoredValues[rightChild];

					//Compare the values of the children
					if (leftValue.CompareTo(rightValue) <= 0)
						return -1; //left child is smaller
					else
						return 1; //right child is smaller
				} else if (leftChildSmaller)
					return -1; //left child is smaller
				else
					return 1; //right child smaller
			} else
				return 0; //both children are bigger or don't exist
		}

	}

}