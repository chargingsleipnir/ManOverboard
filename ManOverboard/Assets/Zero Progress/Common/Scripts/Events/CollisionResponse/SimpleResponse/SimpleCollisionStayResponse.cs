using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Activates a UnityEvent in response to a collision or trigger stay
    /// </summary>
    public class SimpleCollisionStayResponse : SimpleCollisionResponseBase
    {
        // Unity or PhysX doesn't actually trigger CollisionStay correctly. After a few executions it
        // just stops firing. Therefore, a custom collision stay has been implemented that activates
        // during OnCollisionEnter and deactivates during OnCollisionExit
        // https://stackoverflow.com/questions/47040318/rigidbodies-touching-but-oncollisionstay-stops-being-called-unity3d?utm_medium=organic&utm_source=google_rich_qa&utm_campaign=google_rich_qa

        /// <summary>
        /// Tracker for coroutine execution state
        /// </summary>
        protected Coroutine collisionStayCoroutine = null;

        /// <summary>
        /// Collection of cached objects that tells us if OnCollisionStay event 
        /// should be invoked or not
        /// </summary>
        protected HashSet<GameObject> collisionObjects = new HashSet<GameObject>();

        [Tooltip("The number of frames to wait before pruning any destroyed objects from " +
            "the collider cache")]
        public int CacheCleanIndex = 5;

        protected virtual void OnCollisionEnter(Collision collision)
        {
            // Don't continue if we are only interested in trigger collisions
            if (OnTrigger)
                return;

            if (!FilterCheck(collision.gameObject))
                return;

            collisionObjects.Add(collision.gameObject);

            if (collisionStayCoroutine == null)
                collisionStayCoroutine = StartCoroutine(CollisionStayAction());
        }

        protected virtual void OnCollisionExit(Collision collision)
        {
            if (OnTrigger)
                return;

            if (!FilterCheck(collision.gameObject))
                return;

            collisionObjects.Remove(collision.gameObject);            
        }
        
        protected virtual void OnTriggerEnter(Collider trigger)
        {
            if (!OnTrigger)
                return;

            if (!FilterCheck(trigger.gameObject))
                return;

            collisionObjects.Add(trigger.gameObject);

            if (collisionStayCoroutine == null)
                collisionStayCoroutine = StartCoroutine(CollisionStayAction());
        }

        protected virtual void OnTriggerExit(Collider trigger)
        {
            if (!OnTrigger)
                return;

            if (!FilterCheck(trigger.gameObject))
                return;

            collisionObjects.Remove(trigger.gameObject);
        }

        /// <summary>
        /// The actual event invoking logic
        /// </summary>
        /// <returns>Enumerator</returns>
        protected virtual IEnumerator CollisionStayAction()
        {
            int frameCounter = 0;

            while (collisionObjects.Count != 0)
            {
                // Wait for all colliders to be processed in a single frame
                yield return new WaitForEndOfFrame();
                InvokeResponse();
                yield return ZeroProgressConstants.WAIT_NEXT_FRAME;
                
                frameCounter++;

                if (frameCounter >= CacheCleanIndex)
                {
                    CleanCollisionCache();
                    frameCounter = 0;
                }
            }

            collisionStayCoroutine = null;
        }

        /// <summary>
        /// Clears out any objects that have been destroyed from the collision cache
        /// </summary>
        protected virtual void CleanCollisionCache()
        {
            collisionObjects.RemoveWhere(gameObject => gameObject == null);
        }
    }
}