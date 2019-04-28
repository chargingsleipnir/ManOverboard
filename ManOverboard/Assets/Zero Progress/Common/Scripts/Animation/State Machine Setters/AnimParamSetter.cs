using System;
using UnityEngine;

namespace ZeroProgress.Common
{
    public enum AnimParamType
    {
        BOOL,
        FLOAT,
        INT,
        TRIGGER,
    }

    [Serializable]
    public class AnimParamSetterValue
    {
        [Tooltip("The identifier for which animation parameter to be set")]
        public AnimParamReference AnimationParameter;

        [Tooltip("The type of parameter to be set")]
        public AnimParamType ParameterType;

        [Tooltip("The float value to be applied")]
        public float FloatValue;

        [Tooltip("The bool value to be applied")]
        public bool BoolValue;

        [Tooltip("The int value to be applied")]
        public int IntValue;
    }

    /// <summary>
    /// Base class for a state machine behaviour that sets an animation parameter value
    /// </summary>
    public class AnimParamSetter : StateMachineBehaviour
    {
        public AnimParamSetterValue[] OnEnterSetters;

        public AnimParamSetterValue[] OnExitSetters;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            foreach (AnimParamSetterValue setterValue in OnEnterSetters)
            {
                SetAnimatorParameter(animator, setterValue);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            foreach (AnimParamSetterValue setterValue in OnExitSetters)
            {
                SetAnimatorParameter(animator, setterValue);
            }
        }

        private void SetAnimatorParameter(Animator animator, AnimParamSetterValue setter)
        {
            switch (setter.ParameterType)
            {
                case AnimParamType.BOOL:
                    animator.SetBool(setter.AnimationParameter, setter.BoolValue);
                    break;
                case AnimParamType.FLOAT:
                    animator.SetFloat(setter.AnimationParameter, setter.FloatValue);
                    break;
                case AnimParamType.INT:
                    animator.SetInteger(setter.AnimationParameter, setter.IntValue);
                    break;
                case AnimParamType.TRIGGER:
                    if (setter.BoolValue)
                        animator.SetTrigger(setter.AnimationParameter);
                    else
                        animator.ResetTrigger(setter.AnimationParameter);
                    break;
            }
        }
    }
}