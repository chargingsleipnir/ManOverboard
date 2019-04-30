using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Additional math logic not provided by Unity
    /// </summary>
    public static class ZPMath
    {
        /// <summary>
        /// An approximation function that allows the specifying of a custom Epsilon value
        /// </summary>
        /// <param name="Value">The value to check</param>
        /// <param name="CompareValue">The value to check against</param>
        /// <param name="Epsilon">The epsilon for comparison</param>
        /// <returns>True if value is within epsilon to the compare value, false if not</returns>
        public static bool Approximately(float Value, float CompareValue, float Epsilon = 0.001f)
        {
            return Mathf.Abs(Value - CompareValue) <= Epsilon;
        }

        /// <summary>
        /// Helper to check if a value is close to zero
        /// </summary>
        /// <param name="Value">The value to check</param>
        /// <param name="Epsilon">The epsilon for comparison</param>
        /// <returns>True if the value is within epsilon of zero, false if not</returns>
        public static bool NearZero(float Value, float Epsilon = 0.001f)
        {
            return Approximately(Value, 0f, Epsilon);
        }

        /// <summary>
        /// Rounds a value to the specified value if it's within epsilon
        /// </summary>
        /// <param name="Value">The value to check</param>
        /// <param name="DesiredValue">The value to round towards</param>
        /// <param name="Epsilon">The epsilon for comparison</param>
        /// <returns>The DesiredValue if Value is within epsilon to it, otherwise the Value unchanged</returns>
        public static float RoundTo(float Value, float DesiredValue, float Epsilon = 0.001f)
        {
            if (Approximately(Value, DesiredValue, Epsilon))
                return DesiredValue;
            else
                return Value;
        }

        /// <summary>
        /// Rounds the values of the vector to the desired value if it's within epsilon
        /// </summary>
        /// <param name="Values">The vector values to round</param>
        /// <param name="DesiredValue">The value to round towards</param>
        /// <param name="Epsilon">The epsilon for comparison</param>
        /// <returns>The vector with each component having RoundTo applied</returns>
        public static Vector3 RoundValuesTo(Vector3 Values, float DesiredValue, float Epsilon = 0.001f)
        {
            for (int i = 0; i < 3; i++)
            {
                Values[i] = RoundTo(Values[i], DesiredValue, Epsilon);
            }

            return Values;
        }

        /// <summary>
        /// Determines if a point is enclosed within a polygon
        /// </summary>
        /// <param name="point">The point to check containment of</param>
        /// <param name="polygonPoints">The collection of polygon points</param>
        /// <returns>True if in polygon, false if not</returns>
        public static bool IsPointInPolygon(Vector2 point, Vector2[] polygonPoints)
        {
            int prevI = polygonPoints.Length - 1;
            for (int i = 0; i < polygonPoints.Length; prevI = i, i++)
            {
                Vector2 v0 = polygonPoints[prevI];
                Vector2 v1 = polygonPoints[i];
                Vector2 edgeDir = v1 - v0;
                Vector2 edgeNormal = new Vector3(-edgeDir.y, edgeDir.x);
                float sign = Vector2.Dot(v0 - point, edgeNormal);

                if (sign > 0.0f) return false; // point not in polygon

            }
            return true; // point in polygon
        }

        /// <summary>
        /// Determines if a point is enclosed within a polygon (2D Shape)
        /// </summary>
        /// <param name="point">The point to check containment of</param>
        /// <param name="polygonPoints">The collection of polygon points</param>
        /// <returns>True if in polygon, false if not</returns>
        public static bool IsPointInPolygon(Vector3 point, Vector3[] polygonPoints)
        {
            int prevI = polygonPoints.Length - 1;
            for (int i = 0; i < polygonPoints.Length; prevI = i, i++)
            {
                Vector3 v0 = polygonPoints[prevI];
                Vector3 v1 = polygonPoints[i];
                Vector3 edgeDir = v1 - v0;
                Vector3 edgeNormal = new Vector3(-edgeDir.y, edgeDir.x);
                float sign = Vector3.Dot(v0 - point, edgeNormal);

                if (sign > 0.0f) return false; // point not in polygon

            }
            return true; // point in polygon
        }

        /// <summary>
        /// Checks for a collision point between two lines
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="intersectionPoint">The point of intersection if returns true</param>
        /// <returns>True if collide, false if not</returns>
        public static bool LineLineCollision(Vector2 a, Vector2 b, Vector2 c, Vector2 d, out Vector2 intersectionPoint)
        {
            float interX, interY;

            // calculate distance to intersection point
            bool result = LineLineCollision(a.x, a.y, b.x, b.y, 
                c.x, c.y, d.x, d.y, out interX, out interY);

            intersectionPoint = new Vector2(interX, interY);

            return result;
        }

        /// <summary>
        /// Determines if two Vector2 lines are intersecting
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="x3"></param>
        /// <param name="y3"></param>
        /// <param name="x4"></param>
        /// <param name="y4"></param>
        /// <returns></returns>
        public static bool LineLineCollision(float x1, float y1,
            float x2, float y2, float x3, float y3, float x4, float y4)
        {
            float interX, interY;

            return LineLineCollision(x1, x2, y1, y2, 
                x3, y3, x4, y4, out interX, out interY);
        }

        /// <summary>
        /// Determines if two Vector2 lines are intersecting
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="x3"></param>
        /// <param name="y3"></param>
        /// <param name="x4"></param>
        /// <param name="y4"></param>
        /// <param name="intersectionX">The intersection point X</param>
        /// <param name="intersectionY">The intersection point Y</param>
        /// <returns>True if intersecting, false if not. The two out parameters
        /// are only valid if this returns true</returns>
        public static bool LineLineCollision(float x1, float y1, 
            float x2, float y2, float x3, float y3, float x4, float y4, 
            out float intersectionX, out float intersectionY)
        {
            // calculate the distance to intersection point
            float uA = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / 
                ((y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1));

            float uB = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)) / 
                ((y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1));

            // set the intersection point
            intersectionX = x1 + (uA * (x2 - x1));
            intersectionY = y1 + (uA * (y2 - y1));

            // if uA and uB are between 0-1, lines are colliding
            return (uA >= 0 && uA <= 1 && uB >= 0 && uB <= 1);
        }
    }
}
