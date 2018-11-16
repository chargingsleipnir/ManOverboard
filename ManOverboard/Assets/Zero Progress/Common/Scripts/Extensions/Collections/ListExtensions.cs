
using System.Collections.Generic;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Collection of extension methods for the List type
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Removes all nulls from the list
        /// </summary>
        /// <typeparam name="T">The type of element contained by the list</typeparam>
        /// <param name="thisList">The list to remove the nulls from</param>
        public static void RemoveNulls<T>(this List<T> thisList)
        {
            thisList.RemoveAll((item) => item.Equals(null));
        }

        /// <summary>
        /// Adds an item only if it doesn't already exist in the list
        /// </summary>
        /// <typeparam name="T">The type of item the list contains</typeparam>
        /// <param name="thisList">The list to add the item to</param>
        /// <param name="newValue">The new item to be added</param>
        public static void AddUnique<T>(this IList<T> thisList, T newValue)
        {
            if (thisList.Contains(newValue))
                return;

            thisList.Add(newValue);
        }

        /// <summary>
        /// Goes through the provided collection of objects and adds the unique ones
        /// to the list
        /// </summary>
        /// <typeparam name="T">The type of items contained in the list</typeparam>
        /// <param name="thisList">The list to add to</param>
        /// <param name="newValues">The new values to be added</param>
        public static void AddUniqueRange<T>(this List<T> thisList, IEnumerable<T> newValues)
        {
            foreach (T newValue in newValues)
            {
                thisList.AddUnique(newValue);
            }
        }

        /// <summary>
        /// Adds an item if the provided predicate is successful
        /// </summary>
        /// <typeparam name="T">The type of item the list contains</typeparam>
        /// <param name="thisList">The list to add the item to</param>
        /// <param name="newValue">The new item to be added</param>
        /// <param name="condition">The predicate that must succeed for the item to be added</param>
        public static void AddIf<T>(this IList<T> thisList, T newValue, System.Predicate<T> condition)
        {
            if (condition(newValue))
                thisList.Add(newValue);
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
