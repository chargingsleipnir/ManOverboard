using UnityEngine;
using ZeroProgress.Common;
using ZeroProgress.Common.Collections;

/// <summary>
/// A set to register SpriteTossable instances
/// </summary>
[CreateAssetMenu(fileName = "SpriteTossableCollection", menuName = ScriptableObjectPaths.ZERO_PROGRESS_COLLECTIONS_PATH + "SpriteTossable Set")]
public class SpriteTossableSet : ScriptableSet<SpriteTossable> { }
