using System;
using UnityEngine;

namespace ZeroProgress.Common
{
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

    [Serializable]
    public class Vector3Reference : ScriptableReference<Vector3, ScriptableVector3>
    {
        public Vector3Reference()
        {
        }

        public Vector3Reference(Vector3 StartValue)
            : base(StartValue)
        {
        }
    }

    [Serializable]
    public class AnimParamReference : ScriptableReference<int, ScriptableAnimParam>
    {
#if UNITY_EDITOR
        public string AnimParamName;
#endif

        public AnimParamReference()
        {
        }

        public AnimParamReference(int StartValue)
            : base(StartValue)
        {
        }

        public AnimParamReference(String StartValue)
        {
            StraightValue = Animator.StringToHash(StartValue);

#if UNITY_EDITOR
            AnimParamName = StartValue;
#endif
        }
    }
}