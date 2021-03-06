﻿using System;
using System.Collections.Generic;

namespace ZeroProgress.Common
{
    /// <summary>
    /// An event designed as an asset to act as a kind of 'singleton' event that
    /// takes an event
    /// </summary>
    public abstract class ParamGameEvent<T> : GameEvent
    {
        /// <summary>
        /// Second list for listeners that take a parameter so we can properly pass the parameters
        /// </summary>
        private readonly List<IGameEventListener<T>> paramListeners = new List<IGameEventListener<T>>();
        
        /// <summary>
        /// Register a new listener to this event
        /// </summary>
        /// <param name="Listener">The listener to register</param>
        public override void RegisterListener(IGameEventListener listener)
        {
            if (isRaisingEvent)
                listenerModTracker.RecordAddModification(listener);
            else
            {
                Type listenerType = listener.GetType();

                if (listenerType.IsAssignableFromGenericInterface(typeof(IGameEventListener<>), typeof(T)))
                    paramListeners.AddUnique(listener as IGameEventListener<T>);
                else
                    listeners.AddUnique(listener);
            }
        }

        /// <summary>
        /// Unregisters a listener from this event
        /// </summary>
        /// <param name="Listener">The listener to unregister</param>
        public override void UnregisterListener(IGameEventListener listener)
        {
            if (isRaisingEvent)
                listenerModTracker.RecordRemoveModification(listener);
            else
            {
                Type listenerType = listener.GetType();

                if (listenerType.IsAssignableFromGenericInterface(typeof(IGameEventListener<T>)))
                    listeners.Remove(listener);
                else
                    paramListeners.Remove(listener as IGameEventListener<T>);
            }
        }

        /// <summary>
        /// Invoke/Raise the event without passing a parameter
        /// </summary>
        public override void RaiseEvent()
        {
            RaiseEvent(default(T));
        }

        /// <summary>
        /// Invoke/Raise the event with a parameter
        /// </summary>
        public void RaiseEvent(T Param)
        {
            isRaisingEvent = true;

            foreach (IGameEventListener listener in listeners)
            {
                listener.OnEventRaised(eventId);
            }

            foreach (IGameEventListener<T> paramListener in paramListeners)
            {
                paramListener.OnEventRaised(eventId, Param);
            }

            isRaisingEvent = false;
            
            listenerModTracker.ApplyStack(RegisterListener, UnregisterListener);

            listeners.RemoveNulls();
            paramListeners.RemoveNulls();
        }
    }
}