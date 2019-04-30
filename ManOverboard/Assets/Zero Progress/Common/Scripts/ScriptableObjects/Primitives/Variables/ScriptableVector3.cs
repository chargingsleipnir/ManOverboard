using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Scriptable primitive of Vector3 type
    /// </summary>
    [CreateAssetMenu(fileName = "New Vector3", menuName = ScriptableObjectPaths.ZERO_PROGRESS_PRIMITIVES_PATH + "Scriptable Vector3", order = (int)ScriptableVariablesMenuIndexing.Vector3Param)]
    public class ScriptableVector3 : ScriptablePrimitive<Vector3>
    {

    }
}
