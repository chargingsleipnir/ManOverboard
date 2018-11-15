using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// GUIStyles, texts, and resources that are commonly used across Zero Progress projects
    /// </summary>
    public static class ZPCommonResources
    {
        #region Paths and Names

        /// <summary>
        /// Name of the style for simple solid backgrounds
        /// </summary>
        public const string SimpleSolidBackgroundStyleName = "simple-solid-background";

        #endregion

        #region Colors

        /// <summary>
        /// Background color for reorderable list items that have an even index in the non-pro version of Unity
        /// </summary>
        private static Color evenElementBgColorNonPro = new Color32(156, 156, 156, 255);

        /// <summary>
        /// Background color for reorderable list items that have an odd index in the non-pro version of Unity
        /// </summary>
        private static Color oddElementBgColorNonPro = new Color32(216, 216, 216, 255);

        /// <summary>
        /// Background color for reorderable list items that have an even index in the pro version of Unity
        /// </summary>
        private static Color evenElementBgColorPro = new Color32(56, 56, 56, 255);

        /// <summary>
        /// Background color for reorderable list items that have an odd index in the pro version of Unity
        /// </summary>
        private static Color oddElementBgColorPro = new Color32(80, 80, 80, 255);

        /// <summary>
        /// Get the background color used for even index items
        /// </summary>
        /// <param name="isPro">True if using pro Unity, false if not</param>
        /// <returns>The corresponding color</returns>
        public static Color GetEvenElementBgColor(bool isPro)
        {
            if (isPro)
                return evenElementBgColorPro;
            else
                return evenElementBgColorNonPro;
        }

        /// <summary>
        /// Get the background color used for odd index items
        /// </summary>
        /// <param name="isPro">True if using pro Unity, false if not</param>
        /// <returns>The corresponding color</returns>
        public static Color GetOddElementBgColor(bool isPro)
        {
            if (isPro)
                return oddElementBgColorPro;
            else
                return oddElementBgColorNonPro;
        }

        /// <summary>
        /// Get the background color for the element of the provided index
        /// </summary>
        /// <param name="index">The index of the element</param>
        /// <param name="isPro">True if using pro Unity, false if not</param>
        /// <returns>The corresponding color</returns>
        public static Color GetElementBgColor(int index, bool isPro)
        {
            if (index % 2 == 0)
                return GetEvenElementBgColor(isPro);
            else
                return GetOddElementBgColor(isPro);
        }

        #endregion

        #region GUIStyles

        private static GUIStyle simpleSolidBackground;

        /// <summary>
        /// GUIStyle for a simple background that uses a single color
        /// (uses white texture by default, so requires tinting by GUI.color)
        /// </summary>
        public static GUIStyle SimpleSolidBackground
        {
            get
            {
                GUIStyle existing = GUI.skin.GetCustomStyle(SimpleSolidBackgroundStyleName);

                if (existing == null)
                {
                    if (simpleSolidBackground == null)
                    {
                        simpleSolidBackground = new GUIStyle(GUI.skin.box);
                        simpleSolidBackground.normal.background = Texture2D.whiteTexture;

                        simpleSolidBackground.name = SimpleSolidBackgroundStyleName;
                    }

                    existing = simpleSolidBackground;
                }

                return existing;
            }
        }
        
        #endregion
    }
}