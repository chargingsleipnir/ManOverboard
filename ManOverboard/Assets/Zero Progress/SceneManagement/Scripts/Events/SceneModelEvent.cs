using System;
using UnityEngine.Events;

namespace ZeroProgress.SceneManagementUtility
{
    /// <summary>
    /// Instance of a UnityEvent that takes a single SceneModel parameter
    /// </summary>
    [Serializable]
    public class SceneModelEvent : UnityEvent<SceneModel>
    {
    }
}
