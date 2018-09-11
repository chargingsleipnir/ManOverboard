using System.Collections.Generic;

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

    }
}