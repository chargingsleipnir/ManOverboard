using System;
using UnityEngine;
using UnityEngine.Events;

namespace ZeroProgress.Common
{
    /// <summary>
    /// A ScriptableReference with an event included to signify when the value has changed
    /// </summary>
    /// <typeparam name="ValueType">The type stored within the primitive/reference</typeparam>
    /// <typeparam name="ScriptablePrimitiveType">The type of scriptable primitive that represents the value type as an asset</typeparam>
    /// <typeparam name="EventType">The event type that will be invoked when the value changes</typeparam>
    [Serializable]
    public class NotifyReference<ValueType, ScriptablePrimitiveType, EventType, EventArgType> : ScriptableReference<ValueType, ScriptablePrimitiveType> 
        where ScriptablePrimitiveType : ScriptablePrimitive<ValueType> 
        where EventType : UnityEvent<EventArgType>
        where EventArgType : ValueChangedEventArgs<ValueType>, new()
    {
        [Tooltip("Event invoked when a value change has occured")]
        public EventType OnValueChanged;

        /// <summary>
        /// Sets or retrieves the current value
        /// </summary>
        public override ValueType Value
        {
            get
            {
                return base.Value;
            }

            set
            {
                if (base.Value.Equals(value))
                    return;

                EventArgType changeArgs = new EventArgType();
                changeArgs.OldValue = Value;
                changeArgs.NewValue = value;

                base.Value = value;

                OnValueChanged.SafeInvoke(changeArgs);
            }
        }

    }
}