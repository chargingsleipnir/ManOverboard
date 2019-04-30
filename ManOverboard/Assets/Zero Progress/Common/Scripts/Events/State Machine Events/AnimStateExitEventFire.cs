using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Fires an animation event when the state is exited
    /// </summary>
    public class AnimStateExitEventFire : AnimEventFireBase
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            FireEvents(animator, stateInfo, layerIndex);
        }
    }
}