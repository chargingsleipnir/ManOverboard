using UnityEngine;

namespace ZeroProgress.Common.Behaviours.StateMachines
{
    /// <summary>
    /// Sets a int animation parameter when the state machine behaviour is exited
    /// </summary>
    public class StateExitIntParamSet : AnimParamSetterBase
    {
        [Tooltip("The value to be assigned to the int animation parameter when the state is exited")]
        public IntReference ValueToSet = new IntReference(0);

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetInteger(AnimationParameter.Value, ValueToSet);
        }
    }
}