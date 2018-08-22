using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Activates a UnityEvent in response to a collision or trigger being exited
    /// </summary>
    public class SimpleCollisionExitResponse : SimpleCollisionResponseBase
    {
        protected virtual void OnCollisionExit(Collision collision)
        {
            if(!OnTrigger)
                TryInvokeResponse(collision.gameObject);
        }

        protected virtual void OnTriggerExit(Collider trigger)
        {
            if(OnTrigger)
                TryInvokeResponse(trigger.gameObject);
        }
    }
}