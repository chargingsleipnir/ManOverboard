using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Scriptable primitive of float type
    /// </summary>
    [CreateAssetMenu(fileName = "New Float", menuName = ScriptableObjectPaths.ZERO_PROGRESS_PRIMITIVES_PATH + "Scriptable Float", order = (int)ScriptableVariablesMenuIndexing.FloatParam)]
    public class ScriptableFloat : ScriptablePrimitive<float>
    {

    }
}