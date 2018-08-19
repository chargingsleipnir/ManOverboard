using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroProgress.Common.Collections
{
    /// <summary>
    /// Event args for when the set is modified
    /// </summary>
    /// <typeparam name="T">The type of collection</typeparam>
    [Serializable]
    public class SetModifiedEventArgs<T> : EventArgs
    {
        public T Item;
    }

    /// <summary>
    /// Intended to keep a collection of runtime objects (can be considered a Manager)
    /// </summary>
    /// <typeparam name="T">The type of objects to store in this set</typeparam>
    public class ScriptableSet<T> : ScriptableObject, IEnumerable<T>, IList<T>
    {
        [SerializeField]
        [Tooltip("All the items that are a part of this set")]
        private List<T> Items = new List<T>();

        [Tooltip("Clear when this set is first enabled")]
        public bool ClearOnEnable = true;

        /// <summary>
        /// Event fired when any kind of modification is applied to the set
        /// </summary>
        public event EventHandler OnSetModified;

        /// <summary>
        /// Event fired when an item has been added to the set
        /// </summary>
        public event EventHandler<SetModifiedEventArgs<T>> OnItemAdded;

        /// <summary>
        /// Event fired when an item has been removed from the set
        /// </summary>
        public event EventHandler<SetModifiedEventArgs<T>> OnItemRemoved;

        /// <summary>
        /// Number of items in the set
        /// </summary>
        public int Count { get { return Items.Count; } }

        /// <summary>
        /// This is not a readonly set (part of the IList interface)
        /// </summary>
        public bool IsReadOnly { get { return false; } }

        /// <summary>
        /// Index accessibility for the set
        /// </summary>
        /// <param name="index">The index to retrieve/set</param>
        /// <returns>The item at the index to retrieve</returns>
        public T this[int index]
        {
            get
            {
                return Items[index];
            }
            set
            {
                if (Items.Contains(value))
                {
                    Debug.LogError("Value already exists in ScriptableSet, cannot set value: " + value.ToString());
                    return;
                }

                Items[index] = value;
            }
        }

        private void OnEnable()
        {
            if (ClearOnEnable)
                Clear();
        }

        /// <summary>
        /// Retrieves the enumerator for this collection
        /// </summary>
        /// <returns>Enumerator</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        /// <summary>
        /// Retrieves the enumerator for this collection
        /// </summary>
        /// <returns>Enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        /// <summary>
        /// Retrieves the index of the specified item
        /// </summary>
        /// <param name="item">The item to try get the index of</param>
        /// <returns>The index of the item if found, or -1 if not found</returns>
        public int IndexOf(T item)
        {
            return Items.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item at the specified index
        /// </summary>
        /// <param name="index">The index to insert at</param>
        /// <param name="item">The item to insert</param>
        public void Insert(int index, T item)
        {
            Items.Insert(index, item);

            RaiseItemChange(OnItemAdded, item);
            RaiseSetModified();
        }

        /// <summary>
        /// Removes an item at the specified index
        /// </summary>
        /// <param name="index">The index to remove the item from</param>
        public void RemoveAt(int index)
        {
            bool validIndex = index < Items.Count;

            if (!validIndex)
                return;

            T removedItem = Items[index];

            Items.RemoveAt(index);

            RaiseItemChange(OnItemRemoved, removedItem);
            RaiseSetModified();
        }

        /// <summary>
        /// Adds an item to the set
        /// </summary>
        /// <param name="item">The item to add</param>
        public void Add(T item)
        {
            bool canAdd = !Items.Contains(item);

            if (!canAdd)
                return;
            
            Items.Add(item);
            RaiseItemChange(OnItemAdded, item);
            RaiseSetModified();            
        }

        /// <summary>
        /// Adds a collection of items to the set
        /// </summary>
        /// <param name="ItemsToAdd">The items to be added</param>
        public void AddRange(IEnumerable<T> ItemsToAdd)
        {
            foreach (T item in ItemsToAdd)
            {
                Add(item);
                RaiseItemChange(OnItemAdded, item);
            }

            RaiseSetModified();
        }

        /// <summary>
        /// Checks if the set has the specified items
        /// </summary>
        /// <param name="item">The item to check containment for</param>
        /// <returns>True if it's contained, false if not</returns>
        public bool Contains(T item)
        {
            return Items.Contains(item);
        }

        /// <summary>
        /// Copies this set to a specified array
        /// </summary>
        /// <param name="array">The array to copy to</param>
        /// <param name="arrayIndex">The index to start copying at</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            Items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Tries to remove an item
        /// </summary>
        /// <param name="item">The item to be removed</param>
        /// <returns>True if removed, false if not</returns>
        public bool Remove(T item)
        {
            bool result = Items.Remove(item);

            if(result)
            {
                RaiseItemChange(OnItemRemoved, item);
                RaiseSetModified();
            }

            return result;
        }

        /// <summary>
        /// Removes a collection of items from the set
        /// </summary>
        /// <param name="ItemsToRemove">The collection of items to be removed</param>
        public void RemoveRangeFromSet(IEnumerable<T> ItemsToRemove)
        {
            foreach (T item in ItemsToRemove)
            {
                Items.Remove(item);
                RaiseItemChange(OnItemRemoved, item);
            }

            RaiseSetModified();
        }

        /// <summary>
        /// Clears the collection
        /// </summary>
        public void Clear()
        {
            Items.Clear();
            RaiseSetModified();
        }

        /// <summary>
        /// Helper to raise the collection modified event
        /// </summary>
        private void RaiseSetModified()
        {
            if (OnSetModified != null)
                OnSetModified(this, EventArgs.Empty);
        }

        /// <summary>
        /// Helper to raise an event for when an item is added or removed 
        /// </summary>
        /// <param name="Event">The event to fire</param>
        /// <param name="ChangeItem">The item that fired the event</param>
        private void RaiseItemChange(EventHandler<SetModifiedEventArgs<T>> Event, T ChangeItem)
        {
            if (Event != null)
                Event(this, new SetModifiedEventArgs<T>() { Item = ChangeItem });
        }
    }
}