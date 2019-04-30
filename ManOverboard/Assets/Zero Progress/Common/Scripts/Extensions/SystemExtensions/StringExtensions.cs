using System;
namespace ZeroProgress.Common
{
    /// <summary>
    /// Utilities extending the string class
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Checks a string for containment with the option of specifying case-insensitivity
        /// </summary>
        /// <param name="SourceText">The text to be evaluated</param>
        /// <param name="CompareText">The text to find within the string</param>
        /// <param name="ComparisonMode">The string comparison mode to be utilized</param>
        /// <returns>True if the string contains the specified text, false if not</returns>
        public static bool Contains(this string SourceText, string CompareText, StringComparison ComparisonMode = StringComparison.OrdinalIgnoreCase)
        {
            if (SourceText == null)
                return false;

            return SourceText.IndexOf(CompareText, ComparisonMode) >= 0;
        }
    }
}