using UnityEngine;

namespace ZeroProgress.Common
{
    public class DelayedAnimStateExitEventFire : DelayedAnimEventFireBase
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            DelayedFireEvent(animator);
        }

    }
}