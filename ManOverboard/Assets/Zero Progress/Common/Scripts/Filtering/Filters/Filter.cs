using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// A base class for filters to implement for custom logic
    /// </summary>
    /// <typeparam name="T">The type that this filter applies to</typeparam>
    [Serializable]
    public class Filter<T> : IFilter<T>
    {
        [Tooltip("True if the FilterItems collection should be treated as a listing of items" +
            "that result in an invalid match, false if they should result in a valid match")]
        public bool IsBlacklist = true;

        [SerializeField]
        [Tooltip("The collection of items that represent valid or invalid items, depending" +
            "on whether or not this collection is treated as a blacklist or not")]
        protected List<T> FilterItems = new List<T>(); 

        /// <summary>
        /// Determines if the passed item passes the filter criteria or not
        /// </summary>
        /// <param name="Item">The item to have validity determined for</param>
        /// <returns>True if the passed item is valid, false if not</returns>
        public virtual bool IsValidItem(T Item)
        {
            return FilterLogic.IsValidItem<T>(Item, FilterItems, IsBlacklist);
        }
    }
}