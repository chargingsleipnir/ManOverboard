using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ZeroProgress.Common
{
    public static class GUIExtensions
    {
        /// <summary>
        /// Constant representing an editor windows' tab to be accounted for during clipping
        /// </summary>
        private const float kEditorWindowTabHeight = 21.0f;

        public static readonly GUIStyle HorizontalLineStyle = new GUIStyle()
        {
            normal = new GUIStyleState() { background = Texture2D.whiteTexture },
            margin = new RectOffset(0, 0, 1, 1),
            fixedHeight = 1f
        };

        public static readonly GUIStyle VerticalLineStyle = new GUIStyle(HorizontalLineStyle)
        {
            fixedHeight = 0f,
            fixedWidth = 1f
        };

        private static Stack<Color> colorStack = new Stack<Color>();

        private static Stack<Color> backgroundColorStack = new Stack<Color>();

        /// <summary>
        /// The last GUI matrices used
        /// </summary>
        private static Stack<Matrix4x4> previousGUIMatrices = new Stack<Matrix4x4>();

        /// <summary>
        /// Draws a line horizontally
        /// </summary>
        /// <param name="rect">The rectangle that displays the line</param>
        /// <param name="lineColour">The line colour</param>
        /// <param name="style">The style to draw with. Defaults to LineStyle if none provided</param>
        public static void DrawHorizontalLine(Rect rect, Color lineColour, GUIStyle style = null)
        {
            if (style == null)
                style = HorizontalLineStyle;

            DrawWithColour(lineColour, () =>
            {
                GUI.Box(rect, "", style);
            });
        }

        /// <summary>
        /// Draws a line vertically
        /// </summary>
        /// <param name="rect">The rectangle that displays the line</param>
        /// <param name="lineColour">The line colour</param>
        /// <param name="style">The style to draw with. Defaults to LineStyle if none provided</param>
        public static void DrawVerticalLine(Rect rect, Color lineColour, GUIStyle style = null)
        {
            DrawHorizontalLine(rect, lineColour, VerticalLineStyle);
        }

        /// <summary>
        /// Applies the specified color to all rendering actions within the render action
        /// and then restores the original GUI.color
        /// </summary>
        /// <param name="colorToApply">The color to use</param>
        /// <param name="renderAction">Drawings taken after setting color</param>
        public static void DrawWithColour(Color colorToApply, System.Action renderAction)
        {
            Color originalColor = GUI.color;

            GUI.color = colorToApply;

            renderAction();

            GUI.color = originalColor;
        }

        /// <summary>
        /// Sets the current GUI.color and stores the original for restore
        /// after EndColouring is called
        /// </summary>
        /// <param name="color">The color to apply</param>
        public static void BeginColouring(Color color)
        {
            colorStack.Push(GUI.color);

            GUI.color = color;
        }

        /// <summary>
        /// Sets the current GUI.backgroundColor and stores the original for restore
        /// after EndBackgroundColouring is called
        /// </summary>
        /// <param name="color">The color to apply</param>
        public static void BeginBackgroundColouring(Color color)
        {
            backgroundColorStack.Push(GUI.backgroundColor);

            GUI.backgroundColor = color;
        }

        /// <summary>
        /// Restores the original GUI.color before BeginColouring was called
        /// </summary>
        public static void EndColouring()
        {
            if (colorStack.Count == 0)
                throw new System.InvalidOperationException("EndColouring unmatched with a BeginColouring call");

            GUI.color = colorStack.Pop();
        }

        /// <summary>
        /// Restores the original GUI.backgroundColor before 
        /// BeginBackgroundColouring was called
        /// </summary>
        public static void EndBackgroundColouring()
        {
            if(backgroundColorStack.Count == 0)
                throw new System.InvalidOperationException("EndBackgroundColouring unmatched with a BeginColouring call");
            
            GUI.backgroundColor = backgroundColorStack.Pop();
        }

        /// <summary>
        /// Creates a simple coloured box with the dimensions of the
        /// provided rectangle to act like a background
        /// </summary>
        /// <param name="boxRect">The rect to render at</param>
        /// <param name="color">The color to apply</param>
        /// <param name="boxStyle">Style to apply</param>
        public static void ColouredBox(Rect boxRect, Color color, 
            GUIContent content, GUIStyle boxStyle = null)
        {
            BeginColouring(color);

            if (boxStyle == null)
                boxStyle = GUI.skin.box;

            if (content == null)
                content = GUIContent.none;

            GUI.Box(boxRect, content, boxStyle);

            EndColouring();
        }

        public static void ColouredBox(Rect boxRect, Color color, GUIStyle boxStyle)
        {
            ColouredBox(boxRect, color, GUIContent.none, boxStyle);
        }

        /// <summary>
        /// Starts an area with a zoom feature
        /// </summary>
        /// <param name="zoomScale">The amount of scaling to apply</param>
        /// <param name="screenCoordsArea">The rectangle defining the zoomable area</param>
        /// <returns>A rect depicting the zoom</returns>
        public static Rect BeginZoomableArea(float zoomScale, Rect screenCoordsArea)
        {
            GUI.EndGroup();        // End the group Unity begins automatically for an EditorWindow to clip out the window tab. This allows us to draw outside of the size of the EditorWindow.

            Rect clippedArea = screenCoordsArea.ScaleSizeBy(1.0f / zoomScale, screenCoordsArea.TopLeftGUI());
            clippedArea.y += kEditorWindowTabHeight;
            GUI.BeginGroup(clippedArea);

            previousGUIMatrices.Push(GUI.matrix);

            Matrix4x4 translation = Matrix4x4.TRS(clippedArea.TopLeftGUI(), Quaternion.identity, Vector3.one);
            Matrix4x4 scale = Matrix4x4.Scale(new Vector3(zoomScale, zoomScale, 1.0f));
            GUI.matrix = translation * scale * translation.inverse * GUI.matrix;

            return clippedArea;
        }

        /// <summary>
        /// Ends an area with a zoom feature
        /// </summary>
        public static void EndZoomableArea()
        {
            GUI.matrix = previousGUIMatrices.Pop();
            GUI.EndGroup();
            GUI.BeginGroup(new Rect(0.0f, kEditorWindowTabHeight, Screen.width, Screen.height));
        }
        
        /// <summary>
        /// Draws a grid in the specified area
        /// </summary>
        /// <param name="gridArea">The area to make the grid in</param>
        /// <param name="cellSize">The size of the cells</param>
        /// <param name="lineColor">The colour of the grid lines</param>
        /// <param name="lineAlpha">The alpha of the lines</param>
        public static void DrawGrid(Rect gridArea, float cellSize, Color lineColor, float lineAlpha)
        {
            DrawGrid(gridArea, new Vector2(cellSize, cellSize), lineColor, lineAlpha);
        }

        /// <summary>
        /// Draws a grid in the specified area
        /// </summary>
        /// <param name="gridArea">The area to make the grid in</param>
        /// <param name="cellSize">The size of the cells</param>
        /// <param name="lineColor">The colour of the grid lines</param>
        /// <param name="lineAlpha">The alpha of the lines</param>
        public static void DrawGrid(Rect gridArea, Vector2 cellSize, Color lineColor, float lineAlpha)
        {
            int widthDivs = Mathf.CeilToInt(gridArea.width / cellSize.x);
            int heightDivs = Mathf.CeilToInt(gridArea.height / cellSize.y);

            GLExtensions.BeginGUIDrawLinesZDepthOff(new Color(lineColor.r, lineColor.g, lineColor.b, lineAlpha));

            Vector3 offset = gridArea.position;
            Vector3 newOffset = new Vector3(offset.x % cellSize.x, offset.y % cellSize.y, 0f);
            offset = newOffset;
            newOffset.y = 0f;

            for (int i = 0; i < widthDivs; i++)
            {
                Vector3 a = new Vector3(cellSize.x * i, 0f, 0f) + newOffset;
                if (a.x > gridArea.width || a.x < 0f)
                    continue;

                GLExtensions.AddLineVertex(a);
                GLExtensions.AddLineVertex(new Vector3(cellSize.x * i, gridArea.height, 0f) + newOffset);
            }

            newOffset.y = offset.y;
            newOffset.x = 0f;

            for (int j = 0; j < heightDivs; j++)
            {
                Vector3 a = new Vector3(0f, cellSize.y * j, 0f) + newOffset;
                if (a.y > gridArea.height || a.y < 0f)
                    continue;
                GLExtensions.AddLineVertex(a);
                GLExtensions.AddLineVertex(new Vector3(gridArea.width, cellSize.y * j, 0f) + newOffset);
            }

            GLExtensions.EndDraw();
        }
    }
}