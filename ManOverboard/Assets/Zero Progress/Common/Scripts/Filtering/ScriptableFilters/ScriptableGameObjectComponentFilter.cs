using UnityEngine;

namespace ZeroProgress.Common
{
    [CreateAssetMenu(fileName = "NewComponentFilter", menuName = ScriptableObjectPaths.ZERO_PROGRESS_FILTERS_PATH + "Game Object Component Filter")]
    public class ScriptableGameObjectComponentFilter : ScriptableFilter<GameObjectComponentFilter, GameObject>
    {
    }
}