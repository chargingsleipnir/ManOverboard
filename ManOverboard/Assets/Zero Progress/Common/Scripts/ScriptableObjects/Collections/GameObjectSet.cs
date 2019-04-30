using UnityEngine;

namespace ZeroProgress.Common.Collections
{
    /// <summary>
    /// A set to register GameObject instances
    /// </summary>
    [CreateAssetMenu(fileName = "GameObjectCollection", menuName = ScriptableObjectPaths.ZERO_PROGRESS_COLLECTIONS_PATH + "Game Object Set", order = (int)ScriptableCollectionsMenuIndexing.GameObjectSet)]
    public class GameObjectSet : ScriptableSet<GameObject>
    {        
    }
}
