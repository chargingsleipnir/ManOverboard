using UnityEngine;

namespace ZeroProgress.Interactions
{
    /// <summary>
    /// Interface for all interactable objects to implement
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// Retrieves the label for this interaction item
        /// </summary>
        /// <returns></returns>
        string GetInteractionLabel();

        /// <summary>
        /// Determines if interaction is possible
        /// </summary>
        /// <param name="Interactor">The object that is activating this interactable</param>
        /// <returns>True if interaction can occur, false if not</returns>
        bool CanInteract(GameObject Interactor);

        /// <summary>
        /// Perform the interaction, checking if Interaction is possible first
        /// </summary>
        void Interact(GameObject Interactor);

        /// <summary>
        /// Perform the interaction without checking if the interaction is possible first
        /// </summary>
        void Interact();
    }
}
