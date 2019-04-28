using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ZeroProgress.Common
{
    public class InputPressResponse : MonoBehaviour
    {
        public string ButtonName;

        public UnityEvent OnButtonPress, OnButtonDown, OnButtonHeld;
        
        // Update is called once per frame
        void Update()
        {
            if (Input.GetButtonUp(ButtonName))
                OnButtonPress.SafeInvoke();

            if (Input.GetButtonDown(ButtonName))
                OnButtonDown.SafeInvoke();

            if (Input.GetButton(ButtonName))
                OnButtonHeld.SafeInvoke();
        }
    }
}