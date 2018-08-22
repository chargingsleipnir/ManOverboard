using UnityEngine;

namespace ZeroProgress.Common.Collections
{
    /// <summary>
    /// Helper component that registers another component to the specified set
    /// </summary>
    public class ComponentSetElement : MonoBehaviour
    {
        [Tooltip("The collection to register the component to")]
        public ComponentSet SetToAddTo;

        [Tooltip("The component to Add and Remove when this script is enabled or disabled")]
        [SerializeField]
        private Component component;
        
        private void OnEnable()
        {
            if (component == null)
                Debug.LogError("Component is null, cannot register to set on: " + this.gameObject.name);

            RegisterComponent();
        }

        private void OnDisable()
        {
            if (component == null)
                Debug.LogError("Component is null, cannot unregister from set on: " + this.gameObject.name);

            UnregisterComponent();
        }

        /// <summary>
        /// Registers the specified component to the specified collection
        /// </summary>
        public void RegisterComponent()
        {
            if (component != null)
                SetToAddTo.Add(component);
        }
        
        /// <summary>
        /// Unregisters the specified component from the specified collection
        /// </summary>
        public void UnregisterComponent()
        {
            if (component != null)
                SetToAddTo.Remove(component);
        }
    }
}