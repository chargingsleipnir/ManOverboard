using UnityEditor;
 using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Extensions for the UnityEditor.Handles static class
    /// </summary>
    public static class HandlesExtensions
    {
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
        
        /// <summary>
        /// Draws a line with a triangle in the midpoint, similar to the
        /// AnimatorWindow transition line
        /// </summary>
        /// <param name="startPoint">The start point of the line</param>
        /// <param name="endPoint">End point of the line</param>
        /// <param name="lineWidth">Width of the line</param>
        /// <param name="triangleScale">Scale to apply to the triangle (15f is a good value)</param>
        public static void DrawLineWithTriangle(Vector3 startPoint, Vector3 endPoint, float lineWidth, Vector3 triangleScale)
        {
            Vector3 difference = endPoint - startPoint;

            Vector3 midPoint = startPoint + (difference * 0.5f);
            
            float angle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            DrawTriangle(midPoint, triangleScale, rotation);

            Handles.DrawAAPolyLine(lineWidth, startPoint, endPoint);      
        }      
        
        /// <summary>
        /// Determines if a point is on a provided line
        /// </summary>
        /// <param name="a">The start point of the line</param>
        /// <param name="b">The end point of the line</param>
        /// <param name="c">The point to check if on the line or not</param>
        /// <param name="lineWidth">The width of the line</param>
        /// <returns>True if on line, false if not</returns>
        public static bool IsPointOnLine(Vector3 a, Vector3 b, Vector3 c, float lineWidth = 1f)
        {
            Vector3 ac = c - a;
            Vector3 ab = b - a;

            float abDisSquared = ab.sqrMagnitude;

            float ac_ab_dot = Vector3.Dot(ac, ab);

            if (ac_ab_dot <= 0f || ac_ab_dot >= abDisSquared)
                return false;

            float acDisSquared = ac.sqrMagnitude;

            float widthSquared = lineWidth * lineWidth;

            return abDisSquared * acDisSquared <= (widthSquared * abDisSquared + (ac_ab_dot * ac_ab_dot));
        }
    }
}