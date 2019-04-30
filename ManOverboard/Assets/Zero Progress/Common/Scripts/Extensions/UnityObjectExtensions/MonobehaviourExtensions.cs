using System;
using System.Collections;
using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Extensions for the MonoBehaviour class
    /// </summary>
    public static class MonobehaviourExtensions
    {
        /// <summary>
        /// Executes an action on the next frame
        /// </summary>
        /// <param name="thisBehaviour">The behaviour to execute on</param>
        /// <param name="execution">The action to take on the next frame</param>
        /// <returns>The coroutine created to execute on next frame</returns>
        public static Coroutine OnNextFrame(this MonoBehaviour thisBehaviour, Action execution)
        {
            return thisBehaviour.StartCoroutine(DelayedAction(0f, execution));
        }

        /// <summary>
        /// Used to perform an action after a set amount of time
        /// </summary>
        /// <param name="ThisBehaviour">The behaviour to use as the host of the coroutine</param>
        /// <param name="TimeToWait">The amount of time to wait before executing the action</param>
        /// <param name="Execution">The action to be executed when the time passes</param>
        /// <param name="RealtimeSeconds">True to use WaitForSecondsRealtime, false to use WaitForSeconds</param>
        /// <returns>The coroutine used to run the code so you can cancel it if desired</returns>
        public static Coroutine DelayedExecution(this MonoBehaviour ThisBehaviour, 
            float TimeToWait, Action Execution, bool RealtimeSeconds = false)
        {
            return ThisBehaviour.StartCoroutine(DelayedAction(TimeToWait, Execution, RealtimeSeconds));
        }

        /// <summary>
        /// Used to perform an action at the end of this frame
        /// </summary>
        /// <param name="ThisBehaviour">The behaviour to use as the host of the coroutine</param>
        /// <param name="Execution">The action to be executed at the end of the frame</param>
        /// <returns>The coroutine used to run the code so you can cancel it if desired</returns>
        public static Coroutine DelayExecutionToFrameEnd(this MonoBehaviour ThisBehaviour, Action Execution)
        {
            return ThisBehaviour.StartCoroutine(EndOfFrameAction(Execution));
        }

        /// <summary>
        /// Searches this game object (and optionally its children) for a component of a specific type. If it
        /// doesn't exist, it gets created
        /// </summary>
        /// <typeparam name="T">The type of component to be retrieved or created</typeparam>
        /// <param name="ThisMonoBehaviour">The behaviour to perform this extension on</param>
        /// <param name="SearchChildren">True to search children for an instance of the component, false if not</param>
        /// <returns>The found or created component</returns>
        public static T GetOrAddComponent<T>(this MonoBehaviour ThisMonoBehaviour, bool SearchChildren = false) where T : Component
        {
            T foundComponent = null;

            if (SearchChildren)
                foundComponent = ThisMonoBehaviour.GetComponentInChildren<T>();
            else
                ThisMonoBehaviour.GetComponent<T>();

            if(foundComponent == null)
                foundComponent = ThisMonoBehaviour.gameObject.AddComponent<T>();

            return foundComponent;            
        }

        /// <summary>
        /// The coroutine used to wait for the delay before performing an action
        /// </summary>
        /// <param name="TimeToWait">The amount of time to wait before executing the action</param>
        /// <param name="Execution">The action to be executed when the time passes</param>
        /// <param name="RealtimeSeconds">True to use WaitForSecondsRealtime, false to use WaitForSeconds</param>
        /// <returns>IEnumerator</returns>
        private static IEnumerator DelayedAction(float TimeToWait, Action Execution, bool RealtimeSeconds = false)
        {
            if (RealtimeSeconds)
                yield return new WaitForSecondsRealtime(TimeToWait);
            else
                yield return new WaitForSeconds(TimeToWait);

            Execution();
        }

        /// <summary>
        /// The coroutine used to wait until the end of the current frame before performing an action
        /// </summary>
        /// <param name="Execution">The action to be executed at the end of the frame</param>
        /// <returns>IEnumerator</returns>
        private static IEnumerator EndOfFrameAction(Action Execution)
        {
            yield return new WaitForEndOfFrame();
            
            Execution();
        }

        /// <summary>
        /// Gets a component or records an error message to the debug console
        /// </summary>
        /// <typeparam name="T">The type of component to retrieve</typeparam>
        /// <param name="ThisMonoBehaviour">The behaviour to perform this on</param>
        /// <param name="ErrorMessage">The error message to be displayed if the component could not be found</param>
        /// <returns>The found component, or null</returns>
        public static T GetComponentOrError<T>(this MonoBehaviour ThisMonoBehaviour, string ErrorMessage) where T : Component
        {
            T foundComponent = ThisMonoBehaviour.GetComponent<T>();

            if (foundComponent == null)
                Debug.LogError(ErrorMessage);
            
            return foundComponent;
        }

        /// <summary>
        /// Gets a component from the children or records an error message to the debug console
        /// </summary>
        /// <typeparam name="T">The type of component to retrieve</typeparam>
        /// <param name="ThisMonoBehaviour">The behaviour to perform this on</param>
        /// <param name="ErrorMessage">The error message to be displayed if the component could not be found</param>
        /// <returns>The found component, or null</returns>
        public static T GetComponentInChildrenOrError<T>(this MonoBehaviour ThisMonoBehaviour, string ErrorMessage) where T : Component
        {
            T foundComponent = ThisMonoBehaviour.GetComponentInChildren<T>();

            if (foundComponent == null)
                Debug.LogError(ErrorMessage);

            return foundComponent;
        }

        /// <summary>
        /// Gets a component from the parent or records an error message to the debug console
        /// </summary>
        /// <typeparam name="T">The type of component to retrieve</typeparam>
        /// <param name="ThisMonoBehaviour">The behaviour to perform this on</param>
        /// <param name="ErrorMessage">The error message to be displayed if the component could not be found</param>
        /// <returns>The found component, or null</returns>
        public static T GetComponentInParentOrError<T>(this MonoBehaviour ThisMonoBehaviour, string ErrorMessage) where T : Component
        {
            T foundComponent = ThisMonoBehaviour.GetComponentInParent<T>();

            if (foundComponent == null)
                Debug.LogError(ErrorMessage);

            return foundComponent;
        }
    }
}
