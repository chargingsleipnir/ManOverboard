using System;
using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// A filter for strings
    /// </summary>
    [Serializable]
    public class StringFilter : Filter<string>
    {
        [Tooltip("True to check that the evaluated string simply contains a filter " +
            "item, false to require equals")]
        public bool CheckContainsString = false;
        
        [Tooltip("How the strings should be compared")]
        public StringComparison ComparisonMode = StringComparison.OrdinalIgnoreCase;

        /// <summary>
        /// Determines if the passed item passes the filter criteria or not
        /// </summary>
        /// <param name="Item">The item to have validity determined for</param>
        /// <returns>True if the passed item is valid, false if not</returns>
        public override bool IsValidItem(string Item)
        {
            bool foundMatch = false;

            foreach (string filterValue in FilterItems)
            {
                if (CheckContainsString)
                {
                    if (Item.Contains(filterValue, ComparisonMode))
                    {
                        foundMatch = true;
                        break;
                    }
                }
                else
                {
                    if(Item.Equals(filterValue, ComparisonMode))
                    {
                        foundMatch = true;
                        break;
                    }
                }
            }

            if (IsBlacklist)
                return !foundMatch;
            else
                return foundMatch;
        }
    }
}