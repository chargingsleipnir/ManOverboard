using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;

namespace ZeroProgress.SceneManagementUtility
{
    /// <summary>
    /// Collection of GUIStyles, Textures, Names, etc that the SceneManagementUtility
    /// depends upon
    /// </summary>
    public static class SceneManagerResources
    {
        #region Paths and Names

        /// <summary>
        /// Base path for built-in resources
        /// </summary>
        public const string ResourcesBasePath = "ZeroProgress/SceneNodeEditor/";

        /// <summary>
        /// Name of style to provide an outline box, generally used with 
        /// BeginHorizontal/Vertical
        /// </summary>
        public const string SectionOutlineStyleName = "scene-section-outline";

        /// <summary>
        /// Name of style for scene nodes that are currently unlocked
        /// </summary>
        public const string SceneNodeOpenStyleName = "scene-node-unlocked";

        /// <summary>
        /// Name of style for scene nodes that are currently locked
        /// </summary>
        public const string SceneNodeClosedStyleName = "scene-node-locked";

        /// <summary>
        /// Name of the style for the toggle that indicates if a variable should be reset
        /// after each transition
        /// </summary>
        public const string RefreshOnTransitionStyleName = "refresh-on-transition";

        /// <summary>
        /// Name of the style used for displaying the Transition Name
        /// </summary>
        public const string TransitionNameStyleName = "scene-transition-name";

        /// <summary>
        /// Name of the style used for the icon that pulses during a transition
        /// </summary>
        public const string TransitionIconStyleName = "scene-transition-icon";

        /// <summary>
        /// Name of the style used to display instructions on the scene hunter editor
        /// </summary>
        public const string SceneHunterInstrucStyleName = "scene-hunter-instructions";

        #endregion

        #region Colors

        /// <summary>
        /// Color for the scene that is currently loaded and playing
        /// </summary>
        public static Color ActiveSceneColor = Color.green;

        /// <summary>
        /// Color for the Any Scene node
        /// </summary>
        public static Color AnySceneColor = new Color32(34, 207, 189, 255);

        /// <summary>
        /// Color for the Entry Scene node
        /// </summary>
        public static Color EntrySceneColor = new Color32(246, 153, 46, 255);

        #endregion

        #region Textures

        private static Texture2D clapperboardOpenTex;

        /// <summary>
        /// Clapperboard open normal texture
        /// </summary>
        public static Texture2D ClapperboardOpenTex
        {
            get
            {
                if (clapperboardOpenTex == null)
                    clapperboardOpenTex = LoadResource<Texture2D>("clapperboard-open");

                return clapperboardOpenTex;
            }
        }

        private static Texture2D clapperboardOpenActiveTex;

        /// <summary>
        /// Clapperboard open with highlight texture
        /// </summary>
        public static Texture2D ClapperboardOpenActiveTex
        {
            get
            {
                if (clapperboardOpenActiveTex == null)
                    clapperboardOpenActiveTex = LoadResource<Texture2D>("clapperboard-open-highlighted");

                return clapperboardOpenActiveTex;
            }
        }

        private static Texture2D clapperboardCloseTex;

        /// <summary>
        /// Clapperboard closed normal texture
        /// </summary>
        public static Texture2D ClapperboardCloseTex
        {
            get
            {
                if (clapperboardCloseTex == null)
                    clapperboardCloseTex = LoadResource<Texture2D>("clapperboard-close");

                return clapperboardCloseTex;
            }
        }

        private static Texture2D clapperboardCloseActiveTex;

        /// <summary>
        /// Clapperboard closed with highlight texture
        /// </summary>
        public static Texture2D ClapperboardCloseActiveTex
        {
            get
            {
                if (clapperboardCloseActiveTex == null)
                    clapperboardCloseActiveTex = LoadResource<Texture2D>("clapperboard-close-highlighted");
                
                return clapperboardCloseActiveTex;
            }
        }

        private static Texture2D transitionRefreshOnTex;

        /// <summary>
        /// Image for when a refresh on transition behaviour is enabled (SceneVariables)
        /// </summary>
        public static Texture2D TransitionRefreshOnTex
        {
            get
            {
                if (transitionRefreshOnTex == null)
                    transitionRefreshOnTex = LoadResource<Texture2D>("refresh-icon-on");

                return transitionRefreshOnTex;
            }
        }

        private static Texture2D transitionRefreshOffTex;

        /// <summary>
        /// Image for when a refresh on transition behaviour is disabled (SceneVariables)
        /// </summary>
        public static Texture2D TransitionRefreshOffTex
        {
            get
            {
                if (transitionRefreshOffTex == null)
                    transitionRefreshOffTex = LoadResource<Texture2D>("refresh-icon-off");

                return transitionRefreshOffTex;
            }
        }

