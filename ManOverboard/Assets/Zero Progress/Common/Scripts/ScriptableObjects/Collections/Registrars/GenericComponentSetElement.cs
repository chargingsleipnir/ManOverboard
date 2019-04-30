using UnityEngine;

namespace ZeroProgress.Common.Collections
{
    /// <summary>
    /// Base class for helping with registering specific component types with
    /// a specific type of set
    /// </summary>
    /// <typeparam name="T">The type of the set</typeparam>
    /// <typeparam name="T1">The component type that the set should recieve</typeparam>
    public class GenericComponentSetElement<T, T1> : MonoBehaviour where T : ScriptableSet<T1> where T1 : Component 
    {
        [Tooltip("The set that will receive the component")]
        public T SetToAddTo;

        [Tooltip("The component to Add and Remove when this script is enabled or disabled")]
        [SerializeField]
        private T1 component;

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

            component = this.GetComponentIfNull<T1>(component);
            
            RegisterComponent(component);
        }

        private void OnDisable()
        {
            if (!UnregisterOnDisable)
                return;

            UnregisterComponent(component);
        }

        /// <summary>
        /// Registers the stored component
        /// </summary>
        public void RegisterComponent()
        {
            RegisterComponent(component);
        }

        /// <summary>
        /// Registers the specified component
        /// </summary>
        /// <param name="Component">The component to register</param>
        public void RegisterComponent(T1 Component)
        {
            if (Component != null)
                SetToAddTo.Add(Component);
        }

        /// <summary>
        /// Unregisters the stored component
        /// </summary>
        public void UnregisterComponent()
        {
            UnregisterComponent(component);
        }

        /// <summary>
        /// Unregisters the stored component
        /// </summary>
        /// <param name="Component">The component to register</param>
        public void UnregisterComponent(T1 Component)
        {
            if (Component != null)
                SetToAddTo.Remove(Component);
        }
    }
}