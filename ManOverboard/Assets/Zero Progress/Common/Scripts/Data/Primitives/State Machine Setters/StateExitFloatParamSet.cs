using UnityEngine;

namespace ZeroProgress.Common.Behaviours.StateMachines
{
    public class StateExitFloatParamSet : AnimParamSetterBase
    {
        public float ValueToSet = 0f;

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetFloat(AnimationParameter.Value, ValueToSet);
        }
    }
}