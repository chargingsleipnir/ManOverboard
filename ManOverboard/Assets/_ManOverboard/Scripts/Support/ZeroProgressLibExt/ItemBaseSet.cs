using UnityEngine;
using ZeroProgress.Common;
using ZeroProgress.Common.Collections;

/// <summary>
/// A set to register Item instances
/// </summary>
[CreateAssetMenu(fileName = "ItemBaseCollection", menuName = ScriptableObjectPaths.ZERO_PROGRESS_COLLECTIONS_PATH + "ItemBase Set")]
public class ItemBaseSet : ScriptableSet<ItemBase> { }