using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Fires an animation event when the state is entered
    /// </summary>
    public class AnimStateEnterEventFire : AnimEventFireBase
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            FireEvents(animator);
        }
    }
}