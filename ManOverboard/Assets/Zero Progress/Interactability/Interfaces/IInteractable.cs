using UnityEngine;

namespace ZeroProgress.Interactions
{
    /// <summary>
    /// Interface for all interactable objects to implement
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// An identifier for this interactable
        /// </summary>
        /// <returns>The Id used to identify the type of this interactable</returns>
        string GetInteractableId();

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
        /// Try to perform the interaction, checking if Interaction is possible first
        /// </summary>
        void TryInteract(GameObject Interactor);

        /// <summary>
        /// Perform the interaction without checking if the interaction is possible first
        /// </summary>
        void Interact();
    }
}
