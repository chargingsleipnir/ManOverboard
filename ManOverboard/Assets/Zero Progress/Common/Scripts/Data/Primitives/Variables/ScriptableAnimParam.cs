using UnityEngine;

namespace ZeroProgress.Common
{
    [CreateAssetMenu(fileName = "New Anim Param", menuName = ScriptableObjectPaths.ZERO_PROGRESS_PRIMITIVES_PATH + "Scriptable Anim Param")]
    public class ScriptableAnimParam : ScriptableInt
    {
#if UNITY_EDITOR
        public string AnimParamName;
#endif
    }
}