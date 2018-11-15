using System;
using UnityEngine;

namespace ZeroProgress.Common
{
    [CreateAssetMenu(fileName = "NewTypeFilter", menuName = ScriptableObjectPaths.ZERO_PROGRESS_FILTERS_PATH + "Type Filter", order = (int)ScriptableFiltersMenuIndexing.TypeFilter)]
    public class ScriptableTypeFilter : ScriptableFilter<TypeFilter, Type>
    {
    }
}