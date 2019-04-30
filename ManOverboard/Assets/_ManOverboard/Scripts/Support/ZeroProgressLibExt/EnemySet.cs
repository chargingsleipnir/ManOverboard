using UnityEngine;
using ZeroProgress.Common;
using ZeroProgress.Common.Collections;

/// <summary>
/// A set to register Enemy instances
/// </summary>
[CreateAssetMenu(fileName = "EnemyCollection", menuName = ScriptableObjectPaths.ZERO_PROGRESS_COLLECTIONS_PATH + "Enemy Set")]
public class EnemySet : ScriptableSet<Enemy> { }