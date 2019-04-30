using System.Collections;
using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// A wrapper to manage ensuring only one instance of the provided
    /// coroutine is ran at a time
    /// </summary>
    public class SingleRunCoroutine
    {

        /// <summary>
        /// The coroutine that is currently running
        /// </summary>
        private Coroutine coroutine;

        /// <summary>
        /// The behaviour that the coroutine is executing on
        /// </summary>
        private MonoBehaviour coroutineOwner;

        /// <summary>
        /// True if the contained coroutine is currently running, false if not
        /// </summary>
        public bool IsRunning
        {
            get { return coroutine != null; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="owner">The monobehaviour that the coroutine will be attached to</param>
        public SingleRunCoroutine(MonoBehaviour owner)
        {
            coroutineOwner = owner;
        }

        /// <summary>
        /// Starts the coroutine if it isn't currently running
        /// </summary>
        /// <param name="coroutineFunction">The coroutine to execute</param>
        /// <param name="ForceReset">True to reset the execution if it's currently running, false
        /// to ignore this call</param>
        public void StartCoroutine(IEnumerator coroutineFunction, bool ForceReset = false)
        {
            if (coroutine != null)
            {
                if (ForceReset)
                    StopCoroutine();
                else
                    return;
            }

            coroutine = coroutineOwner.StartCoroutine(CoroutineExecutor(coroutineFunction));
        }

        /// <summary>
        /// Stops the currently running coroutine (if it is running)
        /// </summary>
        public void StopCoroutine()
        {
            if (coroutine == null)
                return;

            coroutineOwner.StopCoroutine(coroutine);
            coroutine = null;
        }

        /// <summary>
        /// Wrapper around the coroutine to be executed in order to 
        /// correctly set the current running state when it is completed
        /// </summary>
        /// <param name="coroutineToExecute">The coroutine to run</param>
        /// <returns>IEnumerator for the coroutine to receive</returns>
        private IEnumerator CoroutineExecutor(IEnumerator coroutineToExecute)
        {
            yield return coroutineOwner.StartCoroutine(coroutineToExecute);

            coroutine = null;
        }
    }
}
