using UnityEngine;

namespace ZeroProgress.Common
{
    [CreateAssetMenu(fileName = "New Float Game Event", menuName = ScriptableObjectPaths.ZERO_PROGRESS_EVENTS_PATH + "Param Events/" + "Float Param Game Event", order = (int)ScriptableEventsMenuIndexing.FloatEvent)]
    public class FloatParamEvent : ParamGameEvent<float>
    {

    }
}