using System;
using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Implementation of the ScriptableReference for the string type
    /// </summary>
    [Serializable]
    public class StringReference : ScriptableReference<string, ScriptableString>
    {
        public StringReference()
        {
        }

        public StringReference(String StartValue)
            :base(StartValue)
        {
        }
    }

    /// <summary>
    /// Implementation of the ScriptableReference for the float type
    /// </summary>
    [Serializable]
    public class FloatReference : ScriptableReference<float, ScriptableFloat>
    {
        public FloatReference()
        {
        }

        public FloatReference(float StartValue)
            : base(StartValue)
        {
        }
    }

    /// <summary>
    /// Implementation of the ScriptableReference for the int type
    /// </summary>
    [Serializable]
    public class IntReference : ScriptableReference<int, ScriptableInt>
    {
        public IntReference()
        {
        }
        
        public IntReference(int StartValue)
            : base(StartValue)
        {
        }
    }

    /// <summary>
    /// Implementation of the ScriptableReference for the Vector3 type
    /// </summary>
    [Serializable]
    public class Vector2Reference : ScriptableReference<Vector2, ScriptableVector2> {
        public Vector2Reference() {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="StartValue">The value</param>
        public Vector2Reference(Vector3 StartValue)
            : base(StartValue) {
        }
    }

    /// <summary>
    /// Implementation of the ScriptableReference for the Vector3 type
    /// </summary>
    [Serializable]
    public class Vector3Reference : ScriptableReference<Vector3, ScriptableVector3>
    {
        public Vector3Reference()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="StartValue">The value</param>
        public Vector3Reference(Vector3 StartValue)
            : base(StartValue)
        {
        }
    }

    /// <summary>
    /// Implementation of the ScriptableReference for the AnimationParameter type
    /// </summary>
    [Serializable]
    public class AnimParamReference : ScriptableReference<int, ScriptableAnimParam>
    {
#if UNITY_EDITOR
        /// <summary>
        /// Used to store the name of the animation parameter
        /// </summary>
        public string AnimParamName;
#endif

        public AnimParamReference()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="StartValue">The hash value of the animation parameter to store</param>
        public AnimParamReference(int StartValue)
            : base(StartValue)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="StartValue">The name of the animation parameter to hash</param>
        public AnimParamReference(String StartValue)
        {
            StraightValue = Animator.StringToHash(StartValue);

#if UNITY_EDITOR
            AnimParamName = StartValue;
#endif
        }
    }
}