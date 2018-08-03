using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ZeroProgress.Common
{
    [Serializable]
    public class EventMapping
    {
        public StringReference EventLabel;
        public UnityEvent EventsToFire;
    }

    public class GenericAnimEventReceiver : MonoBehaviour, IAnimationEventListener
    {

        [SerializeField]
        private List<EventMapping> eventMappings = new List<EventMapping>();

        private Dictionary<string, UnityEvent> eventDictionary;

        // Use this for initialization
        void Start()
        {
            eventDictionary = eventMappings.ToDictionary(map => map.EventLabel.Value, map => map.EventsToFire);
            eventMappings = null;
        }

        public void ExecuteEvent(string EventLabel)
        {
            UnityEvent eventToFire;

            if (eventDictionary.TryGetValue(EventLabel, out eventToFire))
                eventToFire.Invoke();
        }
    }
}