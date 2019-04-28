using UnityEngine;

namespace ZeroProgress.Common
{
    [CreateAssetMenu(fileName = "New Component Game Event", menuName = ScriptableObjectPaths.ZERO_PROGRESS_EVENTS_PATH + "Param Events/" + "Component Param Game Event", order = (int)ScriptableEventsMenuIndexing.ComponentEvent)]
    public class ComponentParamEvent : ParamGameEvent<Component>
    {

    }
}