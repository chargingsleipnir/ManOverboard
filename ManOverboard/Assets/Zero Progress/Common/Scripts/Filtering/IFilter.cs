namespace ZeroProgress.Common
{
    /// <summary>
    /// Interface for all filters to implement
    /// </summary>
    /// <typeparam name="T">The type of object that will have validity determined</typeparam>
    public interface IFilter<T>
    {
        /// <summary>
        /// Evaluates the provided item to determine if it passes the filtering conditions
        /// </summary>
        /// <param name="Item">The item to evaluate</param>
        /// <returns>True if valid, false if not</returns>
        bool IsValidItem(T Item);
    }
}
