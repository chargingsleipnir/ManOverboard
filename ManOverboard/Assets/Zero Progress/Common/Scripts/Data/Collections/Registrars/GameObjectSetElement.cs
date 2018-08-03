using UnityEngine;

namespace ZeroProgress.Common.Collections
{
    public class GameObjectSetElement : MonoBehaviour
    {
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

        public void RegisterGameObject()
        {
            RegisterGameObject(managedGameObject);
        }

        public void RegisterGameObject(GameObject GameObject)
        {
            if (GameObject != null)
                SetToAddTo.Add(GameObject);
        }

        public void UnregisterGameObject()
        {
            UnregisterGameObject(managedGameObject);
        }

        public void UnregisterGameObject(GameObject GameObject)
        {
            if (GameObject != null)
                SetToAddTo.Remove(GameObject);
        }
    }
}