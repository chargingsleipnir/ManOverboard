using UnityEngine;

namespace ZeroProgress.Common
{
    [CreateAssetMenu(fileName = "New Vector3 Game Event", menuName = ScriptableObjectPaths.ZERO_PROGRESS_EVENTS_PATH + "Param Events/" + "Vector3 Param Game Event", order = (int)ScriptableEventsMenuIndexing.Vector3Event)]
    public class Vector3ParamEvent : ParamGameEvent<Vector3>
    {

    }
}