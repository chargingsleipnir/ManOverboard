using UnityEngine;

namespace ZeroProgress.Common.Behaviours.StateMachines
{
    /// <summary>
    /// Sets a trigger animation parameter when the state machine behaviour is entered
    /// </summary>
    public class StateEnterTriggerParamSet : AnimParamSetterBase
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetTrigger(AnimationParameter.Value);
        }
    }
}