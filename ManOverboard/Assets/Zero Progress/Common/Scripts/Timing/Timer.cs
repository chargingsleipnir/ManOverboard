using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace ZeroProgress.Common.Timing
{
    /// <summary>
    /// The base timer class
    /// </summary>
    public class Timer : MonoBehaviour
    {
        [Tooltip("How long the timer should run for")]
        public float Duration = 10f;

        [Tooltip("True to start the timer on the components Start")]
        public bool StartTimerOnStart = false;

        [Tooltip("True to start the timer on the components Enabled")]
        public bool StartTimerOnEnable = false;

        [Tooltip("True to use real time, false to use scaled time")]
        public bool UseWaitSecondsRealTime = false;

        [Tooltip("Response for when the timer reaches 0")]
        public UnityEvent OnTimerElapsed;

        [Tooltip("Response for when the timer is started")]
        public UnityEvent OnTimerStarted;

        [Tooltip("Response for when the timer is stopped")]
        public UnityEvent OnTimerStopped;

        /// <summary>
        /// The current timer execution
        /// </summary>
        protected Coroutine timerCoroutine = null;
        
        /// <summary>
        /// True if the timer is running, false if not
        /// </summary>
        public virtual bool IsTimerRunning
        {
            get { return timerCoroutine != null; }
        }

        public virtual void OnEnable()
        {
            if (StartTimerOnEnable)
                StartTimer();
        }

        public virtual void Start()
        {
            if (StartTimerOnStart)
                StartTimer();
        }

        public virtual void OnDisable()
        {
            StopTimer();
        }

        /// <summary>
        /// Starts the timer
        /// </summary>
        /// <param name="ResetIfRunning">True to reset the timer if it's currently running, false to ignore this call</param>
        public virtual void StartTimer(bool ResetIfRunning = false)
        {
            if(IsTimerRunning)
            {
                if (ResetIfRunning)
                    StopTimer();
                else
                    return;
            }

            timerCoroutine = StartCoroutine(TimerFunctionality(Duration));
            OnTimerStarted.SafeInvoke();
        }

        /// <summary>
        /// Stops the timer
        /// </summary>
        public virtual void StopTimer()
        {
            if (!IsTimerRunning)
                return;

            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
            OnTimerStopped.SafeInvoke();
        }

        /// <summary>
        /// Logic for the timer
        /// </summary>
        /// <param name="Duration">How long the timer should run for</param>
        /// <returns>Enumerator</returns>
        protected virtual IEnumerator TimerFunctionality(float Duration)
        {
            if (UseWaitSecondsRealTime)
                yield return new WaitForSecondsRealtime(Duration);
            else
                yield return new WaitForSeconds(Duration);

            OnTimerElapsed.SafeInvoke();
            timerCoroutine = null;
        }
    }
}
