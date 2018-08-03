using System.Collections;
using UnityEngine;

namespace ZeroProgress.Common.Timing
{
    public class CountdownTimer : Timer
    {
        public float TimerInterval = 1f;

        public UnityFloatEvent OnIntervalElapsed;

        protected float currentInterval;

        public void AddTime(float ValueToAdd)
        {
            currentInterval += ValueToAdd;

            if (IsTimerRunning)
                OnIntervalElapsed.SafeInvoke(currentInterval);
        }

        public void RemoveTime(float ValueToRemove)
        {
            currentInterval -= ValueToRemove;

            if (IsTimerRunning)
                OnIntervalElapsed.SafeInvoke(currentInterval);
        }

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