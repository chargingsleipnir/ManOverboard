using System;
using System.Collections.Generic;
using System.Linq;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Collection of extension methods for the List type
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Removes all the items form the IList that match the provided constraint
        /// </summary>
        /// <typeparam name="T">The type of the elements in the IList</typeparam>
        /// <param name="list">The list to remove all from</param>
        /// <param name="match">The predicate used to determine if the item should be removed</param>
        /// <returns>Number of elements removed</returns>
        public static int RemoveAll<T>(this IList<T> list, Predicate<T> match)
        {
            int count = 0;

            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (match(list[i]))
                {
                    ++count;
                    list.RemoveAt(i);
                }
            }

            return count;
        }

        /// <summary>
        /// Removes all nulls from the list
        /// </summary>
        /// <typeparam name="T">The type of element contained by the list</typeparam>
        /// <param name="thisList">The list to remove the nulls from</param>
        public static void RemoveNulls<T>(this IList<T> thisList)
        {         
            thisList.RemoveAll((item) => item.Equals(null));
        }

        /// <summary>
        /// Adds an item only if it doesn't already exist in the list
        /// </summary>
        /// <typeparam name="T">The type of item the list contains</typeparam>
        /// <param name="thisList">The list to add the item to</param>
        /// <param name="newValue">The new item to be added</param>
        public static bool AddUnique<T>(this IList<T> thisList, T newValue)
        {
            if (thisList.Contains(newValue))
                return false;

            thisList.Add(newValue);
            return true;
        }

        /// <summary>
        /// Goes through the provided collection of objects and adds the unique ones
        /// to the list
        /// </summary>
        /// <typeparam name="T">The type of items contained in the list</typeparam>
        /// <param name="thisList">The list to add to</param>
        /// <param name="newValues">The new values to be added</param>
        /// <returns>The number of elements added</returns>
        public static int AddUniqueRange<T>(this List<T> thisList, IEnumerable<T> newValues)
        {
            int count = 0;

            foreach (T newValue in newValues)
            {
                if (thisList.AddUnique(newValue))
                    ++count;
            }

            return count;
        }

        /// <summary>
        /// Adds an item if the provided predicate is successful
        /// </summary>
        /// <typeparam name="T">The type of item the list contains</typeparam>
        /// <param name="thisList">The list to add the item to</param>
        /// <param name="newValue">The new item to be added</param>
        /// <param name="condition">The predicate that must succeed for the item to be added</param>
        public static void AddIf<T>(this IList<T> thisList, T newValue, Predicate<T> condition)
        {
            if (condition(newValue))
                thisList.Add(newValue);
        }

        /// <summary>
        /// Inserts an element if it isn't present in the collection already
        /// </summary>
        /// <typeparam name="T">The type of element contained by the collection</typeparam>
        /// <param name="thisList">The collection to be modified</param>
        /// <param name="index">The index to write at. Must be between 0 and list count</param>
        /// <param name="newValue">The value to be inserted if it doesn't already exist</param>
        /// <returns>True if added, false if not</returns>
        public static bool InsertUnique<T>(this IList<T> thisList, int index, T newValue)
        {
            if (thisList.Contains(newValue))
                return false;

            thisList.Insert(index, newValue);
            return true;
        }

        /// <summary>
        /// Inserts a range of unique elements to the collection
        /// </summary>
        /// <typeparam name="T">The type of element the list contains</typeparam>
        /// <param name="thisList">The list of items to insert into</param>
        /// <param name="index">The index to insert the elements at</param>
        /// <param name="items">The items to be added</param>
        /// <returns>The number of unique elements added</returns>
        public static int InsertUniqueRange<T>(this IList<T> thisList, int index, IEnumerable<T> items)
        {
            IEnumerable<T> uniqueItems = items.Distinct().Where((x) => !thisList.Contains(x));

            int count = 0;

            foreach (T item in uniqueItems)
            {
                thisList.Insert(index, item);
                ++index;
                ++count;
            }

            return count;
        }

        /// <summary>
        /// Helper to check if the provided index is within the valid range
        /// </summary>
        /// <typeparam name="T">Type of the list item</typeparam>
        /// <param name="thisList">The list to check index validity on</param>
        /// <param name="index">The index value to validate</param>
        /// <returns></returns>
        public static bool IsIndexInRange<T>(this IList<T> thisList, int index)
        {
            return index >= 0 && index < thisList.Count;
        }

        /// <summary>
        /// Swaps the values at the provided indices
        /// </summary>
        /// <typeparam name="T">List type</typeparam>
        /// <param name="thisList">The list to swap the values for</param>
        /// <param name="index1">The first index</param>
        /// <param name="index2">The second index</param>
        public static void SwapValues<T>(this IList<T> thisList, int index1, int index2)
        {
            if (index1 == index2)
                return;

            if (!thisList.IsIndexInRange(index1) || !thisList.IsIndexInRange(index2))
                return;

            T temp = thisList[index1];

            thisList[index1] = thisList[index2];
            thisList[index2] = temp;            
        }
    }
}
