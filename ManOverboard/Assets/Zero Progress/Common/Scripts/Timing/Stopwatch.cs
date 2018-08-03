using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace ZeroProgress.Common.Timing
{
    public class Stopwatch : MonoBehaviour
    {
        public bool StartStopwatchOnStart = false;

        public bool StartStopwatchOnEnable = false;

        public float NotificationInterval = 1f;

        public bool UseWaitSecondsRealTime = false;

        public UnityEvent OnStopwatchStarted;

        public UnityEvent OnStopwatchRestarted;

        public UnityFloatEvent OnIntervalElapsed;

        public UnityEvent OnStopwatchStopped;

        protected Coroutine stopwatchCoroutine = null;

        protected float secondsPassed = 0f;

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

        public virtual void StartStopwatch()
        {
            if (IsStopwatchRunning)
                return;

            stopwatchCoroutine = StartCoroutine(StopwatchFunctionality());
            OnStopwatchStarted.SafeInvoke();
        }

        public virtual void ResetStopwatch(bool StartIfNotRunning = true)
        {
            secondsPassed = 0f;

            if (!IsStopwatchRunning && StartIfNotRunning)
                StartStopwatch();
            else
                OnStopwatchRestarted.SafeInvoke();

            OnIntervalElapsed.SafeInvoke(secondsPassed);
        }

        public virtual void StopStopwatch()
        {
            if (!IsStopwatchRunning)
                return;

            StopCoroutine(stopwatchCoroutine);
            stopwatchCoroutine = null;

            OnStopwatchStopped.SafeInvoke();
        }

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