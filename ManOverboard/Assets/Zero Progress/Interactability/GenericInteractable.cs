using UnityEngine;
using UnityEngine.Events;
using ZeroProgress.Common;

namespace ZeroProgress.Interactions
{
    /// <summary>
    /// An interaction response easily configured through the editor
    /// </summary>
    public class GenericInteractable : IInteractable
    {
        [SerializeField]
        [Tooltip("The label to be displayed")]
        protected string interactionLabel = "";

        [Tooltip("The actions to be taken when the interaction is activated")]
        public UnityEvent InteractionResponse;

        public virtual bool CanInteract(GameObject Interactor)
        {
            return true;
        }

        public virtual string GetInteractionLabel()
        {
            return interactionLabel;
        }

        public virtual void Interact(GameObject Interactor)
        {
            if (CanInteract(Interactor))
                Interact();
        }

        public virtual void Interact()
        {
            InteractionResponse.SafeInvoke();
        }
    }
}