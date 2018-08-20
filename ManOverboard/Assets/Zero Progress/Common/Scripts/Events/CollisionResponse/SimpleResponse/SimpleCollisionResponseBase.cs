using UnityEngine;
using UnityEngine.Events;

namespace ZeroProgress.Common
{
    /// <summary>
    /// The base class to be used for the simple collision response (fire without passing any collision information)
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public abstract class SimpleCollisionResponseBase : MonoBehaviour
    {
        [Tooltip("True to have the response activated on the trigger values")]
        public bool OnTrigger = false;

        [Tooltip("The response to be fired as a response to the collision activation")]
        public UnityEvent CollisionResponse;

        [Tooltip("A filter that can be applied to the collisions' gameobject to fine-tune what " +
            "triggers the collision response")]
        public GameObjectFilterer CollisionFilter;

        /// <summary>
        /// Handles invoking the collision response for children
        /// </summary>
        protected virtual void InvokeResponse()
        {
            CollisionResponse.SafeInvoke();
        }

        /// <summary>
        /// Handles invoking the collision response for children
        /// while taking into account the set filter
        /// </summary>
        /// <param name="CollidedObject">The object to pass through the filtering logic</param>
        protected virtual void TryInvokeResponse(GameObject CollidedObject)
        {
            if (!FilterCheck(CollidedObject))
                return;

            InvokeResponse();
        }

        /// <summary>
        /// Helper to run a collided-with game object through the set filters
        /// </summary>
        /// <param name="ObjectToCheck">The object to run through the filter</param>
        /// <returns>True if the object passes filtering, false if not</returns>
        protected virtual bool FilterCheck(GameObject ObjectToCheck)
        {
            if (CollisionFilter == null)
                return true;

            return CollisionFilter.IsValidItem(ObjectToCheck);
        }
    }
}