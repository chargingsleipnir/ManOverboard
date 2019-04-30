using UnityEngine;

namespace ZeroProgress.Common.Collections
{
    /// <summary>
    /// A set for string items
    /// </summary>
    [CreateAssetMenu(fileName = "StringCollection", menuName = ScriptableObjectPaths.ZERO_PROGRESS_COLLECTIONS_PATH + "String Set", order = (int)ScriptableCollectionsMenuIndexing.StringSet)]
    public class StringSet : ScriptableSet<string>
    {
    }
}
