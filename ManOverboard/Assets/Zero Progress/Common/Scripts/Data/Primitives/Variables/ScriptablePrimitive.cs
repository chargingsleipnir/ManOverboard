using System;
using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// The base class for all primitive assets
    /// </summary>
    /// <typeparam name="T">The type to make a scriptable asset from</typeparam>
    [Serializable]
    public class ScriptablePrimitive<T>: ScriptableObject
    {
#if UNITY_EDITOR
        [TextArea]
        [Tooltip("Development-Only text block to describe what this assets' purpose is")]
        public string Description = "";
#endif

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
            ResetValue();
        }

        /// <summary>
        /// Sets the current value to the default/starting value
        /// </summary>
        public virtual void ResetValue()
        {
            CurrentValue = DefaultValue;
        }

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