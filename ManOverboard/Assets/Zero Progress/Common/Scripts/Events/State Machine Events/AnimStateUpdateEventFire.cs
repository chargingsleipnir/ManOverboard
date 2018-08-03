using UnityEngine;

namespace ZeroProgress.Common
{
    public class AnimStateUpdateEventFire : AnimEventFireBase
    {
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            FireEvents(animator);
        }
    }
}