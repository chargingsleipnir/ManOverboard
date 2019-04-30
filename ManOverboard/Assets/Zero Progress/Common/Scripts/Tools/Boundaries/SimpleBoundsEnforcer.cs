using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Bounds Enforcer that uses the Bounds data structure to
    /// keep an item within the area
    /// </summary>
    public class SimpleBoundsEnforcer : MonoBehaviour, IBoundsEnforcer
    {
        [Tooltip("The boundaries to keep items within")]
        public Bounds DefinedBounds;

        /// <summary>
        /// Determines if the specified point is within the bounds
        /// </summary>
        /// <param name="Point">The point to check</param>
        /// <returns>True if the point is within bounds, false if not</returns>
        public bool IsInBounds(Vector3 Point)
        {
            return DefinedBounds.Contains(Point);
        }

        /// <summary>
        /// Calculates the closest valid point in-bounds for the provided point
        /// </summary>
        /// <param name="Point">The point to evaluate</param>
        /// <returns>Returns the Point unmodified if within bounds. If outside the bounds,
        /// returns the point closest to the provided that can be considered valid</returns>
        public Vector3 SetInBounds(Vector3 Point)
        {
            return DefinedBounds.ClosestPoint(Point);
        }
    }
}