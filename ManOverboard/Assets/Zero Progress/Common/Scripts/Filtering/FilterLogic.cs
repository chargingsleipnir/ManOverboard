using System.Collections.Generic;
using System.Linq;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Helper class that provides logic for filtering through a static call as
    /// well as providing extension methods for the IFilter type
    /// </summary>
    public static class FilterLogic
    {
        /// <summary>
        /// Extension method for the IFilter interface that collects all the valid
        /// items from the provided collection
        /// </summary>
        /// <typeparam name="T">The type that the filter is used for</typeparam>
        /// <param name="Filter">The filter to be applied to the collection</param>
        /// <param name="CollectionToFilter">The collection to find all the valid items in</param>
        /// <returns>A collection of found valid items, or an empty collection if none were valid</returns>
        public static IEnumerable<T> FilterCollection<T>(this IFilter<T> Filter, IEnumerable<T> CollectionToFilter)
        {
            List<T> validItems = new List<T>();

            foreach (T item in CollectionToFilter)
            {
                if (Filter.IsValidItem(item))
                    validItems.Add(item);
            }

            return validItems;
        }

        /// <summary>
        /// Helper to determine an items validity using blacklist/whitelist logic
        /// </summary>
        /// <typeparam name="T">The type that the filter is used for</typeparam>
        /// <param name="Item">The item to determine validity for</param>
        /// <param name="FilterItems">The items that the item will be compared against to determine if 
        /// it is a match or not (i.e. if type is string and this is using blacklist functionality, this
        /// would be the list of all the strings that aren't allowed)</param>
        /// <param name="IsBlacklist">True to use blacklist comparison, false to use whitelist comparison</param>
        /// <returns>True if the item is permitted, false if not</returns>
        public static bool IsValidItem<T>(T Item, IEnumerable<T> FilterItems, bool IsBlacklist = true)
        {
            if (IsBlacklist)
                return !FilterItems.Contains(Item);
            else
                return FilterItems.Contains(Item);
        }
    }
}
