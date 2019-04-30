using System;
using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// The base class for all primitive assets
    /// </summary>
    /// <typeparam name="T">The type to make a scriptable asset from</typeparam>
    [Serializable]
    public class ScriptablePrimitive<T>: ScriptableObject, IVariable<T>, IResetable
    {
        [TextArea]
        [Tooltip("Development-Only text block to describe what this assets' purpose is")]
        public string Description = "";

        [Tooltip("The value to set the asset to when this asset 'starts' up")]
        public T DefaultValue;

        [SerializeField]
        [Tooltip("The current value of this asset")]
        private T currentValue;

        /// <summary>
        /// The current value of the asset
        /// </summary>
        public T CurrentValue
        {
            get { return currentValue; }
            set { currentValue = value; }
        }
        
        protected virtual void OnEnable()
        {
            Reset();
        }

        /// <summary>
        /// Sets the current value to the default/starting value
        /// </summary>
        public virtual void Reset()
        {
            CurrentValue = DefaultValue;
        }

        /// <summary>
        /// Determines if this primitive is resetable
        /// </summary>
        /// <returns>True</returns>
        public virtual bool CanReset()
        {
            return true;
        }

        #region IVariable Interface Implementation

        /// <summary>
        /// Get the value of the variable
        /// </summary>
        /// <returns>The current value</returns>
        public T GetTypedValue()
        {
            return CurrentValue;
        }

        /// <summary>
        /// Sets the value of the variable
        /// </summary>
        /// <param name="newValue">The new value</param>
        public void SetValue(T newValue)
        {
            CurrentValue = newValue;
        }

        /// <summary>
        /// Get the value of the variable
        /// </summary>
        /// <returns>The current value</returns>
        public object GetValue()
        {
            return CurrentValue;
        }

        /// <summary>
        /// Sets the value of the variable
        /// </summary>
        /// <param name="newValue">The new value</param>
        public void SetValue(object newValue)
        {
            if (newValue is T)
                SetValue((T)newValue);
        }
        
        #endregion

        /// <summary>
        /// Implicitly cast to the contained value type
        /// </summary>
        /// <param name="value">The value to cast</param>
        public static implicit operator T(ScriptablePrimitive<T> value)
        {
            return value.CurrentValue;
        }
    }
}