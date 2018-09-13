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
        [SerializeField, Tooltip("Identifies this event so a single listener can properly handle multiple types of events")]
        protected string eventId = null;

        /// <summary>
        /// All listeners registered to this event
        /// </summary>
        protected readonly List<IGameEventListener> listeners = new List<IGameEventListener>();

        /// <summary>
        /// Tracks whether or not the game event is currently
        /// invoking responses
        /// </summary>
        protected bool isRaisingEvent = false;
        
        /// <summary>
        /// Tracks the modifications made to the listener collection
        /// during event invocation
        /// </summary>
        protected ListModificationTracker<IGameEventListener> listenerModTracker = 
            new ListModificationTracker<IGameEventListener>();

        /// <summary>
        /// Invoke/Raise the event
        /// </summary>
        public virtual void RaiseEvent()
        {
            isRaisingEvent = true;

            foreach (IGameEventListener listener in listeners)
            {
                if(listener == null)
                {
                    UnregisterListener(listener);
                    continue;
                }

                listener.OnEventRaised(eventId);
            }

            isRaisingEvent = false;
            
            listenerModTracker.ApplyStackToList(listeners, addUnique: true);
            listeners.RemoveNulls();
        }

        /// <summary>
        /// Register a new listener to this event
        /// </summary>
        /// <param name="Listener">The listener to register</param>
        public virtual void RegisterListener(IGameEventListener Listener)
        {
            if (isRaisingEvent)
                listenerModTracker.RecordAddModification(Listener);
            else
                listeners.AddUnique(Listener);
        }

        /// <summary>
        /// Unregisters a listener from this event
        /// </summary>
        /// <param name="Listener">The listener to unregister</param>
        public virtual void UnregisterListener(IGameEventListener Listener)
        {
            if (isRaisingEvent)
                listenerModTracker.RecordRemoveModification(Listener);
            else
                listeners.Remove(Listener);
        }
    }
}