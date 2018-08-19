using UnityEngine.Events;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Extensions for Unity Events
    /// </summary>
    public static class UnityEventExtensions
    {
        /// <summary>
        /// Safely invoke a unity event (performing null check before invocation)
        /// </summary>
        /// <param name="EventToRaise">The event to be raised</param>
        public static void SafeInvoke(this UnityEvent EventToRaise)
        {
            // This works because it's an extension method
            if (EventToRaise != null)
                EventToRaise.Invoke();
        }

        /// <summary>
        /// Safely invoke a unity event (performing null check before invocation)
        /// </summary>
        /// <typeparam name="T">The parameter type to pass along</typeparam>
        /// <param name="EventToRaise">The event to be raised</param>
        /// <param name="ParameterValue">The value to send with the invocation</param>
        public static void SafeInvoke<T>(this UnityEvent<T> EventToRaise, T ParameterValue)
        {
            if (EventToRaise != null)
                EventToRaise.Invoke(ParameterValue);
        }
    }
}
