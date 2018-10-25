using UnityEngine;
using ZeroProgress.Common;
using ZeroProgress.Common.Collections;

/// <summary>
/// A set to register ItemWaterScoopable instances
/// </summary>
[CreateAssetMenu(fileName = "ItemCanScoopCollection", menuName = ScriptableObjectPaths.ZERO_PROGRESS_COLLECTIONS_PATH + "ItemCanScoop Set")]
public class ItemCanScoopSet : ScriptableSet<ItemCanScoop> { }