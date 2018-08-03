using UnityEngine;

namespace ZeroProgress.Common
{
    public interface IBoundsEnforcer
    {
        /// <summary>
        /// Determines if the specified point is within the bounds
        /// </summary>
        /// <param name="Point">The point to check</param>
        /// <returns>True if the point is within bounds, false if not</returns>
        bool IsInBounds(Vector3 Point);

        /// <summary>
        /// Calculates the closest valid point in-bounds for the provided point
        /// </summary>
        /// <param name="Point">The point to evaluate</param>
        /// <returns>Returns the Point unmodified if within bounds. If outside the bounds,
        /// returns the point closest to the provided that can be considered valid</returns>
        Vector3 SetInBounds(Vector3 Point);
    }
}