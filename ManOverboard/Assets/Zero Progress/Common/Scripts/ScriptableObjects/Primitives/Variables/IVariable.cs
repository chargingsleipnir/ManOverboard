using System;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Interface for variable-like data structures to implement
    /// </summary>
    public interface IVariable
    {
        /// <summary>
        /// Get the value of the variable
        /// </summary>
        /// <returns>The current value</returns>
        Object GetValue();

        /// <summary>
        /// Sets the value of the variable
        /// </summary>
        /// <param name="newValue">The new value</param>
        void SetValue(Object newValue);
    }

    public interface IVariable<T> : IVariable
    {
        /// <summary>
        /// Get the value of the variable
        /// </summary>
        /// <returns>The current value</returns>
        T GetTypedValue();

        /// <summary>
        /// Sets the value of the variable
        /// </summary>
        /// <param name="newValue">The new value</param>
        void SetValue(T newValue);
    }
}