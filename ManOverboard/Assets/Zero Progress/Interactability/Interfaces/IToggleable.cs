namespace ZeroProgress.Interactions
{
    /// <summary>
    /// Interface for an item that can be toggled between an ON and an OFF state
    /// </summary>
    public interface IToggleable : IInteractable
    {
        /// <summary>
        /// Sets the toggle object to ACTIVATED
        /// </summary>
        void Activate();

        /// <summary>
        /// Sets the toggle object to DEACTIVATED
        /// </summary>
        void Deactivate();

        /// <summary>
        /// Toggles the toggle to its opposite state
        /// </summary>
        void Toggle();

        /// <summary>
        /// Check to determine what state the toggle object is currently in
        /// </summary>
        /// <returns>True if it's activate, false if it's deactivated</returns>
        bool IsActivated();
    }
}
