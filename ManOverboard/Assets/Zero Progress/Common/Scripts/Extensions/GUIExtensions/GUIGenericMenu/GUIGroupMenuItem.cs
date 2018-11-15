using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Represents a gui item with an arrow on the right side to show it's
    /// responsible for grouping other menu items
    /// </summary>
    public class GUIGroupMenuItem : GUIParamDelegateMenuItem
    {
        /// <summary>
        /// The texture used for the arrow
        /// </summary>
        private static Texture2D arrowTexture;

        /// <summary>
        /// Cache of the draw rectangle to be used for collision checks 
        /// </summary>
        public Rect PositionRect { get; protected set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">Path of the menu item</param>
        /// <param name="content">Text of the menu item itself</param>
        /// <param name="action">The action to be executed on click</param>
        public GUIGroupMenuItem(string path, GUIContent content, MenuItemAction action) 
            : base(path, content, action)
        {
        }

        /// <summary>
        /// Render the item
        /// </summary>
        /// <param name="drawRect">Where to be rendered</param>
        /// <param name="isHovered">True if the item is currently hovered, false if not</param>
        /// <param name="menuContext">The object the menu was opened on</param>
        public override void Draw(Rect drawRect, bool isHovered, System.Object menuContext)
        {
            PositionRect = drawRect;

            base.Draw(drawRect, isHovered, menuContext);

            DrawRightFacingArrow(drawRect);

            if (isHovered)
                Execute(menuContext);
        }

        /// <summary>
        /// Helper to draw the arrow
        /// </summary>
        /// <param name="rect">The rectangle of the menu item to be cordoned off</param>
        private void DrawRightFacingArrow(Rect rect)
        {
            InitializeTexture();

            Rect arrowRect = new Rect(rect);
            arrowRect.xMin = arrowRect.xMax - 20f;
            arrowRect.xMax -= 5f;
            arrowRect.y = arrowRect.center.y - 7.5f;
            arrowRect.height = 15f;

            GUI.DrawTexture(arrowRect, arrowTexture);
        }

        /// <summary>
        /// Initialize the arrow texture if necessary
        /// </summary>
        private void InitializeTexture()
        {
            if (arrowTexture == null)
                arrowTexture = Resources.Load<Texture2D>("ZeroProgress/expand-arrow-right");
        }
    }
}