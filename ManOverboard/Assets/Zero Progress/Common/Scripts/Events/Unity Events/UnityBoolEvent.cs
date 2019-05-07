using System;
using UnityEngine.Events;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Instance of a UnityEvent that takes a single boolean parameter
    /// </summary>
    [Serializable]
    public class UnityBoolEvent : UnityEvent<bool>
    {
    }
}
