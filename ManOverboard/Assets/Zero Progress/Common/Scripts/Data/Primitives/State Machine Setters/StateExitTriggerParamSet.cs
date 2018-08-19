using UnityEngine;

namespace ZeroProgress.Common.Behaviours.StateMachines
{
    /// <summary>
    /// Sets a trigger animation parameter when the state machine behaviour is exited
    /// </summary>
    public class StateExitTriggerParamSet : AnimParamSetterBase
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetTrigger(AnimationParameter.Value);
        }
    }
}