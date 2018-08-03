using UnityEngine;

namespace ZeroProgress.Common.Behaviours.StateMachines
{
    public class StateEnterTriggerParamSet : AnimParamSetterBase
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetTrigger(AnimationParameter.Value);
        }
    }
}