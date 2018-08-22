using UnityEngine;
using UnityEngine.Events;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Translator for a GameEvent to a UnityEvent for easy configuration of event response
    /// through the inspector
    /// </summary>
    public class GameEventListener : MonoBehaviour, IGameEventListener {

        [Tooltip("The game event to register to")]
        public GameEvent Event;

        [Tooltip("The unity event that acts as the response to the game event")]
        public UnityEvent EventResponse;

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
        public virtual void OnEventRaised()
        {
            EventResponse.Invoke();
        }

    }
}