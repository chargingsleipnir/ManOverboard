using UnityEngine;

namespace ZeroProgress.Common
{
    public class DelayedAnimStateEnterEventFire : DelayedAnimEventFireBase
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            DelayedFireEvent(animator);
        }
    }
}
