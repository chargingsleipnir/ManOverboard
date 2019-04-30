using System;
using UnityEngine;
using ZeroProgress.Common;

namespace ZeroProgress.SceneManagementUtility
{
    /// <summary>
    /// Represents a variable that will be used by the Scene Management system
    /// </summary>
    [Serializable]
    internal sealed class SceneVariable : IVariable, IResetable, ISerializationCallbackReceiver
    {
        [Tooltip("The name of the variable")]
        public string Name;

        [Tooltip("True to reset the variable to the original value whenever a " +
            "transition occurs, false to not")]
        public bool ResetOnTransition = false;

        [SerializeField]
        [Tooltip("The value of the variable")]
        private ScriptableObject value;
        
        /// <summary>
        /// Get the value stored in this variable
        /// </summary>
        /// <returns>The value</returns>
        public object GetValue()
        {
            if (value == null)
                return null;

            return ((IVariable)value).GetValue();
        }

        /// <summary>
        /// Set the value stored in this variable
        /// </summary>
        /// <param name="newValue">The value to be set</param>
        public void SetValue(object newValue)
        {
            IVariable variable = value as IVariable;

            if (variable == null)
                return;

            variable.SetValue(newValue);
        }

        public void OnAfterDeserialize()
        {            
        }

        public void OnBeforeSerialize()
        {
            if (value == null)
                return;

            if (!(value is IVariable))
            {
                value = null;
                Debug.LogError("Variable value must implement IVariable");
            }
        }

        /// <summary>
        /// Method to set the container of the variable value
        /// </summary>
        /// <param name="newValue">The new container to be used</param>
        internal void SetContainer(ScriptableObject newValue)
        {
            IVariable variable = newValue as IVariable;

            if (variable == null)
                return;

            value = newValue;
        }

        internal Type GetContainerType()
        {
            if (value == null)
                return null;

            return value.GetType();
        }

        /// <summary>
        /// Determines if the contained scriptable object implements
        /// the resetable interface
        /// </summary>
        /// <returns>True if the contained type is resetable, false if not</returns>
        public bool CanReset()
        {
            return value != null && value is IResetable;
        }

        /// <summary>
        /// Reset the value of this scene variable if possible
        /// </summary>
        public void Reset()
        {
            if (CanReset())
                ((IResetable)value).Reset();
        }
    }
}