using UnityEditor;
using UnityEngine;

namespace ZeroProgress.Common.Editors
{
    /// <summary>
    /// Simple storage utility to track current state
    /// of the split view
    /// </summary>
    public class SplitViewState
    {
        public bool IsResizing = false;

        public float SplitLocation = -1f;
    }

    /// <summary>
    /// Extension methods for EditorGUI
    /// </summary>
    public static class EditorGUIExtensions
    {
        #region Editor GUI Split View

        /// <summary>
        /// Draws a resizable split box separated vertically
        /// </summary>
        /// <param name="rect">The total rect to work with</param>
        /// <param name="leftRender">Callback for what to render in the
        /// left side</param>
        /// <param name="rightRender">Callback for what to render in the
        /// right side</param>
        /// <param name="startSplit">Where the intial split should begin (between 0 and 1)</param>
        /// <param name="separatorColor">The colour of the separator down the middle. Default
        /// is gray</param>
        /// <param name="separatorStyle">Any style to apply to the separator</param>
        public static void DrawVerticalSplitArea(Rect rect,
            System.Action<Rect> leftRender, System.Action<Rect> rightRender,
            float startSplit = 0.5f, Color? separatorColor = null, GUIStyle separatorStyle = null)
        {
            DrawSplitArea(rect, true, leftRender, rightRender, startSplit, 
                separatorColor, separatorStyle);
        }

        /// <summary>
        /// Draws a resizable split box separated horizontally
        /// </summary>
        /// <param name="rect">The total rect to work with</param>
        /// <param name="topRender">Callback for what to render in the
        /// left side</param>
        /// <param name="bottomRender">Callback for what to render in the
        /// right side</param>
        /// <param name="startSplit">Where the intial split should begin (between 0 and 1)</param>
        /// <param name="separatorColor">The colour of the separator down the middle. Default
        /// is gray</param>
        /// <param name="separatorStyle">Any style to apply to the separator</param>
        public static void DrawHorizontalSplitArea(Rect rect,
            System.Action<Rect> topRender, System.Action<Rect> bottomRender,
            float startSplit = 0.5f, Color? separatorColor = null, GUIStyle separatorStyle = null)
        {
            DrawSplitArea(rect, false, topRender, bottomRender, startSplit, 
                separatorColor, separatorStyle);
        }

        private static void DrawSplitArea(Rect rect, bool splitVertically, 
            System.Action<Rect> leftRender, System.Action<Rect> rightRender,
            float startSplit = 0.5f, Color? separatorColor = null, GUIStyle separatorStyle = null)
        {
            if (separatorColor == null)
                separatorColor = Color.gray;

            int controlId = GUIUtility.GetControlID(FocusType.Passive);

            SplitViewState state = (SplitViewState)GUIUtility.GetStateObject(
                typeof(SplitViewState), controlId);

            if (state.SplitLocation < 0f && rect.width != 1f)
                state.SplitLocation = rect.width * startSplit;

            float splitLocation = state.SplitLocation;

            Rect separatorRect;
            
            if(splitVertically)
                separatorRect = new Rect(rect.xMin + splitLocation, rect.yMin, 5f, rect.height);
            else
                separatorRect = new Rect(rect.xMin, rect.yMin + splitLocation, rect.width, 5f);

            switch (Event.current.GetTypeForControl(controlId))
            {
                case EventType.Repaint:

                    Rect rect1 = GetSplitRect1(rect, splitVertically, splitLocation);
                    Rect rect2 = GetSplitRect2(rect, splitVertically, splitLocation);

                    leftRender(rect1);

                    GUIExtensions.DrawVerticalLine(separatorRect,
                        separatorColor.Value, separatorStyle);

                    EditorGUIUtility.AddCursorRect(separatorRect, MouseCursor.ResizeHorizontal);

                    rightRender(rect2);

                    break;

                case EventType.MouseDown:

                    if (separatorRect.Contains(Event.current.mousePosition))
                        state.IsResizing = true;

                    break;

                case EventType.MouseUp:

                    state.IsResizing = false;

                    break;

                case EventType.MouseDrag:

                    if (state.IsResizing)
                    {
                        state.SplitLocation = Mathf.Clamp(Event.current.mousePosition.x, rect.xMin, rect.xMax - 30f);
                        GUI.changed = true;
                    }

                    break;

                default:
                    break;
            }

            if (state.IsResizing && Event.current.isMouse)
                Event.current.Use();
        }

        private static Rect GetSplitRect1(Rect baseRect, bool isVerticalSplit, float splitLocation)
        {
            Rect rect1 = new Rect(baseRect);

            if (isVerticalSplit)
                rect1.width = splitLocation;
            else
                rect1.height = splitLocation;

            return rect1;
        }

        private static Rect GetSplitRect2(Rect baseRect, bool isVerticalSplit, float splitLocation)
        {
            Rect rect2 = new Rect(baseRect);

            if(isVerticalSplit)
            {
                rect2.x += splitLocation;
                rect2.width -= splitLocation;
            }
            else
            {
                rect2.y += splitLocation;
                rect2.height -= splitLocation;
            }

            return rect2;
        }

        #endregion

        /// <summary>
        /// Draws a label for the provided string-valued Serialized Property. If the
        /// label is clicked, becomes a textboox
        /// </summary>
        /// <param name="rect">Rectangle to render in</param>
        /// <param name="property">The string property to edit</param>
        /// <param name="editMode">True if editing, false if not</param>
        /// <returns>Current edit mode value</returns>
        public static bool EditableLabel(Rect rect, SerializedProperty property,
            bool editMode)
        {
            if (editMode)
            {
                EditorGUI.DelayedTextField(rect, property, GUIContent.none);

                if (Event.current.rawType == EventType.MouseDown &&
                    !rect.Contains(Event.current.mousePosition))
                    return false;
            }
            else
            {
                EditorGUI.LabelField(rect, property.stringValue);

                if (Event.current.type == EventType.MouseDown &&
                    rect.Contains(Event.current.mousePosition))
                    return true;
            }

            return editMode;
        }
    }
}