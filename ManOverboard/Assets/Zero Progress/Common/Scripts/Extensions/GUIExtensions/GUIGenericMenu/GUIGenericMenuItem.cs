using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Base class for menu items used in the Generic Menu made for GUI
    /// </summary>
    public abstract class GUIGenericMenuItem {

        /// <summary>
        /// The default style for menu items
        /// </summary>
        protected GUIStyle defaultMenuStyle;

        /// <summary>
        /// The style for the backgraound used to identify selected items
        /// </summary>
        protected GUIStyle selectedItemBackground;

        /// <summary>
        /// Padding applied to the menu items
        /// </summary>
        public static RectOffset NodePadding = new RectOffset(25, 5, 5, 5);

        /// <summary>
        /// The path that the menu item can be found at inside the menu
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// The text/image to be displayed. The text should always be set though,
        /// as there is some logic reliant on it
        /// </summary>
        public GUIContent Display;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">The path that the menu item can be found at inside the menu</param>
        /// <param name="content">The text/image to be displayed. The text should always be set though,
        /// as there is some logic reliant on it</param>
        public GUIGenericMenuItem(string path, GUIContent content)
        {
            Path = GetValidPath(path);
            Display = content;
        }

        /// <summary>
        /// To be implemented by children to handle what happens when this item is clicked
        /// </summary>
        /// <param name="context">The item that the menu was opened on</param>
        public abstract void Execute(System.Object context);

        /// <summary>
        /// Gets the size that the menu item requests for proper displaying
        /// </summary>
        /// <returns>The size to be reserved for this item</returns>
        public virtual Vector2 GetSize()
        {
            InitializeStyles();
            return defaultMenuStyle.CalcSize(Display);
        }
        
        /// <summary>
        /// Renders the menu item
        /// </summary>
        /// <param name="drawRect">The rectangle to be drawn within</param>
        /// <param name="isHovered">True if the item is currently hovered, false if not</param>
        /// <param name="menuContext">The item that the menu was opened on</param>
        public virtual void Draw(Rect drawRect, bool isHovered, System.Object menuContext)
        {
            InitializeStyles();

            if (isHovered)
                GUI.Box(drawRect, "", selectedItemBackground);

            GUI.Label(drawRect, Display, defaultMenuStyle);
        }
        
        /// <summary>
        /// Transforms the provided path to be an acceptable menu path
        /// </summary>
        /// <param name="path">The path to validate</param>
        /// <returns>Menu-valid path string</returns>
        protected string GetValidPath(string path)
        {
            if (path == null)
                path = string.Empty;

            path = path.Trim();

            if (path.StartsWith("/"))
                path = path.Remove(0, 1);

            if (path.EndsWith("/"))
                path = path.Remove(path.Length - 1, 1);

            return path.Trim();
        }

        /// <summary>
        /// Prepares the styles used by this item
        /// </summary>
        protected virtual void InitializeStyles()
        {
            if (defaultMenuStyle == null)
            {
                defaultMenuStyle = new GUIStyle(GUI.skin.label);
                defaultMenuStyle.normal.textColor = Color.black;
            }

            defaultMenuStyle.padding = NodePadding;

            if (selectedItemBackground == null)
            {
                selectedItemBackground = new GUIStyle(GUI.skin.box);
                selectedItemBackground.normal.background =
                    Texture2DExtensions.MakeTexture(1, 1, new Color(0.57f, 0.78f, 0.96f, 1f));
            }
        }
    }
}