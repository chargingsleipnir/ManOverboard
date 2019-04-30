using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Extensions for the GameObject class
    /// </summary>
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Extension method for retrieving a component, and if it doesn't exist, adding it
        /// </summary>
        /// <typeparam name="T">The type of component to be retrieved/added</typeparam>
        /// <param name="ThisGameObject">The game object to perform this action with</param>
        /// <returns>The found or added component</returns>
        public static T GetOrAddComponent<T>(this GameObject ThisGameObject) where T : Component
        {
            T foundComponent = ThisGameObject.GetComponent<T>();

            if (foundComponent != null)
                return foundComponent;

            return ThisGameObject.gameObject.AddComponent<T>();
        }
    }
}