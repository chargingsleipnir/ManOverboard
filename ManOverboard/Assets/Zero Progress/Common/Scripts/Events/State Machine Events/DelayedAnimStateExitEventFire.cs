using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Animation event fired a short time after a state is exited
    /// </summary>
    public class DelayedAnimStateExitEventFire : DelayedAnimEventFireBase
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            DelayedFireEvent(animator);
        }

    }
}