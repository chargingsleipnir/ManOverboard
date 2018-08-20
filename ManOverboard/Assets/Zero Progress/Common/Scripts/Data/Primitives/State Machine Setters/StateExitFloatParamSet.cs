using UnityEngine;

namespace ZeroProgress.Common.Behaviours.StateMachines
{
    /// <summary>
    /// Sets a float animation parameter when the state machine behaviour is exited
    /// </summary>
    public class StateExitFloatParamSet : AnimParamSetterBase
    {
        [Tooltip("The value to be assigned to the bool animation parameter when the state is exited")]
        public FloatReference ValueToSet = new FloatReference(0f);

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetFloat(AnimationParameter.Value, ValueToSet);
        }
    }
}