using UnityEngine;

namespace ZeroProgress.Common
{
    [CreateAssetMenu(fileName = "New Int Game Event", menuName = ScriptableObjectPaths.ZERO_PROGRESS_EVENTS_PATH + "Param Events/" + "Int Param Game Event", order = (int)ScriptableEventsMenuIndexing.IntEvent)]
    public class IntParamEvent : ParamGameEvent<int>
    {

    }
}