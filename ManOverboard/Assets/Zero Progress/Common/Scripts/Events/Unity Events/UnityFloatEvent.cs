using System;
using UnityEngine.Events;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Instance of a UnityEvent that takes a single float parameter
    /// </summary>
    [Serializable]
    public class UnityFloatEvent : UnityEvent<float>
    {
    }
}