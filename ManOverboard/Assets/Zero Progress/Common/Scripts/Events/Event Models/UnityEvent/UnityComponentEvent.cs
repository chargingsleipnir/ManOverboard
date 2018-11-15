using System;
using UnityEngine;
using UnityEngine.Events;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Instance of a UnityEvent that takes a single int parameter
    /// </summary>
    [Serializable]
    public class UnityComponentEvent : UnityEvent<Component>
    {
    }
}
