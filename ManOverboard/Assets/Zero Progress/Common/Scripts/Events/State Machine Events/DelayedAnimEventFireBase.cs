using UnityEngine;

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

        /// <summary>
        /// Intended for children to call to notify the scene of animation-specific events
        /// a short time after this is called
        /// </summary>
        /// <param name="Anim">The animator to fire the events in relation to</param>
        protected virtual void DelayedFireEvent(Animator Anim)
        {
            MonoBehaviour monoBehaviour = Anim.GetComponent<MonoBehaviour>();

            if (monoBehaviour == null)
            {
                Debug.LogError("Could not find a monobehaviour on " + Anim.name +
                    ". Cannot perform DelayedAction for " + EventsToFire);
            }

            monoBehaviour.DelayedExecution(DelayTimeSeconds, () => FireEvents(Anim), UseRealTimeSeconds);
        }
    }
}