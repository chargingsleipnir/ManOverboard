using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ZeroProgress.Common
{
    public class ScriptableReferenceTextDisplay<T, ValueType, PrimitiveType> : MonoBehaviour 
        where T : ScriptableReference<ValueType, PrimitiveType> 
        where PrimitiveType: ScriptablePrimitive<ValueType>
    {
        [ReferenceDropdown]
        public T Reference;

        [DisplayTimeSpan(DisplayMode = TimeSpanDisplayMode.SingleModeSelectable, UnderlyingValueType = TimeIntervalType.Seconds)]
        public float RefreshTime = 1f;

        public Text Textbox;

        public string DisplayText;

        private Coroutine refreshCoroutine = null;

        // Use this for initialization
        void Start()
        {
            Textbox = this.GetComponentIfNull(Textbox);
        }

        protected virtual void OnEnable()
        {
            if (refreshCoroutine != null)
                return;

            refreshCoroutine = StartCoroutine(RefreshDisplay());
        }

        protected virtual void OnDisable()
        {
            if (refreshCoroutine == null)
                return;

            StopCoroutine(refreshCoroutine);
        }

        public void UpdateDisplay()
        {
            string textToDisplay = DisplayText;

            if (string.IsNullOrEmpty(textToDisplay))
                textToDisplay = Reference.Value.ToString();
            else
                textToDisplay = string.Format(DisplayText, Reference.Value.ToString());

            Textbox.text = textToDisplay;
        }

        protected virtual IEnumerator RefreshDisplay()
        {
            while (true)
            {
                UpdateDisplay();
                yield return new WaitForSeconds(RefreshTime);
            }
        }
    }
}