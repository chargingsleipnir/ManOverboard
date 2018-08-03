using UnityEngine;

namespace ZeroProgress.Common.Behaviours.StateMachines
{
    public class StateEnterIntParamSet : AnimParamSetterBase
    {
        public int ValueToSet = 0;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetInteger(AnimationParameter.Value, ValueToSet);
        }
    }
}