        private static Texture2D transitionPulseIcon;

        /// <summary>
        /// Icon for the pulse image that is displayed during a transition
        /// </summary>
        public static Texture2D TransitionPulseIcon
        {
            get
            {
                if (transitionPulseIcon == null)
                    transitionPulseIcon = LoadResource<Texture2D>("transition-icon");

                return transitionPulseIcon;
            }
        }

        private static Texture2D variablesListIcon;

        /// <summary>
        /// Icon for the toolbar for the variables list items
        /// </summary>
        public static Texture2D VariablesListIcon
        {
            get
            {
                if (variablesListIcon == null)
                    variablesListIcon = LoadResource<Texture2D>("variables-list-icon");

                return variablesListIcon;
            }
        }

        private static Texture2D sceneOrderIcon;

        /// <summary>
        /// Icon for the toolbar for the organize scenes items
        /// </summary>
        public static Texture2D SceneOrderIcon
        {
            get
            {
                if (sceneOrderIcon == null)
                    sceneOrderIcon = LoadResource<Texture2D>("organize-scenes-icon");

                return sceneOrderIcon;
            }
        }
        
        private static Texture2D sceneImportIcon;

        /// <summary>
        /// Icon for the toolbar for the import scenes items
        /// </summary>
        public static Texture2D SceneImportIcon
        {
            get
            {
                if (sceneImportIcon == null)
                    sceneImportIcon = LoadResource<Texture2D>("import-icon");

                return sceneImportIcon;
            }
        }

        #endregion

        #region Styles

        private static GUIStyle sectionOutlineStyle;

        /// <summary>
        /// An outline for sections, such as each tab
        /// </summary>
        public static GUIStyle SectionOutlineStyle
        {
            get
            {
                GUIStyle existing = GUI.skin.GetCustomStyle(SectionOutlineStyleName);

                if (existing == null)
                {
                    if (sectionOutlineStyle == null)
                    {
                        sectionOutlineStyle = new GUIStyle(GUI.skin.box);
                        sectionOutlineStyle.margin = new RectOffset(2, 2, 2, 2);
                        sectionOutlineStyle.name = SectionOutlineStyleName;
                    }

                    existing = sectionOutlineStyle;

                }
                return existing;
            }
        }

        private static GUIStyle sceneNodeOpenStyle;

        /// <summary>
        /// Retrieve the GUIStyle for when the scene node is unlocked
        /// </summary>
        public static GUIStyle SceneNodeOpenStyle
        {
            get
            {
                GUIStyle existing = GUI.skin.GetCustomStyle(SceneNodeOpenStyleName);

                if (existing == null)
                {
                    if (sceneNodeOpenStyle == null)
                    {
                        sceneNodeOpenStyle = new GUIStyle(GUI.skin.box);
                        sceneNodeOpenStyle.normal.background = ClapperboardOpenTex;
                        sceneNodeOpenStyle.normal.textColor = Color.white;

                        sceneNodeOpenStyle.onActive.background = ClapperboardOpenActiveTex;
                        sceneNodeOpenStyle.onActive.textColor = Color.white;

                        sceneNodeOpenStyle.border = new RectOffset(12, 12, 12, 12);
                        sceneNodeOpenStyle.padding = new RectOffset(75, 5, 5, 5);
                        sceneNodeOpenStyle.fixedWidth = 250f;
                        sceneNodeOpenStyle.fixedHeight = 200f;

                        sceneNodeOpenStyle.fontSize = 24;
                        sceneNodeOpenStyle.alignment = TextAnchor.MiddleCenter;
                        sceneNodeOpenStyle.wordWrap = true;

                        sceneNodeOpenStyle.name = SceneNodeOpenStyleName;
                    }

                    existing = sceneNodeOpenStyle;
                }
                return existing;
            }
        }

        private static GUIStyle sceneNodeCloseStyle;

