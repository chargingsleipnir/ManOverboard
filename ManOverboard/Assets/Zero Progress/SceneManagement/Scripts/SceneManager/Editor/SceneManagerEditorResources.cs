using UnityEditor;
using UnityEngine;
using ZeroProgress.Common;

namespace ZeroProgress.SceneManagementUtility.Editors
{
    /// <summary>
    /// Resources for the Scene Manager editors that require the
    /// Editor namespace
    /// </summary>
    public static class SceneManagerEditorResources
    {
        #region Paths and Names

        /// <summary>
        /// Name of the style for the visibility icon for showing the tabbed window
        /// </summary>
        public const string ShowTabsIconStyle = "scene-show-tabs-window";

        /// <summary>
        /// Name of the style for the visibility icon for hiding the tabbed window
        /// </summary>
        public const string HideTabsIconStyle = "scene-hide-tabs-window";

        #endregion

        #region Textures

        private static Texture2D eyeOnTexture;

        /// <summary>
        /// Texture that shows an open eye, used to indicate visibility of an item
        /// </summary>
        public static Texture2D EyeOnTexture
        {
            get
            {
                if (eyeOnTexture == null)
                    eyeOnTexture = SceneManagerResources.LoadResource<Texture2D>("eye-on");

                return eyeOnTexture;
            }
        }

        private static Texture2D eyeOffTexture;

        /// <summary>
        /// Texture that shows a closed eye, used to indicate lack of visibility of an item
        /// </summary>
        public static Texture2D EyeOffTexture
        {
            get
            {
                if (eyeOffTexture == null)
                    eyeOffTexture = SceneManagerResources.LoadResource<Texture2D>("eye-off");

                return eyeOffTexture;
            }
        }

        #endregion

        #region GUIContent

        private static GUIContent eyeOnContent;

        /// <summary>
        /// GUIContent of the EyeOn Texture used for the toggling of the tabbed sidebar
        /// </summary>
        public static GUIContent EyeOnContent
        {
            get
            {
                if (eyeOnContent == null)
                    eyeOnContent = new GUIContent(EyeOnTexture, "Hide the sidebar");

                return eyeOnContent;
            }
        }

        private static GUIContent eyeOffContent;

        /// <summary>
        /// GUIContent of the EyeOff Texture used for the toggling of the tabbed sidebar
        /// </summary>
        public static GUIContent EyeOffContent
        {
            get
            {
                if (eyeOffContent == null)
                    eyeOffContent = new GUIContent(EyeOffTexture, "Show the sidebar");

                return eyeOffContent;
            }
        }

        #endregion

        #region Styles

        /// <summary>
        /// The style for the eye-icon used to toggle visibility of the tabbed sidebar
        /// This is the open-eye style
        /// </summary>
        private static GUIStyle showTabsIconStyle;

        /// <summary>
        /// The style for the eye-icon used to toggle visibility of the tabbed sidebar
        /// This is the closed-eye style
        /// </summary>
        private static GUIStyle hideTabsIconStyle;

        #endregion

        /// <summary>
        /// Gets the style used to render the icon used to determine
        /// the side-bar tab window
        /// </summary>
        /// <param name="getShowIcon">True to get the show tab icon, false to
        /// get the hide tab icon</param>
        /// <returns>The GUI Style for the icon</returns>
        public static GUIStyle GetTabsVisibilityIconStyle(bool getShowIcon)
        {
            GUIStyle returnStyle;

            if (getShowIcon)
            {
                returnStyle = GUI.skin.GetCustomStyle(ShowTabsIconStyle);

                if (returnStyle == null)
                {
                    if (showTabsIconStyle == null)
                    {
                        showTabsIconStyle = new GUIStyle(EditorStyles.toolbarButton);
                        showTabsIconStyle.padding = new RectOffset(2, 2, 4, 4);
                    }

                    returnStyle = showTabsIconStyle;
                }
            }
            else
            {
                returnStyle = GUI.skin.GetCustomStyle(HideTabsIconStyle);

                if (returnStyle == null)
                {
                    if (hideTabsIconStyle == null)
                    {
                        hideTabsIconStyle = new GUIStyle(EditorStyles.toolbarButton);
                        hideTabsIconStyle.padding = new RectOffset(2, 2, 4, 4);
                    }

                    returnStyle = hideTabsIconStyle;
                }
            }

            return returnStyle;
        }
    }
}