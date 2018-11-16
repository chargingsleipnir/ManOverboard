using UnityEngine;
using UnityEngine.Events;
using ZeroProgress.Common;

namespace ZeroProgress.Interactions
{
    /// <summary>
    /// A toggleable response easily configured through the editor
    /// </summary>
    public class GenericToggleable : ToggleableBase
    {
        [Tooltip("The actions to be taken when this toggleable is activated")]
        public UnityEvent ActivationResponse;

        [Tooltip("The actions to be taken when this toggleable is deactivated")]
        public UnityEvent DeactivationResponse;

        protected override void OnActivate()
        {
            ActivationResponse.SafeInvoke();
        }

        protected override void OnDeactivate()
        {
            DeactivationResponse.SafeInvoke();
        }
    }
}