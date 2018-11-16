using UnityEditor;
using UnityEngine;

namespace ZeroProgress.Common.Editors
{
    /// <summary>
    /// A split view utilizing EditorGUILayout functionality
    /// 
    /// Attempts to mimic the Animator window split view
    /// </summary>
    public class SplitViewLayout {

        /// <summary>
        /// How to split the view
        /// </summary>
        public enum SplitDirection
        {
            Horizontal,
            Vertical
        }

        /// <summary>
        /// Indicates that the sizing should have no constraint (can resize to any valid value)
        /// </summary>
        public const float NO_CONSTRAINT = -1f;

        /// <summary>
        /// The minimum allowable size for the first window
        /// </summary>
        public float MinSize = NO_CONSTRAINT;

        /// <summary>
        /// The maximum allowable size for the first window
        /// </summary>
        public float MaxSize = NO_CONSTRAINT;

        /// <summary>
        /// Margin to be applied to ensure the handle is grabbable
        /// </summary>
        private float normalizedMinMargin = 0.05f;

        /// <summary>
        /// Margin to be applied to ensure the handle is grabbable
        /// </summary>
        private float normalizedMaxMargin = 0.95f;

        /// <summary>
        /// Direction to split the view
        /// </summary>
        private SplitDirection splitDirection = SplitDirection.Horizontal;

        /// <summary>
        /// The split direction normalized
        /// </summary>
        private float normalizedSplitPosition = 0.3f;

        /// <summary>
        /// Current location of the scroll bar if used
        /// </summary>
        private Vector2 scrollPosition;

        /// <summary>
        /// Rectangle identifying the area to render in for
        /// the first window
        /// </summary>
        private Rect availableRect;
        
        /// <summary>
        /// Indicates if the window is currently being resized
        /// </summary>
        private bool isResizing = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="direction">Direction of the split</param>
        /// <param name="initialNormalizedSplitPosition">Where to start the split</param>
        /// <param name="minSizeConstraint">Any positive value to constrain the smallest
        /// allowable size for the first window</param>
        /// /// <param name="maxSizeConstraint">Any positive value to constrain the largest
        /// allowable size for the first window</param>
        public SplitViewLayout(SplitDirection direction, 
            float initialNormalizedSplitPosition = 0.5f, float minSizeConstraint = NO_CONSTRAINT,
            float maxSizeConstraint = NO_CONSTRAINT)
        {
            splitDirection = direction;
            normalizedSplitPosition = initialNormalizedSplitPosition;

            MinSize = minSizeConstraint;
            MaxSize = maxSizeConstraint;
        }

        /// <summary>
        /// Called in a similar fashion to BeginHorizontal
        /// </summary>
        public void BeginSplitView()
        {
            Rect tempRect;

            if (splitDirection == SplitDirection.Horizontal)
                tempRect = EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            else
                tempRect = EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
            
            if(Event.current.type == EventType.Repaint)
                availableRect = tempRect;

            float clampedValue = GetConstrainedValue();
            
            GUILayoutOption constraint = splitDirection == SplitDirection.Horizontal ?
                GUILayout.Width(clampedValue) :
                GUILayout.Height(clampedValue);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, constraint);
        }

        /// <summary>
        /// Identifies where the split occurs
        /// </summary>
        public void Split()
        {
            EditorGUILayout.EndScrollView();

            if (splitDirection == SplitDirection.Horizontal)
                ResizeHorizontalSplit();
            else
                ResizeVerticalSplit();
        }
        
        /// <summary>
        /// Identifies that the split view is completed, similar to EndHorizontal
        /// </summary>
        /// <returns>True if currently resizing, false if not. Use this value
        /// to determine if a repaint is necessary</returns>
        public bool EndSplitView()
        {
            if (splitDirection == SplitDirection.Horizontal)
                EditorGUILayout.EndHorizontal();
            else
                EditorGUILayout.EndVertical();

            return isResizing;
        }

        /// <summary>
        /// Helper to get the value of the window dimension properly
        /// constrained between min and max size
        /// </summary>
        /// <returns>The constrained dimensional value</returns>
        public float GetConstrainedValue()
        {
            float min = MinSize < 0f ? 0f : MinSize;

            if (splitDirection == SplitDirection.Horizontal)
            {
                float maxWidth = MaxSize < 0f ? availableRect.width : MaxSize;

                return Mathf.Clamp(availableRect.width * normalizedSplitPosition,
                    min, maxWidth);
            }
            else
            {
                float maxHeight = MaxSize < 0f ? availableRect.height : MaxSize;

                return Mathf.Clamp(availableRect.height * normalizedSplitPosition,
                    min, maxHeight);
            }
        }

        /// <summary>
        /// Handles the resize functionality of the split for a horizontal window
        /// </summary>
        private void ResizeHorizontalSplit()
        {
            float width = GetConstrainedValue();

            Rect resizeHandleRect = new Rect(width,
                    availableRect.y, 2f, availableRect.height);
            
            GUIExtensions.DrawVerticalLine(resizeHandleRect, Color.gray);
            
            EditorGUIUtility.AddCursorRect(resizeHandleRect, MouseCursor.ResizeHorizontal);

            CheckIsResizing(resizeHandleRect, SplitDirection.Horizontal);
        }

        /// <summary>
        /// Handles the resize functionality of the split for a horizontal window
        /// </summary>
        private void ResizeVerticalSplit()
        {
            float height = GetConstrainedValue();

            Rect resizeHandleRect = new Rect(availableRect.x,
                height, availableRect.width, 2f);

            GUIExtensions.DrawHorizontalLine(resizeHandleRect, Color.gray);

            EditorGUIUtility.AddCursorRect(resizeHandleRect, MouseCursor.ResizeVertical);

            CheckIsResizing(resizeHandleRect, SplitDirection.Vertical);
        }

        /// <summary>
        /// Checks the current gui event to determine if the user is currently
        /// resizing the view
        /// </summary>
        /// <param name="handleRect">Where the grab area is located</param>
        /// <param name="splitDirection">The direction of the split</param>
        private void CheckIsResizing(Rect handleRect, SplitDirection splitDirection)
        {
            if (Event.current.type == EventType.MouseDown &&
                handleRect.Contains(Event.current.mousePosition))
            {
                isResizing = true;
            }

            if (isResizing && availableRect.width > 0f)
            {
                if(splitDirection == SplitDirection.Horizontal)
                    normalizedSplitPosition = Event.current.mousePosition.x / availableRect.width;
                else
                    normalizedSplitPosition = Event.current.mousePosition.y / availableRect.height;
                
                normalizedSplitPosition = Mathf.Clamp(normalizedSplitPosition, 
                    normalizedMinMargin, normalizedMaxMargin);
            }

            if (Event.current.rawType == EventType.MouseUp)
                isResizing = false;
        }
    }
}
