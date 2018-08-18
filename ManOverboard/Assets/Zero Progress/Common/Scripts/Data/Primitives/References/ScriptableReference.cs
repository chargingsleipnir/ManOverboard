using System;
using UnityEngine;

namespace ZeroProgress.Common
{
    [Serializable]
    public abstract class ScriptableReference<T, T1> where T1 : ScriptablePrimitive<T>
    {
        public bool UseStraightValue = true;

        public T StraightValue;

        public T1 ScriptableValue;

        public T Value
        {
            get
            {
                if(!UseStraightValue)
                {
                    if (ScriptableValue == null)
                        Debug.LogError("Cannot use scriptable value, it's null");
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

        public static implicit operator T(ScriptableReference<T, T1> Reference)
        {
            return Reference.Value;
        }

        public ScriptableReference()
        {

        }

        public ScriptableReference(T StartValue)
        {
            StraightValue = StartValue;
        }
    }
}