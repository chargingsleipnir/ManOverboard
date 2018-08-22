using System;
using UnityEngine;

namespace ZeroProgress.Common
{
    [CreateAssetMenu(fileName = "NewTypeFilter", menuName = ScriptableObjectPaths.ZERO_PROGRESS_FILTERS_PATH + "Type Filter")]
    public class ScriptableTypeFilter : ScriptableFilter<TypeFilter, Type>
    {
    }
}