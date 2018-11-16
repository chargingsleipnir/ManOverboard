using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Extensions for the UnityEngine.Event class
    /// </summary>
    public static class EventExtensions
    {
        /// <summary>
        /// Determines if this event represents a left-mouse click event
        /// </summary>
        /// <param name="thisEvent">The event to check</param>
        /// <returns>True if it's a left click event, false if not</returns>
        public static bool IsLeftClickEvent(this Event thisEvent)
        {
            return thisEvent.type == EventType.MouseDown &&
                thisEvent.button == 0;
        }

        /// <summary>
        /// Determines if this event represents a right-mouse click event
        /// </summary>
        /// <param name="thisEvent">The event to check</param>
        /// <returns>True if it's a right click event, false if not</returns>
        public static bool IsRightClickEvent(this Event thisEvent)
        {
            return thisEvent.type == EventType.MouseDown &&
                thisEvent.button == 1;
        }
    }
}