using UnityEngine;

namespace ZeroProgress.Common.Behaviours.StateMachines
{
    /// <summary>
    /// Sets a float animation parameter when the state machine behaviour is entered
    /// </summary>
    public class StateEnterFloatParamSet : AnimParamSetterBase
    {
        [Tooltip("The value to be assigned to the float animation parameter when the state is entered")]
        public FloatReference ValueToSet = new FloatReference(0f);

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetFloat(AnimationParameter.Value, ValueToSet);
        }
    }
}
