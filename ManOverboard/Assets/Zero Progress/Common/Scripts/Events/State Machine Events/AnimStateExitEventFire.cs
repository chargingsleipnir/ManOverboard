using UnityEngine;

namespace ZeroProgress.Common
{
    public class AnimStateExitEventFire : AnimEventFireBase
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            FireEvents(animator);
        }
    }
}