using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Provides a Scriptable wrapper around the filter to allow for asset versions
    /// of filters
    /// </summary>
    /// <typeparam name="T">The IFilter type to wrap</typeparam>
    /// <typeparam name="FilterT">The type that the filter acts upon</typeparam>
    public abstract class ScriptableFilter<T, FilterT> : ScriptablePrimitive<T> where T : IFilter<FilterT>
    {
        /// <summary>
        /// Implicit cast operator for passing this asset to methods that are looking for IFilter for example
        /// </summary>
        /// <param name="Scriptable"></param>
        public static implicit operator T(ScriptableFilter<T, FilterT> Scriptable)
        {
            return Scriptable.CurrentValue;
        }
        
        /// <summary>
        /// Resets the filter value to the specified default value
        /// </summary>
        public override void ResetValue()
        {
            CurrentValue = DefaultValue.DeepCopy();
        }
    }
}