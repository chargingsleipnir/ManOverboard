using UnityEngine;

namespace ZeroProgress.Common.Collections
{
    [CreateAssetMenu(fileName = "ComponentCollection", menuName = ScriptableObjectPaths.ZERO_PROGRESS_COLLECTIONS_PATH + "Component Set")]
    public class ComponentSet : ScriptableSet<Component>
    {
    }
}