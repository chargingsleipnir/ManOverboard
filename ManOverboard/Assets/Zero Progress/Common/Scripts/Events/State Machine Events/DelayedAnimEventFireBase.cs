using UnityEngine;

namespace ZeroProgress.Common
{
    public class DelayedAnimEventFireBase : AnimEventFireBase
    {
        public float DelayTimeSeconds = 1f;

        public bool UseRealTimeSeconds = false;
        
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