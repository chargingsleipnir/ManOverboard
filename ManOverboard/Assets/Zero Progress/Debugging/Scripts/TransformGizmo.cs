using System;
using UnityEngine;

namespace ZeroProgress.Debugging
{
    /// <summary>
    /// Descriptor of a Line
    /// </summary>
    [Serializable]
    public class GizmoLineInfo
    {
        /// <summary>
        /// The color to be used for the line
        /// </summary>
        public Color LineColor;

        /// <summary>
        /// The Length of the line
        /// </summary>
        public float Length = 0.25f;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public GizmoLineInfo()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="LineColor">The color to be used for the line</param>
        /// <param name="LineLength">The Length of the line</param>
        public GizmoLineInfo(Color LineColor, float LineLength)
        {
            this.LineColor = LineColor;
            this.Length = LineLength;
        }
    }

    /// <summary>
    /// Gizmo for illustrating transform local directions even when the object is not selected
    /// </summary>
    public class TransformGizmos : MonoBehaviour
    {
        [Tooltip("Descriptor for the line representing the objects Forward direction")]
        public GizmoLineInfo ForwardLine = new GizmoLineInfo(Color.blue, 0.25f);

        [Tooltip("Descriptor for the line representing the objects Up direction")]
        public GizmoLineInfo UpLine = new GizmoLineInfo(Color.green, 0.25f);

        [Tooltip("Descriptor for the line representing the objects Right direction")]
        public GizmoLineInfo RightLine = new GizmoLineInfo(Color.red, 0.25f);

        [Tooltip("Used to set all of the line lengths")]
        public float LineLengthOverride = 1f;

        [Tooltip("Indicates whether or not to utilize the Line Length Override option")]
        public bool UseLineLengthOverride = false;

        /// <summary>
        /// Enumeration to help abstract the selection of line attributes
        /// </summary>
        private enum LineDirection { Forward, Up, Right };

        private void OnDrawGizmos()
        {
            Vector3 position = transform.position;

            Color initialColor = Gizmos.color;

            Gizmos.color = ForwardLine.LineColor;
            Gizmos.DrawLine(position, position + transform.forward * GetLineLength(LineDirection.Forward));

            Gizmos.color = UpLine.LineColor;
            Gizmos.DrawLine(position, position + transform.up * GetLineLength(LineDirection.Up));

            Gizmos.color = RightLine.LineColor;
            Gizmos.DrawLine(position, position + transform.right * GetLineLength(LineDirection.Right));

            Gizmos.color = initialColor;
        }

        /// <summary>
        /// Retrieves the length of the line to be used
        /// </summary>
        /// <param name="Direction">The direction of the line</param>
        /// <returns>The length to be used</returns>
        private float GetLineLength(LineDirection Direction)
        {
            float lineLength = LineLengthOverride;

            if (!UseLineLengthOverride)
            {
                switch (Direction)
                {
                    case LineDirection.Forward:
                        lineLength = ForwardLine.Length;
                        break;
                    case LineDirection.Up:
                        lineLength = UpLine.Length;
                        break;
                    case LineDirection.Right:
                        lineLength = RightLine.Length;
                        break;
                }
            }

            return Mathf.Abs(lineLength);
        }
    }
}