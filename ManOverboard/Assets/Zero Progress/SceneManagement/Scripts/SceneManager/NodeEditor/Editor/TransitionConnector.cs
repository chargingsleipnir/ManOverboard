using UnityEngine;
using ZeroProgress.Common;
using ZeroProgress.NodeEditor;
using UnityEditor;
using ZeroProgress.Common.Editors;

namespace ZeroProgress.SceneManagementUtility.Editors
{
    /// <summary>
    /// The connector used by the Node Editor to provide a GUI
    /// of all Scene Transitions
    /// </summary>
    public class TransitionConnector : Connector
    {
        /// <summary>
        /// The size to make the triangle that is found at the midpoint
        /// </summary>
        public Vector3 TriangleSize { get; set; }
        
        /// <summary>
        /// The number of duplicate transitions.
        /// Used to determine if a single triangle or multiple triangles
        /// should be rendered
        /// </summary>
        public int ConnectionCount { get; set; }
        
        /// <summary>
        /// True to apply an offset to keep inputs and outputs separated
        /// </summary>
        public bool ShiftOver = false;

        /// <summary>
        /// How much space to apply between input and output lines
        /// (only when both are existing)
        /// </summary>
        public float OffsetLength = 8f;

        private bool isCurrentlyTransitioning;

        /// <summary>
        /// True if this transition is currently being processed, false if not
        /// </summary>
        public bool IsCurrentlyTransitioning
        {
            get { return isCurrentlyTransitioning; }
            set
            {
                isCurrentlyTransitioning = value;
                animLerpTime = 0f;
            }
        }
        
        private float animLerpTime = 0f;

