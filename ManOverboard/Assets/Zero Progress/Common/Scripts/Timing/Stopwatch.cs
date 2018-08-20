using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace ZeroProgress.Common.Timing
{
    /// <summary>
    /// Reusable stopwatch (counts up until stopped)
    /// </summary>
    public class Stopwatch : MonoBehaviour
    {
        [Tooltip("True to start this stopwatch when this component is Started")]
        public bool StartStopwatchOnStart = false;

        [Tooltip("True to start this stopwatch when this component is Enabled")]
        public bool StartStopwatchOnEnable = false;

        [Tooltip("When to fire an interval event")]
        public float NotificationInterval = 1f;

        [Tooltip("True to use real time, false to use scaled time")]
        public bool UseWaitSecondsRealTime = false;

        [Tooltip("Event response for when the stopwatch is started")]
        public UnityEvent OnStopwatchStarted;

        [Tooltip("Event response for when the stopwatch is restarted")]
        public UnityEvent OnStopwatchRestarted;

        [Tooltip("Event response for when the designated interval is reached")]
        public UnityFloatEvent OnIntervalElapsed;

        [Tooltip("Event response for when the stopwatch is stopped")]
        public UnityEvent OnStopwatchStopped;

        /// <summary>
        /// Coroutine that runs the stopwatch
        /// </summary>
        protected Coroutine stopwatchCoroutine = null;

        /// <summary>
        /// The number of seconds that have passed so far
        /// </summary>
        protected float secondsPassed = 0f;

        /// <summary>
        /// True if the stopwatch is running, false if not
        /// </summary>
        public virtual bool IsStopwatchRunning
        {
            get { return stopwatchCoroutine != null; }
        }

        public virtual void OnEnable()
        {
            if (StartStopwatchOnEnable)
                StartStopwatch();
        }

        public virtual void Start()
        {
            if (StartStopwatchOnStart)
                StartStopwatch();
        }

        public virtual void OnDisable()
        {
            StopStopwatch();
        }

        /// <summary>
        /// Starts the stopwatch. Does not reset the current value
        /// </summary>
        public virtual void StartStopwatch()
        {
            if (IsStopwatchRunning)
                return;

            stopwatchCoroutine = StartCoroutine(StopwatchFunctionality());
            OnStopwatchStarted.SafeInvoke();
        }

        /// <summary>
        /// Resets the stopwatch value
        /// </summary>
        /// <param name="StartIfNotRunning">True to start the stopwatch if it isn't currently running</param>
        public virtual void ResetStopwatch(bool StartIfNotRunning = true)
        {
            secondsPassed = 0f;

            if (!IsStopwatchRunning && StartIfNotRunning)
                StartStopwatch();
            else
                OnStopwatchRestarted.SafeInvoke();

            OnIntervalElapsed.SafeInvoke(secondsPassed);
        }

        /// <summary>
        /// Stops the stopwatchs. Does not reset the current value
        /// </summary>
        public virtual void StopStopwatch()
        {
            if (!IsStopwatchRunning)
                return;

            StopCoroutine(stopwatchCoroutine);
            stopwatchCoroutine = null;

            OnStopwatchStopped.SafeInvoke();
        }

        /// <summary>
        /// The logic that drives the stopwatch
        /// </summary>
        /// <returns>Enumerator</returns>
        protected virtual IEnumerator StopwatchFunctionality()
        {
            OnIntervalElapsed.SafeInvoke(secondsPassed);

            while (true)
            {
                float interval = NotificationInterval;

                if (UseWaitSecondsRealTime)
                    yield return new WaitForSecondsRealtime(interval);
                else
                    yield return new WaitForSeconds(interval);

                secondsPassed += interval;
                OnIntervalElapsed.SafeInvoke(secondsPassed);
            }            
        }
    }
}