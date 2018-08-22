using UnityEngine;

namespace ZeroProgress.Common.Behaviours.StateMachines
{
    /// <summary>
    /// Sets a bool animation parameter when the state machine behaviour is exited
    /// </summary>
    public class StateExitBoolParamSet : AnimParamSetterBase
    {
        [Tooltip("The value to be assigned to the bool animation parameter when the state is exited")]
        public bool ValueToSet = false;

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool(AnimationParameter.Value, ValueToSet);
        }
    }
}
