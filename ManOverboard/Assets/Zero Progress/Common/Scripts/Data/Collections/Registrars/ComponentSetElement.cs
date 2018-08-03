using UnityEngine;

namespace ZeroProgress.Common.Collections
{
    public class ComponentSetElement : MonoBehaviour
    {
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

        public void RegisterComponent()
        {
            if (component != null)
                SetToAddTo.Add(component);
        }
        
        public void UnregisterComponent()
        {
            if (component != null)
                SetToAddTo.Remove(component);
        }
    }
}