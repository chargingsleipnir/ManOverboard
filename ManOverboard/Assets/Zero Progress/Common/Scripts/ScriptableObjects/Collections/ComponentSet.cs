using UnityEngine;

namespace ZeroProgress.Common.Collections
{
    /// <summary>
    /// A set to register base component instances
    /// </summary>
    [CreateAssetMenu(fileName = "ComponentCollection", menuName = ScriptableObjectPaths.ZERO_PROGRESS_COLLECTIONS_PATH + "Component Set", order = (int)ScriptableCollectionsMenuIndexing.ComponentSet)]
    public class ComponentSet : ScriptableSet<Component>
    {
    }
}