        /// <summary>
        /// Retrieve the GUI Style for when a scene node is locked
        /// </summary>
        public static GUIStyle SceneNodeCloseStyle
        {
            get
            {
                GUIStyle existing = GUI.skin.GetCustomStyle(SceneNodeClosedStyleName);

                if (existing == null)
                {
                    if (sceneNodeCloseStyle == null)
                    {
                        sceneNodeCloseStyle = new GUIStyle(GUI.skin.box);
                        sceneNodeCloseStyle.normal.background = ClapperboardCloseTex;
                        sceneNodeCloseStyle.normal.textColor = Color.white;

                        sceneNodeCloseStyle.onActive.background = ClapperboardCloseActiveTex;
                        sceneNodeCloseStyle.onActive.textColor = Color.white;

                        sceneNodeCloseStyle.border = new RectOffset(12, 12, 12, 12);
                        sceneNodeCloseStyle.padding = new RectOffset(75, 5, 5, 5);
                        sceneNodeCloseStyle.fixedWidth = 250f;
                        sceneNodeCloseStyle.fixedHeight = 150f;

                        sceneNodeCloseStyle.fontSize = 24;
                        sceneNodeCloseStyle.alignment = TextAnchor.MiddleCenter;
                        sceneNodeCloseStyle.wordWrap = true;

                        sceneNodeCloseStyle.name = SceneNodeClosedStyleName;
                    }

                    existing = sceneNodeCloseStyle;
                }
                return existing;
            }
        }

        private static GUIStyle refreshOnTransitionStyle;

        /// <summary>
        /// GUI Style for the toggle that indicates if a scene variable should be reset
        /// after a scene transition
        /// </summary>
        public static GUIStyle RefreshOnTransitionStyle
        {
            get
            {
                GUIStyle existing = GUI.skin.GetCustomStyle(RefreshOnTransitionStyleName);

                if (existing == null)
                {
                    if (refreshOnTransitionStyle == null)
                    {
                        refreshOnTransitionStyle = new GUIStyle(GUIStyle.none);

                        refreshOnTransitionStyle.normal.background = TransitionRefreshOffTex;
                        refreshOnTransitionStyle.onNormal.background = TransitionRefreshOnTex;
                    }

                    existing = refreshOnTransitionStyle;
                }
                return existing;
            }
        }

        private static GUIStyle transitionNameStyle;

        /// <summary>
        /// GUIStyle used for the Transition Name textfield
        /// </summary>
        public static GUIStyle TransitionNameStyle
        {
            get
            {
                GUIStyle existing = GUI.skin.GetCustomStyle(TransitionNameStyleName);

                if (existing == null)
                {
                    if (transitionNameStyle == null)
                    {
                        transitionNameStyle = new GUIStyle(GUI.skin.textField);
                        transitionNameStyle.margin = new RectOffset(5, 5, 5, 5);

                        transitionNameStyle.name = TransitionNameStyleName;
                    }

                    existing = transitionNameStyle;
                }
                return existing;
            }
        }

        private static GUIStyle transitionIconStyle;

        /// <summary>
        /// Style for the icon that pulses during a scene transition
        /// </summary>
        public static GUIStyle TransitionIconStyle
        {
            get
            {
                GUIStyle existing = GUI.skin.GetCustomStyle(TransitionIconStyleName);

                if (existing == null)
                {
                    if (transitionIconStyle == null)
                    {
                        transitionIconStyle = new GUIStyle(GUI.skin.box);

                        transitionIconStyle.normal.background = TransitionPulseIcon;
                        transitionIconStyle.fixedWidth = 25f;
                        transitionIconStyle.fixedHeight = 25f;

                        transitionIconStyle.name = TransitionIconStyleName;
                    }

                    existing = transitionIconStyle;
                }

                return existing;
            }
        }

        private static GUIStyle sceneHunterInstrucStyle;

        /// <summary>
        /// Style for the instructions on the scene hunter
        /// </summary>
        public static GUIStyle SceneHunterInstrucStyle
        {
            get
            {
                GUIStyle existing = GUI.skin.GetCustomStyle(SceneHunterInstrucStyleName);

                if (existing == null)
                {
                    if (sceneHunterInstrucStyle == null)
                    {
                        sceneHunterInstrucStyle = new GUIStyle(GUI.skin.box);

                        sceneHunterInstrucStyle.wordWrap = true;
                        sceneHunterInstrucStyle.border = new RectOffset(2, 2, 2, 2);

                        sceneHunterInstrucStyle.normal.textColor = GUI.skin.label.normal.textColor;
                    }

                    existing = sceneHunterInstrucStyle;
                }

                return existing;
            }
        }


        #endregion

        /// <summary>
        /// Helper to call Resources.Load with the <cref>ResourcesBasePath</cref> 
        /// added to the provided relative path
        /// </summary>
        /// <typeparam name="T">The type of resource to load</typeparam>
        /// <param name="relativePath">The path to the item without the <cref>ResourcesBasePath</cref></param>
        /// <returns></returns>
        public static T LoadResource<T>(string relativePath) 
            where T:UnityEngine.Object
        {
            return Resources.Load<T>(ResourcesBasePath + relativePath);
        }
    }
}