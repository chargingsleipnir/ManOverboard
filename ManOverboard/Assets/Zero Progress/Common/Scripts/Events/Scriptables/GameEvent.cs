using System.Collections.Generic;
using UnityEngine;

namespace ZeroProgress.Common
{
    [CreateAssetMenu(fileName = "New Game Event", menuName = ScriptableObjectPaths.ZERO_PROGRESS_EVENTS_PATH + "Game Event")]
    public class GameEvent : ScriptableObject
    {
        private readonly List<IGameEventListener> listeners = new List<IGameEventListener>();

        public void RaiseEvent()
        {
            foreach (IGameEventListener listener in listeners)
            {
                listener.OnEventRaised();
            }
        }

        public void RegisterListener(IGameEventListener Listener)
        {
            if (!listeners.Contains(Listener))
                listeners.Add(Listener);
        }

        public void UnregisterListener(IGameEventListener Listener)
        {
            listeners.Remove(Listener);
        }
    }
}