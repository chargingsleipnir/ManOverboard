using UnityEngine;

namespace ZeroProgress.Interactions
{
    public interface IInteractable
    {
        bool CanInteract(GameObject Interactor);
        void Interact();
    }
}
