using UnityEngine;

namespace ZeroProgress.Common.Behaviours.StateMachines
{
    /// <summary>
    /// Sets an int animation parameter when the state machine behaviour is entered
    /// </summary>
    public class StateEnterIntParamSet : AnimParamSetterBase
    {
        [Tooltip("The value to be assigned to the int animation parameter when the state is entered")]
        public IntReference ValueToSet = new IntReference(0);

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetInteger(AnimationParameter.Value, ValueToSet);
        }
    }
}