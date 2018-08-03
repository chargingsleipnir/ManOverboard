using UnityEngine;

namespace ZeroProgress.Common
{
    public class AnimStateEnterEventFire : AnimEventFireBase
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            FireEvents(animator);
        }
    }
}