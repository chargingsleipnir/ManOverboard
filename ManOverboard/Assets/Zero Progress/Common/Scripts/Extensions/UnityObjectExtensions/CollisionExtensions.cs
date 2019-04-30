using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Extension methods for the collision information
    /// </summary>
    public static class CollisionExtensions
    {

        /// <summary>
        /// Retrieves a component from the game object found in the collision
        /// data structure
        /// </summary>
        /// <typeparam name="T">The type of component to retrieve</typeparam>
        /// <param name="thisCollision">The collision to search through</param>
        /// <returns>The found component or null</returns>
        public static T GetComponent<T>(this Collision thisCollision)
        {
            return thisCollision.gameObject.GetComponent<T>();
        }

        /// <summary>
        /// Retrieves all components of the matching type from the game object 
        /// found in the collision data structure
        /// </summary>
        /// <typeparam name="T">The type of component to retrieve</typeparam>
        /// <param name="thisCollision">The collision to search through</param>
        /// <returns>The found components or null</returns>
        public static T[] GetComponents<T>(this Collision thisCollision)
        {
            return thisCollision.gameObject.GetComponents<T>();
        }

        /// <summary>
        /// Retrieves a component from the game object found in the collision
        /// data structure
        /// </summary>
        /// <param name="thisCollision">The collision to search through</param>
        /// <param name="componentType">The type of component to retrieve</param>
        /// <returns>The found component or null</returns>
        public static Component GetComponent(this Collision thisCollision, System.Type componentType)
        {
            return thisCollision.gameObject.GetComponent(componentType);
        }
    }
}