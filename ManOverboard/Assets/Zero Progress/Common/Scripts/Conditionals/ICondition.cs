namespace ZeroProgress.Common
{
    /// <summary>
    /// Interface for programmatic conditionals to adhere to
    /// </summary>
    public interface ICondition
    {
        /// <summary>
        /// Determines if the provided value matches the condition
        /// </summary>
        /// <param name="Param">The parameter to evaluate</param>
        /// <returns>True if the parameter passes the condition, false if not</returns>
        bool IsMet(object Param);
    }

    /// <summary>
    /// Generic interface for programmatic conditionals to adhere to
    /// </summary>
    public interface ICondition<T> : ICondition
    {
        /// <summary>
        /// Determines if the provided value matches the condition
        /// </summary>
        /// <param name="Param">The parameter to evaluate</param>
        /// <returns>True if the parameter passes the condition, false if not</returns>
        bool IsMet(T Param);
    }
}
