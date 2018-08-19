using System.Collections;
using UnityEngine;

namespace ZeroProgress.Common.Timing
{
    /// <summary>
    /// A reusable countdown that fires a unity event every time the interval is elapsed
    /// </summary>
    public class CountdownTimer : Timer
    {
        [Tooltip("Interval that the event should be fired on")]
        public float TimerInterval = 1f;

        [Tooltip("Response for when the interval is reached")]
        public UnityFloatEvent OnIntervalElapsed;

        /// <summary>
        /// The current value of the countdown
        /// </summary>
        protected float currentInterval;

        /// <summary>
        /// Add time to the countdown and notify listeners that the interval has changed
        /// </summary>
        /// <param name="ValueToAdd">The amount of time to be added to the current value</param>
        public void AddTime(float ValueToAdd)
        {
            currentInterval += ValueToAdd;

            if (IsTimerRunning)
                OnIntervalElapsed.SafeInvoke(currentInterval);
        }

        /// <summary>
        /// Removes time from the countdown
        /// </summary>
        /// <param name="ValueToRemove">The amount of time to be removed from the current value</param>
        public void RemoveTime(float ValueToRemove)
        {
            currentInterval -= ValueToRemove;

            if (IsTimerRunning)
                OnIntervalElapsed.SafeInvoke(currentInterval);
        }

        /// <summary>
        /// The logic behind the countdown
        /// </summary>
        /// <param name="Duration">Starting time for the interval</param>
        /// <returns>Enumerator for yield</returns>
        protected override IEnumerator TimerFunctionality(float Duration)
        {
            float timerInterval = TimerInterval;

            currentInterval = Duration;
            OnIntervalElapsed.SafeInvoke(currentInterval);

            while (currentInterval > 0f)
            {
                float timeToWait = timerInterval;

                if (currentInterval < timerInterval)
                    timeToWait = currentInterval;
                
                if (UseWaitSecondsRealTime)
                    yield return new WaitForSecondsRealtime(timeToWait);
                else
                    yield return new WaitForSeconds(timeToWait);

                currentInterval -= timeToWait;

                OnIntervalElapsed.SafeInvoke(currentInterval);
            }

            OnTimerElapsed.SafeInvoke();
            timerCoroutine = null;
        }
    }
}