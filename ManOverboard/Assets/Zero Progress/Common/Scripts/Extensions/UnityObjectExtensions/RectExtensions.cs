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
            return new Rect(thisRect.x + addPosition.x,
                thisRect.y + addPosition.y,
                thisRect.width, thisRect.height);
        }

        /// <summary>
        /// Gets this rect with the specified position
        /// </summary>
        /// <param name="thisRect">The rect to replace the position for</param>
        /// <param name="newPosition">The new position for the new rectangle</param>
        /// <returns>The new rectangle with the position set as new position</returns>
        public static Rect WithPosition(this Rect thisRect, Vector2 newPosition)
        {
            return new Rect(newPosition.x, newPosition.y, thisRect.width, thisRect.height);
        }
    }
}