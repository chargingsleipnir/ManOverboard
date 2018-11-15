using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Extension methods for the GUI skin class
    /// </summary>
    public static class GUISkinExtensions
    {
        /// <summary>
        /// Adds a new custom skin to the list if it doesn't already exist
        /// (uses the style name to determine if it's been added)
        /// </summary>
        /// <param name="thisSkin">The skin to add the custom style to</param>
        /// <param name="customStyle">The style to add</param>
        /// <param name="overrideIfExist">Overrides the existing style if true, otherwise ignores the addition</param>
        public static void AddCustomStyle(this GUISkin thisSkin, GUIStyle customStyle, bool overrideIfExist = true)
        {
            int index = System.Array.FindIndex(thisSkin.customStyles, (x) => x.name == customStyle.name);

            if (index >= 0)
            {
                if (overrideIfExist)
                    thisSkin.customStyles[index] = customStyle;
            }
            else
            {
                List<GUIStyle> customs = new List<GUIStyle>(thisSkin.customStyles);

                customs.Add(customStyle);

                thisSkin.customStyles = customs.ToArray();
            }
        }        

        /// <summary>
        /// Search the custom styles stored in the provided skin for a style
        /// that matches the provided name
        /// </summary>
        /// <param name="thisSkin">The skin to search</param>
        /// <param name="customName">The name of the style to find</param>
        /// <param name="customStyle">The found style, or null if not found</param>
        /// <returns>True if found, false if not</returns>
        public static bool TryGetCustomStyle(this GUISkin thisSkin, string customName, out GUIStyle customStyle)
        {
            customStyle = thisSkin.GetCustomStyle(customName);

            return customStyle != null;
        }

        /// <summary>
        /// Gets the style at the provided name
        /// </summary>
        /// <param name="thisSkin">The skin to search</param>
        /// <param name="customName">The name of the custom style to retrieve</param>
        /// <returns>The gui style, or null if not found</returns>
        public static GUIStyle GetCustomStyle(this GUISkin thisSkin, string customName)
        {
            return Array.Find(thisSkin.customStyles,
                (x) => x.name.Equals(customName, StringComparison.OrdinalIgnoreCase));
        }
    }
}