namespace ZeroProgress.Common
{
    /// <summary>
    /// Interface for items that can be selected
    /// </summary>
    public interface ISelectable
    {
        /// <summary>
        /// Determines if the item is currently selectable
        /// </summary>
        /// <returns>True if the item can be selected at this point in time, false if not</returns>
        bool CanSelect();

        /// <summary>
        /// Returns the current selected state
        /// </summary>
        /// <returns>True if selected, false if not</returns>
        bool IsSelected();

        /// <summary>
        /// Selects the item
        /// </summary>
        void Select();

        /// <summary>
        /// Unselects the item
        /// </summary>
        void UnSelect();

        /// <summary>
        /// Toggle the current selected state
        /// </summary>
        void ToggleSelected();
    }
}