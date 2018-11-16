using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Extension methods for the Vector2 class
    /// </summary>
    public static class Vector2Extensions
    {
        /// <summary>
        /// Gets the nearest cardinal direction
        /// </summary>
        /// <param name="thisVector">This vector</param>
        /// <returns>The rounded vector</returns>
        public static Vector2 RoundToNearest90(this Vector2 thisVector)
        {
            float angle = Vector2.SignedAngle(-Vector2.up, thisVector);
            
            if (angle > -45 && angle < 45)
                return Vector2.down;
            else if (angle >= 45 && angle < 135)
                return Vector2.right;
            else if ((angle >= 135 && angle <= 180) || (angle >= -180 && angle < -135))
                return Vector2.up;
            else
                return Vector2.left;
        }
    }
}