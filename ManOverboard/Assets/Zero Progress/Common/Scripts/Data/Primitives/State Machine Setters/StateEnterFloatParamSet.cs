using UnityEngine;

namespace ZeroProgress.Common.Behaviours.StateMachines
{
    public class StateEnterFloatParamSet : AnimParamSetterBase
    {
        public float ValueToSet = 0f;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetFloat(AnimationParameter.Value, ValueToSet);
        }
    }
}
