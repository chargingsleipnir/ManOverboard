using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Data pairing of a label and a UnityEvent
    /// </summary>
    [Serializable]
    public class EventMapping
    {
        public StringReference EventLabel;
        public UnityEvent EventsToFire;
    }

    /// <summary>
    /// A generalized receiver of animation events to provide an inspector-focused 
    /// response to events received from state machines
    /// </summary>
    public class GenericAnimEventReceiver : MonoBehaviour, IAnimationEventListener
    {
        [SerializeField]
        [Tooltip("The mappings of labels to event responses")]
        private List<EventMapping> eventMappings = new List<EventMapping>();

        /// <summary>
        /// Simple lookup for the event mappings list (at runtime the mappings get moved to the dictionary)
        /// </summary>
        private Dictionary<string, UnityEvent> eventDictionary;

        // Use this for initialization
        void Start()
        {
            eventDictionary = eventMappings.ToDictionary(map => map.EventLabel.Value, map => map.EventsToFire);
            eventMappings = null;
        }

        /// <summary>
        /// Responds to the received animation event
        /// </summary>
        /// <param name="EventLabel">The label used to describe the received event</param>
        public void ReceiveEvent(string EventLabel)
        {
            UnityEvent eventToFire;

            if (eventDictionary.TryGetValue(EventLabel, out eventToFire))
                eventToFire.Invoke();
        }
    }
}