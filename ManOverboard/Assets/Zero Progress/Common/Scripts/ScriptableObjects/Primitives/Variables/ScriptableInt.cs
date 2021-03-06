﻿using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Scriptable primitive of int type
    /// </summary>
    [CreateAssetMenu(fileName = "New Int", menuName = ScriptableObjectPaths.ZERO_PROGRESS_PRIMITIVES_PATH + "Scriptable Int", order = (int)ScriptableVariablesMenuIndexing.IntParam)]
    public class ScriptableInt : ScriptablePrimitive<int>
    {

    }
}
