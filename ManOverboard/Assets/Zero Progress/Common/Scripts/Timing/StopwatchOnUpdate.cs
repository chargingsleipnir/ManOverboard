using UnityEngine;

namespace ZeroProgress.Common.Timing
{
    public class StopwatchOnUpdate : Stopwatch
    {
        protected bool isRunning = false;

        protected float intervalCounter = 0f;

        public override bool IsStopwatchRunning
        {
            get { return isRunning; }
        }
        
        public float SecondsPassed
        {
            get
            {
                return secondsPassed;
            }
        }

        public override void StartStopwatch()
        {
            if (IsStopwatchRunning)
                return;

            isRunning = true;
            OnStopwatchStarted.SafeInvoke();
        }
        
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