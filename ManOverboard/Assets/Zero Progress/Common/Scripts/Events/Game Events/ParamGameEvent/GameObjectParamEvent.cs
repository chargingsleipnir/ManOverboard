using UnityEngine;

namespace ZeroProgress.Common
{
    [CreateAssetMenu(fileName = "New Game Object Game Event", menuName = ScriptableObjectPaths.ZERO_PROGRESS_EVENTS_PATH + "Param Events/" + "Game Object Param Game Event", order = (int)ScriptableEventsMenuIndexing.GameObjectEvent)]
    public class GameObjectParamEvent : ParamGameEvent<GameObject>
    {

    }
}