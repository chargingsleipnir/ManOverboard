using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ZeroProgress.Common
{
    public class InputAxisResponse : MonoBehaviour
    {
        public string AxisName;

        public bool UseSmoothingFiltering = true;

        public UnityEvent OnAxisNegative, OnAxisPositive, OnAxisZeroed;
        
        // Update is called once per frame
        void Update()
        {
            if (Input.GetAxis(AxisName) == 0f)
                OnAxisZeroed.SafeInvoke();
            else if (Input.GetAxis(AxisName) > 0f)
                OnAxisPositive.SafeInvoke();
            else if (Input.GetAxis(AxisName) < 0f)
                OnAxisNegative.SafeInvoke();
        }
    }
}