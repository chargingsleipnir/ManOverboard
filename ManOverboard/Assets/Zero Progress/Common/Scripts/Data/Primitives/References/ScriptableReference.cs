using System;
using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Simple mechanism for swapping between 'hard-coded' inspector values or
    /// asset values
    /// </summary>
    /// <typeparam name="T">The actual value type</typeparam>
    /// <typeparam name="T1">The ScriptablePrimitive that contains the type</typeparam>
    [Serializable]
    public abstract class ScriptableReference<T, T1> where T1 : ScriptablePrimitive<T>
    {
        [Tooltip("True to use a hard-coded value, false to use an asset value")]
        public bool UseStraightValue = true;

        [Tooltip("The 'hard-coded' value")]
        public T StraightValue;

        [Tooltip("The asset value")]
        public T1 ScriptableValue;

        /// <summary>
        /// Sets or retrieves the current value of the reference
        /// </summary>
        public virtual T Value
        {
            get
            {
                if (!UseStraightValue)
                {
                    if (ScriptableValue == null)
                        return default(T);
                    else
                        return ScriptableValue.CurrentValue;
                }

                return StraightValue;
            }
            set {
                if (!UseStraightValue) {
                    if (ScriptableValue != null)
                        ScriptableValue.CurrentValue = value;
                }
                else
                    StraightValue = value;
            }
        }

        /// <summary>
        /// Auto conversion to the value type to allow direct usage of this reference as if it were actually
        /// the type of T
        /// </summary>
        /// <param name="Reference">The reference to perform the implicit cast on</param>
        public static implicit operator T(ScriptableReference<T, T1> Reference)
        {
            return Reference.Value;
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ScriptableReference()
        {

        }

        /// <summary>
        /// Constructor that sets the straight value. Does not set UseStraightValue to true
        /// </summary>
        /// <param name="StartValue">The value to start the reference at, if it's a straight value</param>
        public ScriptableReference(T StartValue)
        {
            StraightValue = StartValue;
        }
    }
}