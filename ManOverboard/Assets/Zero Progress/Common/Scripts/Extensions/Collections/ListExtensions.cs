
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
        public static void AddUnique<T>(this List<T> thisList, T newValue)
        {
            if (thisList.Contains(newValue))
                return;

            thisList.Add(newValue);
        }

        /// <summary>
        /// Adds an item if the provided predicate is successful
        /// </summary>
        /// <typeparam name="T">The type of item the list contains</typeparam>
        /// <param name="thisList">The list to add the item to</param>
        /// <param name="newValue">The new item to be added</param>
        /// <param name="condition">The predicate that must succeed for the item to be added</param>
        public static void AddIf<T>(this List<T> thisList, T newValue, System.Predicate<T> condition)
        {
            if (condition(newValue))
                thisList.Add(newValue);
        }
    }
}
