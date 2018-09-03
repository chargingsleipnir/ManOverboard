using System.Collections.Generic;
using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// An event designed as an asset to act as a kind of 'singleton' event
    /// </summary>
    [CreateAssetMenu(fileName = "New Game Event", menuName = ScriptableObjectPaths.ZERO_PROGRESS_EVENTS_PATH + "Game Event")]
    public class GameEvent : ScriptableObject
    {
        /// <summary>
        /// All listeners registered to this event
        /// </summary>
        protected readonly List<IGameEventListener> listeners = new List<IGameEventListener>();

        /// <summary>
        /// Invoke/Raise the event
        /// </summary>
        public virtual void RaiseEvent()
        {
            foreach (IGameEventListener listener in listeners)
            {
                listener.OnEventRaised();
            }
        }

        /// <summary>
        /// Register a new listener to this event
        /// </summary>
        /// <param name="Listener">The listener to register</param>
        public virtual void RegisterListener(IGameEventListener Listener)
        {
            if (!listeners.Contains(Listener))
                listeners.Add(Listener);
        }

        /// <summary>
        /// Unregisters a listener from this event
        /// </summary>
        /// <param name="Listener">The listener to unregister</param>
        public virtual void UnregisterListener(IGameEventListener Listener)
        {
            listeners.Remove(Listener);
        }
    }
}