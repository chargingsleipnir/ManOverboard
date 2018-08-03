using UnityEngine;

namespace ZeroProgress.Common
{
    public class DelayedAnimStateUpdateEventFire : DelayedAnimEventFireBase
    {
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            DelayedFireEvent(animator);
        }
    }
}
