using UnityEngine;

namespace ZeroProgress.Common.Collections
{
    /// <summary>
    /// Helper component that registers another component to the specified set
    /// </summary>
    public class GameObjectSetElement : MonoBehaviour
    {
        [Tooltip("The collection to register the game object to")]
        public GameObjectSet SetToAddTo;

        [Tooltip("The game object to Add and Remove when this script is enabled or disabled")]
        [SerializeField]
        private GameObject managedGameObject;

        [Tooltip("True to register the specified game object OnEnable (if game object is null," +
            " will use the object this component is attached to). False to Register/Unregister manually")]
        public bool RegisterOnEnable = true;

        [Tooltip("True to unregister the specified game object OnDisable (if game object is null," +
            " will use the object this component is attached to). False to Register/Unregister manually")]
        public bool UnregisterOnDisable = true;

        private void OnEnable()
        {
            if (!RegisterOnEnable)
                return;

            if (managedGameObject == null)
                managedGameObject = this.gameObject;

            RegisterGameObject(managedGameObject);
        }

        private void OnDisable()
        {
            if (!UnregisterOnDisable)
                return;

            UnregisterGameObject(managedGameObject);
        }

        /// <summary>
        /// Registers the game object specified through the inspector
        /// </summary>
        public void RegisterGameObject()
        {
            RegisterGameObject(managedGameObject);
        }

        /// <summary>
        /// Registers the specified game object
        /// </summary>
        /// <param name="GameObject">The object to register to the set being pointed at</param>
        public void RegisterGameObject(GameObject GameObject)
        {
            if (GameObject != null)
                SetToAddTo.Add(GameObject);
        }

        /// <summary>
        /// Unregisters the game object specified through the inspector
        /// </summary>
        public void UnregisterGameObject()
        {
            UnregisterGameObject(managedGameObject);
        }

        /// <summary>
        /// Unregisters the specified game object
        /// </summary>
        /// <param name="GameObject">The game object to unregister</param>
        public void UnregisterGameObject(GameObject GameObject)
        {
            if (GameObject != null)
                SetToAddTo.Remove(GameObject);
        }
    }
}