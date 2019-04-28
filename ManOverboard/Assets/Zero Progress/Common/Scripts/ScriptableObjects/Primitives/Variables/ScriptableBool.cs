using UnityEngine;

namespace ZeroProgress.Common
{
    [CreateAssetMenu(fileName = "New Bool", menuName = ScriptableObjectPaths.ZERO_PROGRESS_PRIMITIVES_PATH + "Scriptable Bool", order = (int)ScriptableVariablesMenuIndexing.BoolParam)]
    public class ScriptableBool : ScriptablePrimitive<bool>
    {
    }
}