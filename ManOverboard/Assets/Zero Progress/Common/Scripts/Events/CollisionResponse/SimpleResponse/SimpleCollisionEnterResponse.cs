using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Activates a UnityEvent in response to a collision or trigger being entered
    /// </summary>
    public class SimpleCollisionEnterResponse : SimpleCollisionResponseBase
    {
        protected virtual void OnCollisionEnter(Collision collision)
        {
            if(!OnTrigger)
                TryInvokeResponse(collision.gameObject);
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (OnTrigger)
                TryInvokeResponse(other.gameObject);
        }
    }
}