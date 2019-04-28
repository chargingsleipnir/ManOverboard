using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Extension methods for IEnumerables
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Checks to determine what the best match to the provided
        /// string in the collection of items is
        /// </summary>
        /// <param name="items">The items to evaluate</param>
        /// <param name="compareString">The string to check</param>
        /// <param name="bestMatch">The string that was best match</param>
        /// <returns>The value of the highest match (the highest string.IndexOf() value)</returns>
        public static int FindBestContainsMatch(this IEnumerable<string> items,
            string compareString, ref string bestMatch)
        {
            int highestIndex = -1;
            bestMatch = null;

            foreach (string item in items)
            {
                int index = compareString.IndexOf(item);

                if (index > highestIndex)
                {
                    highestIndex = index;
                    bestMatch = item;
                }
            }

            return highestIndex;
        }

        /// <summary>
        /// Checks to determine what the best match to the provided
        /// string in the collection of items is
        /// </summary>
        /// <param name="items">The items to evaluate</param>
        /// <param name="compareString">The string to check</param>
        /// <param name="bestMatch">The string that was best match</param>
        /// <returns>The value of the highest match (the highest string.IndexOf() value)</returns>
        public static int FindBestContainsMatch(this IEnumerable<string> items,
            string compareString)
        {
            string refValue = null;

            return FindBestContainsMatch(items, compareString, ref refValue);
        }

        /// <summary>
        /// Adds all of the vector3s together
        /// </summary>
        /// <param name="items">Collection of vector3s to be summed</param>
        /// <returns>The sum of all of the vector3s</returns>
        public static Vector3 Sum(this IEnumerable<Vector3> items)
        {
            return items.Aggregate((a, b) => a + b);
        }

        /// <summary>
        /// Gets the average of the collection of vector3s
        /// </summary>
        /// <param name="items">The collection of vector3's to get the average of</param>
        /// <returns>The vector3 average</returns>
        public static Vector3 Average(this IEnumerable<Vector3> items)
        {
            return items.Sum() / items.Count();            
        }

        /// <summary>
        /// Adds all of the vector3s together
        /// </summary>
        /// <param name="items">Collection of vector3s to be summed</param>
        /// <returns>The sum of all of the vector3s</returns>
        public static Vector3Int Sum(this IEnumerable<Vector3Int> items)
        {
            return items.Aggregate((a, b) => a + b);
        }

        /// <summary>
        /// Gets the average of the collection of vector3s
        /// </summary>
        /// <param name="items">The collection of vector3's to get the average of</param>
        /// <returns>The vector3 average</returns>
        public static Vector3Int Average(this IEnumerable<Vector3Int> items)
        {
            return items.Sum().SafeDivide(items.Count());
        }

        /// <summary>
        /// Determines if the provided collection has any duplicate values
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection</typeparam>
        /// <param name="items">The collection of items to determine duplicates within</param>
        /// <returns>True if it has duplicates, false if not</returns>
        public static bool HasDuplicates<T>(this IEnumerable<T> items, IEqualityComparer<T> comparer = null)
        {
            IEnumerable<T> distinct = comparer == null ? items.Distinct() : items.Distinct(comparer);
            return items.Count() != distinct.Count();
        }
    }
}