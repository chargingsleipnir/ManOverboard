using UnityEngine;

namespace ZeroProgress.Common.Collections
{
    public class GenericComponentSetElement<T> : MonoBehaviour where T : Component 
    {
        public ScriptableSet<T> SetToAddTo;

        [Tooltip("The component to Add and Remove when this script is enabled or disabled")]
        [SerializeField]
        private T component;

        [Tooltip("True to register the specified component OnEnable (if component is null," +
            " will find one OnEnable if true). False to Register/Unregister manually")]
        public bool RegisterOnEnable = true;

        [Tooltip("True to unregister the specified component OnDisable (if component is null," +
            " will find one OnDisable if true). False to Register/Unregister manually")]
        public bool UnregisterOnDisable = true;

        private void OnEnable()
        {
            if (!RegisterOnEnable)
                return;

            if(component == null)
                component = GetComponent<T>();

            RegisterComponent(component);
        }

        private void OnDisable()
        {
            if (!UnregisterOnDisable)
                return;

            UnregisterComponent(component);
        }

        public void RegisterComponent()
        {
            RegisterComponent(component);
        }

        public void RegisterComponent(T Component)
        {
            if (Component != null)
                SetToAddTo.Add(Component);
        }

        public void UnregisterComponent()
        {
            UnregisterComponent(component);
        }

        public void UnregisterComponent(T Component)
        {
            if (Component != null)
                SetToAddTo.Remove(Component);
        }
    }
}