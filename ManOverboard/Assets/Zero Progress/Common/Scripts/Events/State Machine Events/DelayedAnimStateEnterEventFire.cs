using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Animation event fired a short time after a state is entered
    /// </summary>
    public class DelayedAnimStateEnterEventFire : DelayedAnimEventFireBase
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            DelayedFireEvent(animator);
        }
    }
}
