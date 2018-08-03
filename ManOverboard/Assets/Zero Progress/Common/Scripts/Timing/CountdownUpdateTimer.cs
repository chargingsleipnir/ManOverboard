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
        private bool isRunning = false;

        private float intervalCounter = 0f;

        public override bool IsTimerRunning
        {
            get
            {
                return isRunning;
            }
        }

        public float CurrentValue
        {
            get
            {
                return currentInterval;
            }
        }

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