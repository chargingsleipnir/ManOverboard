using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Instance of a scriptable int that includes an Editor-specific parameter that identifies an animation parameter by name
    /// </summary>
    [CreateAssetMenu(fileName = "New Anim Param", menuName = ScriptableObjectPaths.ZERO_PROGRESS_PRIMITIVES_PATH + "Scriptable Anim Param")]
    public class ScriptableAnimParam : ScriptableInt
    {
#if UNITY_EDITOR
        [Tooltip("The name of the animation parameter to hash")]
        public string AnimParamName;
#endif
    }
}