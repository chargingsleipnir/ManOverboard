using System;
using UnityEngine;
using UnityEngine.Events;

namespace ZeroProgress.Common {
    /// <summary>
    /// Instance of a UnityEvent that takes a single Vector2 parameter
    /// </summary>
    [Serializable]
    public class UnityVector2Event : UnityEvent<Vector2> {
    }
}
