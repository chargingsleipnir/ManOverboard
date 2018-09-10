using UnityEngine;
using UnityEngine.Events;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Listener for game events that require parameters
    /// </summary>
    /// <typeparam name="ParamType">The type of the parameter for the events</typeparam>
    /// <typeparam name="GameEventType"></typeparam>
    /// <typeparam name="UnityEventType"></typeparam>
    public abstract class ParamGameEventListener<ParamType, GameEventType, UnityEventType> : MonoBehaviour, IGameEventListener<ParamType> 
        where GameEventType: ParamGameEvent<ParamType>
        where UnityEventType: UnityEvent<ParamType>
    {
        [Tooltip("The game event to register to")]
        public GameEventType Event;

        [Tooltip("The unity event that acts as the response to the game event")]
        public UnityEventType EventResponse;

        protected virtual void OnEnable()
        {
            Event.RegisterListener(this);
        }

        protected virtual void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        /// <summary>
        /// Response to the game event
        /// </summary>
        public void OnEventRaised(string eventId)
        {
            EventResponse.SafeInvoke(default(ParamType));
        }

        /// <summary>
        /// Response to the game event
        /// </summary>
        public void OnEventRaised(string eventId, ParamType Param)
        {
            EventResponse.SafeInvoke(Param);
        }
    }
}