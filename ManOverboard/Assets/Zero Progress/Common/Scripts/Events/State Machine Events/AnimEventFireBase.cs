using System.Collections.Generic;
using UnityEngine;

namespace ZeroProgress.Common
{
    public class AnimEventFireBase : StateMachineBehaviour
    {
        [SerializeField]
        protected StringReference[] EventsToFire;

        [Tooltip("True to notify children of the game object as well")]
        public bool PropagateToChildren = false;

        [Tooltip("True to include children that have been deactivated, false to use only active ones")]
        public bool IncludeInactiveChildren = false;

        protected virtual void FireEvents(Animator Anim)
        {
            IEnumerable<IAnimationEventListener> animEvents = GetEventReceivers(Anim);

            int numNotified = 0;

            foreach (StringReference eventToFire in EventsToFire)
            {
                foreach (IAnimationEventListener animEvent in animEvents)
                {
                    animEvent.ExecuteEvent(eventToFire);
                    numNotified++;
                }
            }

            if (numNotified == 0)
            {
                Debug.LogError("No AnimationEventReceivers found on: " +
                    Anim.name + " for event: " + EventsToFire);
            }
        }

        protected virtual IEnumerable<IAnimationEventListener> GetEventReceivers(Animator Anim)
        {
            if (PropagateToChildren)
                return Anim.GetComponentsInChildren<IAnimationEventListener>(IncludeInactiveChildren);
            else
                return Anim.GetComponents<IAnimationEventListener>();
        }
    }
}