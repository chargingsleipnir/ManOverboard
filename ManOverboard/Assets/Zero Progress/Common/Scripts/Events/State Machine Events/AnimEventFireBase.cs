using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Base class for firing events associated with the animation state machine
    /// </summary>
    public class AnimEventFireBase : StateMachineBehaviour
    {
        [SerializeField]
        [Tooltip("The event identifiers to be fired")]
        protected StringReference[] EventsToFire;

        [Tooltip("True to notify children of the game object as well")]
        public bool PropagateToChildren = false;

        [Tooltip("True to include children that have been deactivated, false to use only active ones")]
        public bool IncludeInactiveChildren = false;

        protected virtual void FireEvents(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            FireEvents(animator, stateInfo, layerIndex, AnimatorControllerPlayable.Null);
        }

        /// <summary>
        /// Intended for children to call to notify the scene of animation-specific events
        /// </summary>
        /// <param name="Anim">The animator to fire the events in relation to</param>
        protected virtual void FireEvents(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            IEnumerable<IAnimationEventListener> animEvents = GetEventReceivers(animator);

            StateMachineEventArgs args = new StateMachineEventArgs(animator, stateInfo, layerIndex, controller);

            int numNotified = 0;

            foreach (StringReference eventToFire in EventsToFire)
            {
                foreach (IAnimationEventListener animEvent in animEvents)
                {
                    animEvent.ReceiveEvent(eventToFire, args);
                    numNotified++;
                }
            }

            if (numNotified == 0)
            {
                Debug.LogError("No AnimationEventReceivers found on: " + animator.name, this);
            }
        }

        /// <summary>
        /// Retrieves all event receivers on the provided animator item
        /// </summary>
        /// <param name="Anim">The animator to find all listeners on</param>
        /// <returns>Collection of event receivers to notify</returns>
        protected virtual IEnumerable<IAnimationEventListener> GetEventReceivers(Animator Anim)
        {
            if (PropagateToChildren)
                return Anim.GetComponentsInChildren<IAnimationEventListener>(IncludeInactiveChildren);
            else
                return Anim.GetComponents<IAnimationEventListener>();
        }
    }
}