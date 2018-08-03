using UnityEngine;

namespace ZeroProgress.Common.Behaviours.StateMachines
{
    public class StateEnterBoolParamSet : AnimParamSetterBase
    {
        public bool ValueToSet = true;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool(AnimationParameter.Value, ValueToSet);
        }
    }
}