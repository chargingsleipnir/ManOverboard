using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Renders a line as a separator between menu items
    /// </summary>
    public class GUIMenuItemSeparator : GUIGenericMenuItem
    {
        /// <summary>
        /// Total height of the rectangle for the separator
        /// </summary>
        public static float SeparatorBoxHeight = 8f;

        /// <summary>
        /// Color of the separator
        /// </summary>
        private static readonly Color separatorColor = new Color(0.4f, 0.4f, 0.4f, 0.4f);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">Where in the menu tree to place the separator</param>
        public GUIMenuItemSeparator(string path) 
            : base(path, new GUIContent(""))
        {
        }

        /// <summary>
        /// Gets the size of the separator
        /// </summary>
        /// <returns>The size to reserve for rendering</returns>
        public override Vector2 GetSize()
        {
            return new Vector2(GUIGenericMenu.MinWidth, SeparatorBoxHeight);
        }

        /// <summary>
        /// Renders the separator line
        /// </summary>
        /// <param name="drawRect">The rect to draw the separator within</param>
        /// <param name="isHovered">True if hovered, false if not</param>
        /// <param name="context">The object the menu was opened for</param>
        public override void Draw(Rect drawRect, bool isHovered, System.Object context)
        {
            GLExtensions.BeginGUIDrawLinesZDepthOff(separatorColor);

            GLExtensions.AddLineVertex(new Vector3(drawRect.xMin + NodePadding.left, drawRect.center.y));
            GLExtensions.AddLineVertex(new Vector3(drawRect.xMax, drawRect.center.y));

            GLExtensions.EndDraw();
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="context">The item that the menu was opened on</param>
        public override void Execute(object context)
        {
        }
    }
}