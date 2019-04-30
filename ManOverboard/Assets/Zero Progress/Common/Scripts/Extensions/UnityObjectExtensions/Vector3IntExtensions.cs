using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Extension methods for Vector3
    /// </summary>
    public static class Vector3IntExtensions
    {
        /// <summary>
        /// Helper to divide a Vector3Int by another while ignoring 0's in the other vector
        /// (if the other vector has a 0 in an axis, that axis is ignored for division and the value
        /// of this vector is used instead. i.e. if other.x == 0, then result.x == thisVector.x)
        /// (Generally for getting averages of each axis)
        /// </summary>
        /// <param name="thisVector">This vector</param>
        /// <param name="other">The vector that represents the values to divide by</param>
        /// <param name="epsilon">Used to determine proximity to 0 to prevent divide by 0 errors</param>
        /// <returns>The results of the division. If any axis is 0, thisVectors value for that axis will be substituted </returns>
        public static Vector3Int SafeDivideComponents(this Vector3Int thisVector, Vector3Int other, float epsilon = 0.0001f)
        {
            for (int i = 0; i < 3; i++)
            {
                if (ZPMath.NearZero(other[i], epsilon))
                    other[i] = 1;
            }

            return thisVector.DivideComponents(other);
        }

        /// <summary>
        /// Divides a vector by another vector (axis-wise division). Useful for
        /// averages. Divide by zero exceptions can result.
        /// </summary>
        /// <param name="thisVector">This vector</param>
        /// <param name="other">the vector to divide by</param>
        /// <returns>The results of the division of each axis</returns>
        public static Vector3Int DivideComponents(this Vector3Int thisVector, Vector3Int other)
        {
            return new Vector3Int(thisVector.x / other.x, thisVector.y / other.y, thisVector.z / other.z);
        }

        /// <summary>
        /// Divide a Vector3Ints components by a single integer
        /// </summary>
        /// <param name="thisVector">This vector</param>
        /// <param name="divisor">Value to divide by</param>
        /// <returns>The results of the division. If the divisor is 0, thisVector is returned instead</returns>
        public static Vector3Int SafeDivide(this Vector3Int thisVector, int divisor)
        {
            if (divisor == 0)
                return thisVector;

            return new Vector3Int(thisVector.x / divisor, thisVector.y / divisor, thisVector.z / divisor);
        }

        /// <summary>
        /// Divide a Vector3Ints components by a single integer
        /// </summary>
        /// <param name="thisVector">This vector</param>
        /// <param name="divisor">Value to divide by</param>
        /// <returns>The results of the division</returns>
        public static Vector3Int Divide(this Vector3Int thisVector, int divisor)
        {
            return new Vector3Int(thisVector.x / divisor, thisVector.y / divisor, thisVector.z / divisor);
        }
    }
}