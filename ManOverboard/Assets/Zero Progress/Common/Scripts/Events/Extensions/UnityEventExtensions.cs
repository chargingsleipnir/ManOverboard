using UnityEngine.Events;

namespace ZeroProgress.Common
{
    public static class UnityEventExtensions
    {
        public static void SafeInvoke(this UnityEvent EventToRaise)
        {
            if (EventToRaise != null)
                EventToRaise.Invoke();
        }

        public static void SafeInvoke<T>(this UnityEvent<T> EventToRaise, T ParameterValue)
        {
            if (EventToRaise != null)
                EventToRaise.Invoke(ParameterValue);
        }
    }
}
