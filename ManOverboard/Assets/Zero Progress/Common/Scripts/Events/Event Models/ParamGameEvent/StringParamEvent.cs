using UnityEngine;

namespace ZeroProgress.Common
{
    [CreateAssetMenu(fileName = "New String Game Event", menuName = ScriptableObjectPaths.ZERO_PROGRESS_EVENTS_PATH + "Param Events/" + "String Param Game Event", order = (int)ScriptableEventsMenuIndexing.StringEvent)]
    public class StringParamEvent : ParamGameEvent<string>
    {

    }
}