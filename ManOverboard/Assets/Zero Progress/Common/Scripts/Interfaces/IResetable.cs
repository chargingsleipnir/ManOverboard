namespace ZeroProgress.Common
{
    /// <summary>
    /// Interface for items that can be reset
    /// </summary>
    public interface IResetable
    {
        /// <summary>
        /// Determines if the item can be reset
        /// </summary>
        /// <returns></returns>
        bool CanReset();

        /// <summary>
        /// Resets the item
        /// </summary>
        void Reset();
    }
}