using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Animation event fired when a state is updating
    /// </summary>
    public class AnimStateUpdateEventFire : AnimEventFireBase
    {
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            FireEvents(animator);
        }
    }
}