using UnityEngine;

namespace ZeroProgress.Common.Behaviours.StateMachines
{
    public class StateExitBoolParamSet : AnimParamSetterBase
    {
        public bool ValueToSet = false;

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool(AnimationParameter.Value, ValueToSet);
        }
    }
}
