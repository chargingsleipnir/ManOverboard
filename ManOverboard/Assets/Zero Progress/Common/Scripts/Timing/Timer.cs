using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace ZeroProgress.Common.Timing
{
    public class Timer : MonoBehaviour
    {
        public float Duration = 10f;

        public bool StartTimerOnStart = false;

        public bool StartTimerOnEnable = false;

        public bool UseWaitSecondsRealTime = false;

        public UnityEvent OnTimerElapsed;

        public UnityEvent OnTimerStarted;

        public UnityEvent OnTimerStopped;

        protected Coroutine timerCoroutine = null;
        
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

        public virtual void StopTimer()
        {
            if (!IsTimerRunning)
                return;

            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
            OnTimerStopped.SafeInvoke();
        }

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
