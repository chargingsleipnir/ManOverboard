using UnityEngine;

namespace ZeroProgress.Common.Behaviours.StateMachines
{
    /// <summary>
    /// Sets a boolean value when an animation state is entered
    /// </summary>
    public class StateEnterBoolParamSet : AnimParamSetterBase
    {
        [Tooltip("The value to assign to the boolean animation parameter")]
        public bool ValueToSet = true;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool(AnimationParameter.Value, ValueToSet);
        }
    }
}