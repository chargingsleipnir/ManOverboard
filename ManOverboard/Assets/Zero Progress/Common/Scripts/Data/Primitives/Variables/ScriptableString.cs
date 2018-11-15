using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Scriptable primitive of string type
    /// </summary>
    [CreateAssetMenu(fileName = "New String", menuName = ScriptableObjectPaths.ZERO_PROGRESS_PRIMITIVES_PATH + "Scriptable String", order = (int)ScriptableVariablesMenuIndexing.StringParam)]
    public class ScriptableString : ScriptablePrimitive<string>
    {
        public ScriptableString()
        {
            CurrentValue = DefaultValue = string.Empty;
        }
    }
}