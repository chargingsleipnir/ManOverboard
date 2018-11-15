using System.Collections.Generic;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Extensions for the HashSet<T>
    /// </summary>
    public static class HashSetExtensions
    {
        /// <summary>
        /// Adds a range of items to the hashset
        /// </summary>
        /// <typeparam name="T">The type contained by the collection</typeparam>
        /// <param name="thisHashSet">The hashset to add to</param>
        /// <param name="newValues">New values to be added</param>
        /// <returns>The number of items successfully added</returns>
        public static int AddRange<T>(this HashSet<T> thisHashSet, IEnumerable<T> newValues)
        {
            int itemsAdded = 0;

            foreach (T newValue in newValues)
            {
                if (thisHashSet.Add(newValue))
                    itemsAdded++;
            }

            return itemsAdded;
        }
    }
}