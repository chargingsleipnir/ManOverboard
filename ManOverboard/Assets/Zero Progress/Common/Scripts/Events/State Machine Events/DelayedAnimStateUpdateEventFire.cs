using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Animation event fired a short time after a state is updated
    /// </summary>
    public class DelayedAnimStateUpdateEventFire : DelayedAnimEventFireBase
    {
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            DelayedFireEvent(animator, stateInfo, layerIndex);
        }
    }
}
