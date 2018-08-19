using UnityEngine;

namespace ZeroProgress.Common.Timing
{
    /// <summary>
    /// Stopwatch that functions on the update cycle
    /// </summary>
    public class StopwatchOnUpdate : Stopwatch
    {
        /// <summary>
        /// True if the stopwatch is running, false if not
        /// </summary>
        protected bool isRunning = false;

        /// <summary>
        /// The current counter for tracking if the interval is exceeded (for firing the interval event)
        /// </summary>
        protected float intervalCounter = 0f;

        /// <summary>
        /// True if currently running, false if not
        /// </summary>
        public override bool IsStopwatchRunning
        {
            get { return isRunning; }
        }
        
        /// <summary>
        /// The number of seconds that have passed so far
        /// </summary>
        public float SecondsPassed
        {
            get
            {
                return secondsPassed;
            }
        }

        /// <summary>
        /// Starts the stopwatch
        /// </summary>
        public override void StartStopwatch()
        {
            if (IsStopwatchRunning)
                return;

            isRunning = true;
            OnStopwatchStarted.SafeInvoke();
        }
        
        /// <summary>
        /// Stops the stopwatch
        /// </summary>
        public override void StopStopwatch()
        {
            if (!IsStopwatchRunning)
                return;

            isRunning = false;
            OnStopwatchStopped.SafeInvoke();
        }

        private void Update()
        {
            if (!isRunning)
                return;

            float timePassed = Time.deltaTime;

            if (UseWaitSecondsRealTime)
                timePassed = Time.unscaledDeltaTime;

            secondsPassed += timePassed;
            intervalCounter += timePassed;

            if(intervalCounter >= NotificationInterval)
            {
                OnIntervalElapsed.SafeInvoke(secondsPassed);
                intervalCounter = 0f;
            }
        }
    }
}