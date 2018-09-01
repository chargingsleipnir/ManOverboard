using System;

namespace ZeroProgress.Common
{
    /// <summary>
    /// A generic implementation of event args for single value
    /// </summary>
    /// <typeparam name="T">The type of parameter to include</typeparam>
    public class EventArgs<T> : EventArgs
    {
        /// <summary>
        /// The event value
        /// </summary>
        public T Value;
    }
}