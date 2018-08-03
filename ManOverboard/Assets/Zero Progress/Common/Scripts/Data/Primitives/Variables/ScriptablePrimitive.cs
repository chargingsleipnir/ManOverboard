using System;
using UnityEngine;

namespace ZeroProgress.Common
{
    [Serializable]
    public class ScriptablePrimitive<T>: ScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        public string Description = "";
#endif

        public T DefaultValue;

        [SerializeField]
        private T currentValue;

        public T CurrentValue
        {
            get { return currentValue; }
            set { currentValue = value; }
        }
        
        protected virtual void OnEnable()
        {
            ResetValue();
        }

        public virtual void ResetValue()
        {
            CurrentValue = DefaultValue;
        }
    }
}