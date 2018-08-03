using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroProgress.Common.Collections
{
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
        private List<T> Items = new List<T>();

        [Tooltip("Clear when this set is first enabled")]
        public bool ClearOnEnable = true;

        public event EventHandler OnSetModified;

        public event EventHandler<SetModifiedEventArgs<T>> OnItemAdded;

        public event EventHandler<SetModifiedEventArgs<T>> OnItemRemoved;

        public int Count { get { return Items.Count; } }

        public bool IsReadOnly { get { return false; } }

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

        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return Items.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            Items.Insert(index, item);

            RaiseItemChange(OnItemAdded, item);
            RaiseSetModified();
        }

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

        public void Add(T item)
        {
            bool canAdd = !Items.Contains(item);

            if (!canAdd)
                return;
            
            Items.Add(item);
            RaiseItemChange(OnItemAdded, item);
            RaiseSetModified();            
        }

        public void AddRange(IEnumerable<T> ItemsToAdd)
        {
            foreach (T item in ItemsToAdd)
            {
                Add(item);
                RaiseItemChange(OnItemAdded, item);
            }

            RaiseSetModified();
        }

        public bool Contains(T item)
        {
            return Items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Items.CopyTo(array, arrayIndex);
        }

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

        public void RemoveRangeFromSet(IEnumerable<T> ItemsToRemove)
        {
            foreach (T item in ItemsToRemove)
            {
                Items.Remove(item);
                RaiseItemChange(OnItemRemoved, item);
            }

            RaiseSetModified();
        }

        public void Clear()
        {
            Items.Clear();
        }

        private void RaiseSetModified()
        {
            if (OnSetModified != null)
                OnSetModified(this, EventArgs.Empty);
        }

        private void RaiseItemChange(EventHandler<SetModifiedEventArgs<T>> Event, T ChangeItem)
        {
            if (Event != null)
                Event(this, new SetModifiedEventArgs<T>() { Item = ChangeItem });
        }
    }
}