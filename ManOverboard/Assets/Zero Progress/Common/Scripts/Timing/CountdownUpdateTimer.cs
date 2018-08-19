using UnityEngine;

namespace ZeroProgress.Common.Timing
{
    /// <summary>
    /// This timer uses the Update function so you can see the time
    /// every step of the way... if you only need certain intervals, you should
    /// use CountdownTimer
    /// </summary>
    public class CountdownUpdateTimer : CountdownTimer
    {
        /// <summary>
        /// Current state of the timer
        /// </summary>
        private bool isRunning = false;

        /// <summary>
        /// Current value of the timer
        /// </summary>
        private float intervalCounter = 0f;

        /// <summary>
        /// Gets the current state of the timer
        /// </summary>
        public override bool IsTimerRunning
        {
            get
            {
                return isRunning;
            }
        }

        /// <summary>
        /// Gets the current timer value
        /// </summary>
        public float CurrentValue
        {
            get
            {
                return currentInterval;
            }
        }

        /// <summary>
        /// Starts the timer
        /// </summary>
        /// <param name="ResetIfRunning">True to reset the countdown if it's already running</param>
        public override void StartTimer(bool ResetIfRunning = false)
        {
            if(isRunning)
            {
                if (ResetIfRunning)
                    currentInterval = Duration;
                else
                    return;
            }

            currentInterval = Duration;
            intervalCounter = 0f;
            isRunning = true;
            OnTimerStarted.SafeInvoke();
            OnIntervalElapsed.SafeInvoke(currentInterval);
        }

        /// <summary>
        /// Stops the timer
        /// </summary>
        public override void StopTimer()
        {
            if (!isRunning)
                return;

            isRunning = false;
        }

        private void Update()
        {
            if (!isRunning)
                return;

            float timePassed = Time.deltaTime;

            if (UseWaitSecondsRealTime)
                timePassed = Time.unscaledDeltaTime;

            currentInterval -= timePassed;
            intervalCounter += timePassed;

            if (currentInterval < 0f)
                currentInterval = 0f;

            if (intervalCounter >= TimerInterval)
            {
                OnIntervalElapsed.SafeInvoke(currentInterval);
                intervalCounter = 0f;
            }

            if(currentInterval <= 0f)
            {
                OnTimerElapsed.SafeInvoke();
                isRunning = false;
            }            
        }
    }
}