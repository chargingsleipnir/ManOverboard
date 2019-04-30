using UnityEngine;
using UnityEngine.Animations;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Base class for firing an animation event a short time after the necessary action occurs
    /// </summary>
    public class DelayedAnimEventFireBase : AnimEventFireBase
    {
        [Tooltip("The amount of seconds to wait after the triggering occurs")]
        public float DelayTimeSeconds = 1f;

        [Tooltip("True to compute the delay with real time, false to use scaled time")]
        public bool UseRealTimeSeconds = false;

        protected virtual void DelayedFireEvent(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            DelayedFireEvent(animator, stateInfo, layerIndex, AnimatorControllerPlayable.Null);
        }

        /// <summary>
        /// Intended for children to call to notify the scene of animation-specific events
        /// a short time after this is called
        /// </summary>
        /// <param name="Anim">The animator to fire the events in relation to</param>
        protected virtual void DelayedFireEvent(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            MonoBehaviour monoBehaviour = animator.GetComponent<MonoBehaviour>();

            if (monoBehaviour == null)
            {
                Debug.LogError("Could not find a monobehaviour on " + animator.name, this);
                return;
            }

            monoBehaviour.DelayedExecution(DelayTimeSeconds, () => FireEvents(animator, stateInfo, layerIndex, controller), UseRealTimeSeconds);
        }
    }
}