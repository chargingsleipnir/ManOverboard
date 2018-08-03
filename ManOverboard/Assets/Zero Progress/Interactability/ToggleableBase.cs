using UnityEngine;
using UnityEngine.Events;
using ZeroProgress.Common;

namespace ZeroProgress.Interactions
{
    public class ToggleableBase : MonoBehaviour, IToggleable
    {
        public UnityEvent TurnedOn;

        public UnityEvent TurnedOff;

        [Tooltip("True to ignore all toggle method calls if this object is inactive")]
        public bool IgnoreIfInactive = true;

        [SerializeField]
        private bool startingValue = true;

        [SerializeField]
        private bool applyOnEnable = true;

        [SerializeField]
        protected BoolValueSetOption onEnableValueOverride = BoolValueSetOption.UNCHANGED;

        [SerializeField]
        private bool applyOnStart = true;
        
        protected bool currentValue = true;

        private void OnEnable()
        {
            currentValue =
                BoolValueSetOptionExtensions.GetBoolValue(onEnableValueOverride, currentValue);

            if (applyOnEnable)
                HandleValueChanged(currentValue);
        }

        // Use this for initialization
        void Start()
        {
            if (applyOnStart)
            {
                currentValue = startingValue;
                ApplyToggleChange(currentValue);
                HandleValueChanged(currentValue);
            }
        }

        public virtual bool IsOn()
        {
            return currentValue;
        }

        public virtual void Toggle()
        {
            if (IgnoreIfInactive && !enabled)
                return;

            currentValue = !currentValue;
            HandleValueChanged(currentValue);
        }

        public virtual void TurnOff()
        {
            if (IgnoreIfInactive && !enabled)
                return;

            if (currentValue == false)
                return;

            currentValue = false;
            HandleValueChanged(currentValue);
        }

        public virtual void TurnOn()
        {
            if (IgnoreIfInactive && !enabled)
                return;

            if (currentValue)
                return;

            currentValue = true;
            HandleValueChanged(currentValue);
        }

        protected void HandleValueChanged(bool CurrentValue)
        {
            ApplyToggleChange(currentValue);
            RaiseEvents(currentValue);
        }

        protected virtual void ApplyToggleChange(bool NewValue)
        { }

        protected void RaiseEvents(bool CurrentValue)
        {
            UnityEvent eventToRaise = CurrentValue ? TurnedOn : TurnedOff;
            eventToRaise.SafeInvoke();
        }
    }
}
