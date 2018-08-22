using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Extensions for the component type
    /// </summary>
    public static class ComponentExtensions
    {
        /// <summary>
        /// Attempts to find the component of the specified type if the provided instance is null
        /// </summary>
        /// <typeparam name="T">The type of component to search for</typeparam>
        /// <param name="ComponentToSearch">The 'this' parameter for this extension method. This is the object that the search will begin on</param>
        /// <param name="Instance">The instance to evaluate. If this value is null, a search will take place to find a component of the appropriate type.
        /// If it is not null, nothing will happen</param>
        /// <param name="SearchChildren">True to search children if the instance is null and the component isn't found on this object, false to return null
        /// if the component wasn't found</param>
        /// <returns>The instance if it wasn't passed in as null. The found instance if an instance could be found on this object or it's children (if SearchChildren is true).
        /// Null if it could not be found based on the provided criterion</returns>
        public static T GetComponentIfNull<T>(this Component ComponentToSearch, T Instance, bool SearchChildren = false) where T : Component
        {
            if (Instance != null)
                return Instance;

            Instance = ComponentToSearch.GetComponent<T>();

            if (Instance == null && SearchChildren)
                return ComponentToSearch.GetComponentInChildren<T>();

            return Instance;
        }
    }
}