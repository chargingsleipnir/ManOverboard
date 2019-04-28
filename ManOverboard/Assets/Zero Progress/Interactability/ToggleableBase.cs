using UnityEngine;
using ZeroProgress.Common;

namespace ZeroProgress.Interactions
{
    /// <summary>
    /// A base monobehaviour class to help with the toggling functionality of the IToggleable class
    /// </summary>
    public abstract class ToggleableBase : MonoBehaviour, IToggleable
    {
        [Tooltip("True to ignore all toggle method calls if this object is inactive")]
        public bool IgnoreIfInactive = true;

        [SerializeField]
        protected StringReference interactionId;

        [SerializeField]
        [Tooltip("The label associated with the interaction")]
        protected string InteractionLabel = "";

        [SerializeField]
        [Tooltip("True to apply the current toggle value whenever Enabled")]
        private bool applyOnEnable = true;

        /// <summary>
        /// The current toggle value
        /// </summary>
        [Tooltip("The current toggle value")]
        protected bool toggleState = true;

        protected abstract void OnActivate();

        protected abstract void OnDeactivate();

        protected virtual void OnEnable()
        {
            if(applyOnEnable)
            {
                if (toggleState)
                    OnActivate();
                else
                    OnDeactivate();
            }
        }

        /// <summary>
        /// Toggles the state to its inverted value
        /// </summary>
        public virtual void Toggle()
        {
            if (IgnoreIfInactive && !enabled)
                return;

            toggleState = !toggleState;

            if (toggleState)
                Activate();
            else
                Deactivate();
        }
        
        /// <summary>
        /// Inherited by children to provide the actual functionality caused by the current toggle state
        /// </summary>
        /// <param name="NewValue">The new toggle state being used</param>
        protected virtual void ApplyToggleChange(bool NewValue)
        { }
        
        public virtual void Activate()
        {
            if (toggleState)
                return;

            if (IgnoreIfInactive && !enabled)
                return;

            toggleState = true;
            OnActivate();
        }

        public virtual void Deactivate()
        {
            if (!toggleState)
                return;

            if (IgnoreIfInactive && !enabled)
                return;

            toggleState = false;
            OnDeactivate();
        }

        public virtual bool IsActivated()
        {
            return toggleState;
        }

        public virtual string GetInteractionLabel()
        {
            return InteractionLabel;
        }

        public virtual bool CanInteract(GameObject Interactor)
        {
            return true;
        }

        public virtual void TryInteract(GameObject Interactor)
        {
            if (CanInteract(Interactor))
                Toggle();
        }

        public virtual void Interact()
        {
            Toggle();
        }

        public string GetInteractableId()
        {
            return interactionId;
        }
    }
}