        /// <summary>
        /// Factory method to be passed to the Context Menu for creating
        /// an instance of this connector class
        /// </summary>
        /// <param name="editor">The node editor making the connection</param>
        /// <param name="nodes">Any nodes to start the connector with</param>
        /// <returns></returns>
        public static TransitionConnector TransitionConnectorFactory(
            ZeroProgress.NodeEditor.NodeEditor editor, params Node[] nodes)
        {
            TransitionConnector newConnector = new TransitionConnector();

            if (nodes == null)
                return newConnector;

            foreach (Node node in nodes)
            {
                newConnector.AddNode(node);
            }

            return newConnector;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="triangleSize">The size to make the triangle found at the
        /// line midpoint</param>
        public TransitionConnector(float triangleSize = 15f)
            : this(new Vector3(triangleSize, triangleSize, triangleSize))
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="triangleSize">The size to make the triangle found at the
        /// line midpoint</param>
        public TransitionConnector(Vector3 triangleSize)
        {
            TriangleSize = triangleSize;
            ConnectionCount = 1;
        }

        /// <summary>
        /// Determines if the provided point is within the connector
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <param name="margin">The margin to apply around the point</param>
        /// <param name="offset">The node editors offset</param>
        /// <returns>True if the point is found within the connector, false if not</returns>
        public override bool ContainsPoint(Vector2 point, Vector2 margin, Vector2 offset)
        {
            bool contains = false;

            Rect mouseRect = new Rect(point - (margin * 0.5f), margin);

            ForEachConnection(offset, (node1, node1Rect, node2, node2Rect) =>
            {
                Vector3 start, end, mid, diff;
                Quaternion rotation;

                GetPoints(node1Rect, node2Rect, out start, out end, 
                    out mid, out diff, out rotation);

                if (mouseRect.LineRectIntersection(start, end))
                {
                    contains = true;
                    // return false to break out of the loop
                    return false;
                }

                return true;
            });

            return contains;
        }

        /// <summary>
        /// Determines if the connector is contained within the provided rect
        /// </summary>
        /// <param name="queryRect">The rectangle used to check connector containment</param>
        /// <param name="offset">Node editors' pan offset</param>
        /// <param name="allowOverlap">True to pass if only overlapping, false to require
        /// full containment</param>
        /// <returns>True if contained, false if not</returns>
        public override bool ContainedByRect(Rect queryRect, Vector2 offset, bool allowOverlap = false)
        {
            bool contains = true;

            ForEachConnection(offset, (node1, node1Rect, node2, node2Rect) =>
            {
                Vector3 start, end, mid, diff;
                Quaternion rotation;

                GetPoints(node1Rect, node2Rect, out start, out end,
                    out mid, out diff, out rotation);

                if (allowOverlap)
                {
                    // if overlap is allowed and currently overlapping, skip the contains
                    // check, because we've passed
                    if (queryRect.LineRectIntersection(start, end))
                        return true;
                }

                // If the start or end point are not within the query rectangle
                // then containment check is failed
                if (!queryRect.Contains(start) || !queryRect.Contains(end))
                {
                    contains = false;
                    return false;
                }

                return true;
            });

            return contains;
        }

        /// <summary>
        /// Renders the connector
        /// </summary>
        /// <param name="offset">Node editor pan offset</param>
        public override void Draw(Vector2 offset)
        {
            Color color = Color.white;

            if (IsSelected())
                color = NodeEditor.NodeEditor.DefaultSelectionColor;
            
            Color handleColor = Handles.color;
            Handles.color = color;

            ForEachConnection(offset, (node1, node1Rect, node2, node2Rect) =>
            {
                Vector3 start, end, mid, diff;
                Quaternion rotation;

                GetPoints(node1Rect, node2Rect, out start, out end, 
                    out mid, out diff, out rotation);

#if UNITY_EDITOR

                Handles.DrawLine(start, end);

                if (node1 is SceneNode && node2 is SceneNode)
                {
                    DrawTriangles(mid, diff,
                        rotation, ConnectionCount, color);
                }

#endif

                if (IsCurrentlyTransitioning)
                {
                    ActiveTransitionAnimation(node1Rect, node2Rect, rotation, animLerpTime);
                    animLerpTime += (EditorTiming.GetDeltaTime() / 2.0f);

                    if (animLerpTime >= 1f)
                        animLerpTime = 0f;
                }

                return true;
            });

            SceneNode startNode = GetStartNode() as SceneNode;
            SceneNode endNode = GetEndNode() as SceneNode;

            if (startNode == null || endNode == null)
                return;

            if (startNode.SceneId == endNode.SceneId)
            {
                Rect nodeRect = startNode.GetNodeRect(offset);
                Vector2 triMid = new Vector2(nodeRect.x, nodeRect.center.y);
                triMid.x -= 7.5f;


                DrawTriangle(triMid, new Vector3(15f, 15f, 1f), 
                    Quaternion.Euler(0f, 0f, 0f));
            }

            Handles.color = handleColor;
        }
                
        /// <summary>
        /// Helper to do the math to get the points used to render the lines
        /// as well as the triangles
        /// </summary>
        /// <param name="node1Rect">The rectangle of the first node</param>
        /// <param name="node2Rect">The rectangle of the second node</param>
        /// <param name="startPoint">Out for where the connection starts</param>
        /// <param name="endPoint">Out for where the connection ends</param>
        /// <param name="midPoint">Out for the midpoint of the line</param>
        /// <param name="diff">Out for the difference between start and end</param>
        /// <param name="triRotation">Out for the rotation to be applied to
        /// the triangles</param>
        private void GetPoints(Rect node1Rect, Rect node2Rect, 
            out Vector3 startPoint, out Vector3 endPoint, 
            out Vector3 midPoint, out Vector3 diff, out Quaternion triRotation)
        {
            startPoint = new Vector3(node1Rect.center.x, node1Rect.center.y);

            endPoint = new Vector3(node2Rect.center.x, node2Rect.center.y);

            diff = endPoint - startPoint;
            Vector3 normDiff = diff.normalized;

            midPoint = startPoint + (diff * 0.5f);

            float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

            triRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            Vector3 perpendicular = new Vector3(-normDiff.y, normDiff.x, normDiff.z);

            Vector3 inOutOffset = Vector3.zero;

            if (ShiftOver)
                inOutOffset = perpendicular * OffsetLength;

            startPoint += inOutOffset;
            endPoint += inOutOffset;
            midPoint += inOutOffset;
        }

        /// <summary>
        /// Render the appropriate number of triangles
        /// </summary>
        /// <param name="midPoint">Midpoint of the line</param>
        /// <param name="diff">Difference between start and end</param>
        /// <param name="rotation">Rotation of the triangles</param>
        /// <param name="connectionCount">If greater than 1, 3 triangles are rendered</param>
        /// <param name="color">The color to draw the triangles with</param>
        private void DrawTriangles(Vector2 midPoint, Vector2 diff, 
            Quaternion rotation, int connectionCount, Color color)
        {
            DrawTriangle(midPoint, TriangleSize, rotation);

            if (connectionCount > 1)
            {
                diff.Normalize();

                Vector2 upperMidpoint = midPoint + (diff * TriangleSize);
                Vector2 lowerMidpoint = midPoint - (diff * TriangleSize);

                DrawTriangle(upperMidpoint, TriangleSize, rotation);
                DrawTriangle(lowerMidpoint, TriangleSize, rotation);
            }
        }

        /// <summary>
        /// Draws a triangle
        /// </summary>
        /// <param name="midPoint">The center of the triangle</param>
        /// <param name="scale">The scale of the triangle</param>
        /// <param name="rotation">The rotation to be applied</param>
        /// <returns>The points of the triangle</returns>
        public static Vector3[] DrawTriangle(Vector3 midPoint, Vector3 scale, Quaternion rotation)
        {
            Vector3[] trianglePoints = new Vector3[3]
            {
                    midPoint + (rotation * Vector3.Scale(new Vector2(0.5f, 0.0f), scale)),
                    midPoint + (rotation * Vector3.Scale(new Vector2(0.0f, 0.5f), scale)),
                    midPoint + (rotation * Vector3.Scale(new Vector2(0.0f, -0.5f), scale))
            };
            
            Handles.DrawAAConvexPolygon(trianglePoints);

            return trianglePoints;
        }
        
        private void ActiveTransitionAnimation(Rect node1Rect, Rect node2Rect, Quaternion rotation, float time)
        {
            Vector2 startPoint = node1Rect.center;
            Vector2 endPoint = node2Rect.center;

            Vector2 position = Vector2.Lerp(startPoint, endPoint, time);

            GUIStyle iconStyle = SceneManagerResources.TransitionIconStyle;

            Vector2 rectSize = new Vector2(iconStyle.fixedWidth, iconStyle.fixedHeight);

            Vector3 axis;
            float angle;

            rotation.ToAngleAxis(out angle, out axis);

            Rect animRect = new Rect(position - (rectSize * 0.5f), rectSize);
            
            GUI.Box(animRect, GUIContent.none, iconStyle);
        }        
    }
}
