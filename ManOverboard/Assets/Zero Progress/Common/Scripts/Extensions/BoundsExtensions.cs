using UnityEngine;

namespace ZeroProgress.Common
{
    public static class BoundsExtensions
    {
        /// <summary>
        /// Determines if the specified bounds is fully encapsulated by this bounds
        /// </summary>
        /// <param name="ThisBounds">The bounds to compare against</param>
        /// <param name="TargetBounds">The bounds to determine containment on</param>
        /// <returns>True if TargetBounds is fully contained, false if not</returns>
        public static bool Contains(this Bounds ThisBounds, Bounds TargetBounds)
        {
            return ThisBounds.Contains(TargetBounds.min) && ThisBounds.Contains(TargetBounds.max);
        }
    }
}