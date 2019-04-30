using UnityEngine;

namespace ZeroProgress.Common
{
    [CreateAssetMenu(fileName = "New Vector2 Game Event", menuName = ScriptableObjectPaths.ZERO_PROGRESS_EVENTS_PATH + "Param Events/" + "Vector2 Param Game Event", order = (int)ScriptableEventsMenuIndexing.Vector2Event)]
    public class Vector2ParamEvent : ParamGameEvent<Vector2>
    {

    }
}