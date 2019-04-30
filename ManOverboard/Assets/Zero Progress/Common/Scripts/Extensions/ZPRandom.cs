using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Additional random-generating functions
    /// </summary>
    public static class ZPRandom
    {
        /// <summary>
        /// Gets a random position within the provided bounds
        /// </summary>
        /// <param name="bounds">The bounds to generate a random position inside</param>
        /// <returns>A random position inside the bounds</returns>
        public static Vector3 RandomPosition(Bounds bounds)
        {
            return new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z));
        }

        /// <summary>
        /// Gets a random position within the provided bounding sphere
        /// </summary>
        /// <param name="bounds">The bounds of the sphere</param>
        /// <returns>A random position within the sphere</returns>
        public static Vector3 RandomPosition(BoundingSphere bounds)
        {
            return bounds.position + (Random.insideUnitSphere * bounds.radius);
        }
    }
}