using System;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Represents the change event that occurs when the value is updated
    /// </summary>
    /// <typeparam name="T">The type of value</typeparam>
    [Serializable]
    public class ValueChangedEventArgs<T>
    {
        /// <summary>
        /// The value that was previously set
        /// </summary>
        public T OldValue;

        /// <summary>
        /// The new value
        /// </summary>
        public T NewValue;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ValueChangedEventArgs()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="oldValue">The value that was previously set</param>
        /// <param name="newValue">The new value</param>
        public ValueChangedEventArgs(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    [Serializable]
    public class FloatChangedEventArgs : ValueChangedEventArgs<float>
    {
        public FloatChangedEventArgs()
        {

        }

        public FloatChangedEventArgs(float oldValue, float newValue) : base(oldValue, newValue)
        {
        }
    }

    [Serializable]
    public class StringChangedEventArgs : ValueChangedEventArgs<string>
    {
        public StringChangedEventArgs()
        {

        }

        public StringChangedEventArgs(string oldValue, string newValue) : base(oldValue, newValue)
        {
        }
    }

    [Serializable]
    public class IntChangedEventArgs : ValueChangedEventArgs<int>
    {
        public IntChangedEventArgs()
        {

        }

        public IntChangedEventArgs(int oldValue, int newValue) : base(oldValue, newValue)
        {
        }
    }
}
