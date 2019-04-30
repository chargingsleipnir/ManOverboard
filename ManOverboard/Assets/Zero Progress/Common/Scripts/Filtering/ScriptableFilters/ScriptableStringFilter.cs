using UnityEngine;

namespace ZeroProgress.Common
{
    [CreateAssetMenu(fileName = "NewStringFilter", menuName = ScriptableObjectPaths.ZERO_PROGRESS_FILTERS_PATH + "String Filter", order = (int)ScriptableFiltersMenuIndexing.StringFilter)]
    public class ScriptableStringFilter : ScriptableFilter<StringFilter, string>
    {        
    }
}