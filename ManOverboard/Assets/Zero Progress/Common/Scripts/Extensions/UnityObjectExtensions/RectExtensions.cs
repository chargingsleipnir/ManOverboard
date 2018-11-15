using System;
using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Extension methods for the Unity Rect class
    /// </summary>
    public static class RectExtensions
    {
        /// <summary>
        /// Adds the provided position to this rectangle
        /// </summary>
        /// <param name="thisRect">This rectangle to add the position to</param>
        /// <param name="addPosition">The position to be added</param>
        /// <returns>A rect located at the sum of the two positions</returns>
        public static Rect AddPosition(this Rect thisRect, Vector2 addPosition)
        {
            return new Rect(thisRect.position + addPosition, thisRect.size);
        }

        /// <summary>
        /// Gets a new rect with this rects size at the specified position
        /// </summary>
        /// <param name="thisRect">The rect to replace the position for</param>
        /// <param name="newPosition">The new position for the new rectangle</param>
        /// <returns>The new rectangle with the position set as new position</returns>
        public static Rect WithPosition(this Rect thisRect, Vector2 newPosition)
        {
            return new Rect(newPosition.x, newPosition.y, thisRect.width, thisRect.height);
        }

        /// <summary>
        /// Creates a rect from the two provided points
        /// </summary>
        /// <param name="a">First point</param>
        /// <param name="b">Second point</param>
        /// <returns>Rect that spans between the two points</returns>
        public static Rect FromPoints(Vector2 a, Vector2 b)
        {
            float minX = Mathf.Min(a.x, b.x);
            float maxX = Mathf.Max(a.x, b.x);
            float minY = Mathf.Min(a.y, b.y);
            float maxY = Mathf.Max(a.y, b.y);

            return Rect.MinMaxRect(minX, minY, maxX, maxY);
        }

        /// <summary>
        /// Determines if this rect fully encapsulates the other rect
        /// </summary>
        /// <param name="thisRect">The rect to check is a container</param>
        /// <param name="otherRect">The rect to check is contained</param>
        /// <returns>True if contained, false if not</returns>
        public static bool Contains(this Rect thisRect, Rect otherRect)
        {
            return thisRect.Contains(otherRect.TopLeftGUI()) &&
                thisRect.Contains(otherRect.TopRightGUI()) &&
                thisRect.Contains(otherRect.BottomLeftGUI()) &&
                thisRect.Contains(otherRect.BottomRightGUI());
        }

        /// <summary>
        /// Gets the top left of the rectangle based on GUI coordinates
        /// </summary>
        /// <param name="rect">The rect to get the top left coordinate of</param>
        /// <returns>Vector2 representing the top left of the rectangle</returns>
        public static Vector2 TopLeftGUI(this Rect rect)
        {
            return new Vector2(rect.xMin, rect.yMin);
        }

        /// <summary>
        /// Gets the top right vector of the rectangle based on GUI coordinates
        /// </summary>
        /// <param name="rect">The rect to get the top right coordinate of</param>
        /// <returns>Vector2 representing the top right of the rectangle</returns>
        public static Vector2 TopRightGUI(this Rect rect)
        {
            return new Vector2(rect.xMax, rect.yMin);
        }

        /// <summary>
        /// Gets the bottom left of the rectangle based on GUI coordinates
        /// </summary>
        /// <param name="rect">The rect to get the bottom left coordinate of</param>
        /// <returns>Vector2 representing the bottom left of the rectangle</returns>
        public static Vector2 BottomLeftGUI(this Rect rect)
        {
            return new Vector2(rect.xMin, rect.yMax);
        }

        /// <summary>
        /// Gets the bottom right vector of the rectangle based on GUI coordinates
        /// </summary>
        /// <param name="rect">The rect to get the bottom right coordinate of</param>
        /// <returns>Vector2 representing the bottom right of the rectangle</returns>
        public static Vector2 BottomRightGUI(this Rect rect)
        {
            return new Vector2(rect.xMax, rect.yMax);
        }


        /// <summary>
        /// Scales the rectangle from the center by the provided value
        /// </summary>
        /// <param name="rect">The rect to scale</param>
        /// <param name="scale">The amount to scale by</param>
        /// <returns>The scaled rectangle</returns>
        public static Rect ScaleSizeBy(this Rect rect, float scale)
        {
            return rect.ScaleSizeBy(scale, rect.center);
        }

        /// <summary>
        /// Scales the rectangle from the provided pivot point
        /// </summary>
        /// <param name="rect">The rect to scale</param>
        /// <param name="scale">The amount to scale by</param>
        /// <param name="pivotPoint">Where to scale from</param>
        /// <returns>The scaled rectangle</returns>
        public static Rect ScaleSizeBy(this Rect rect, float scale, Vector2 pivotPoint)
        {
            Rect result = rect;
            result.x -= pivotPoint.x;
            result.y -= pivotPoint.y;
            result.xMin *= scale;
            result.xMax *= scale;
            result.yMin *= scale;
            result.yMax *= scale;
            result.x += pivotPoint.x;
            result.y += pivotPoint.y;
            return result;
        }

        /// <summary>
        /// Apply a non-uniform scale to the rectangle
        /// </summary>
        /// <param name="rect">The rect to scale</param>
        /// <param name="scale">The amount to scale by</param>
        /// <returns>The scaled rectangle</returns>
        public static Rect ScaleSizeBy(this Rect rect, Vector2 scale)
        {
            return rect.ScaleSizeBy(scale, rect.center);
        }

        /// <summary>
        /// Apply a non-uniform scale at the provided pivot location
        /// </summary>
        /// <param name="rect">The rect to scale</param>
        /// <param name="scale">The amount to scale by</param>
        /// <param name="pivotPoint">Where to scale from</param>
        /// <returns>The scaled rectangle</returns>
        public static Rect ScaleSizeBy(this Rect rect, Vector2 scale, Vector2 pivotPoint)
        {
            Rect result = rect;
            result.x -= pivotPoint.x;
            result.y -= pivotPoint.y;
            result.xMin *= scale.x;
            result.xMax *= scale.x;
            result.yMin *= scale.y;
            result.yMax *= scale.y;
            result.x += pivotPoint.x;
            result.y += pivotPoint.y;
            return result;
        }

        /// <summary>
        /// Splits the rect into equal sizes
        /// </summary>
        /// <param name="rect">The rect to split</param>
        /// <param name="splitCount">The number of splits to make</param>
        /// <returns>An array of rect representing the splits</returns>
        public static Rect[] SplitRectHorizontally(this Rect rect, int splitCount, int spacing = 0)
        {
            if (splitCount <= 0)
                throw new ArgumentOutOfRangeException("SplitCount must be greater than 0");

            Rect[] rects = new Rect[splitCount];

            float splitWidth = (rect.width - (spacing * (splitCount - 1))) / splitCount;

            for (int i = 0; i < splitCount; i++)
            {
                Rect currentRect = new Rect(rect);
                currentRect.x = rect.x + (splitWidth + spacing) * i;
                currentRect.width = splitWidth;
                rects[i] = currentRect;
            }

            return rects;
        }

        public static Vector2 GetIntersectionPoint(this Rect rect, Vector2 direction)
        {
            Vector2 result = Vector2.zero;

            float largestRectDimension = Mathf.Sqrt(rect.width * rect.width + rect.height * rect.height);

            LineRectIntersection(rect, rect.center, rect.center + direction.normalized * largestRectDimension, out result);

            return result;
        }

        public static Vector2 GetIntersectionPointAABB(this Rect rect, Vector2 direction)
        {
            Vector2 result = Vector2.zero;

            Vector2 aabbDirection = direction.RoundToNearest90();

            float largestRectDimension = Mathf.Sqrt(rect.width * rect.width + rect.height * rect.height);

            LineRectIntersection(rect, rect.center, rect.center + aabbDirection * largestRectDimension, out result);

            return result;
        }

        public static bool LineRectIntersection(this Rect rect, Vector2 lineStart,
            Vector2 lineEnd)
        {
            Vector2 intersection;

            return LineRectIntersection(rect, lineStart, lineEnd, out intersection);
        }

        public static bool LineRectIntersection(this Rect rect, Vector2 lineStart, 
            Vector2 lineEnd, out Vector2 intersectionPoint)
        {
            Vector2 topLeft = rect.TopLeftGUI();
            Vector2 topRight = rect.TopRightGUI();
            Vector2 bottomLeft = rect.BottomLeftGUI();
            Vector2 bottomRight = rect.BottomRightGUI();

            intersectionPoint = Vector2.zero;

            if (ZPMath.LineLineCollision(topLeft, topRight, lineStart, lineEnd, out intersectionPoint))
                return true;

            if (ZPMath.LineLineCollision(topRight, bottomRight, lineStart, lineEnd, out intersectionPoint))
                return true;

            if (ZPMath.LineLineCollision(bottomRight, bottomLeft, lineStart, lineEnd, out intersectionPoint))
                return true;

            if (ZPMath.LineLineCollision(topLeft, bottomLeft, lineStart, lineEnd, out intersectionPoint))
                return true;

            return false;
        }
    }
}