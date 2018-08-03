using UnityEngine;

namespace ZeroProgress.Common.Behaviours.StateMachines
{
    public class StateExitIntParamSet : AnimParamSetterBase
    {
        public int ValueToSet = 0;

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetInteger(AnimationParameter.Value, ValueToSet);
        }
    }
}