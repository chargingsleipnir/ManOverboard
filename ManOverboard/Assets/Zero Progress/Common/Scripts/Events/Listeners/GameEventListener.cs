using UnityEngine;
using UnityEngine.Events;

namespace ZeroProgress.Common
{
    public class GameEventListener : MonoBehaviour, IGameEventListener {

        public GameEvent Event;

        public UnityEvent EventResponse;

        protected virtual void OnEnable()
        {
            Event.RegisterListener(this);
        }

        protected virtual void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public virtual void OnEventRaised()
        {
            EventResponse.Invoke();
        }

    }
}