using UnityEngine;

namespace ZeroProgress.Common
{
    [CreateAssetMenu(fileName = "NewGameObjectTagFilter", menuName = ScriptableObjectPaths.ZERO_PROGRESS_FILTERS_PATH + "Game Object Tag Filter")]
    public class ScriptableGameObjectTagFilter : ScriptableFilter<GameObjectTagFilter, GameObject>
    {
    }
}