using UnityEngine;

namespace ZeroProgress.Common.Behaviours.StateMachines
{
    public class StateExitTriggerParamSet : AnimParamSetterBase
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetTrigger(AnimationParameter.Value);
        }
    }
}