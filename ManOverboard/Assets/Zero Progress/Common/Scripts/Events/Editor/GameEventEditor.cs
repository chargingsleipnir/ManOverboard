using UnityEditor;
using UnityEngine;
using ZeroProgress.Common.Reflection;

namespace ZeroProgress.Common.Editors
{
    /// <summary>
    /// Editor for GameEvents that creates a 'test' button that is only enabled while
    /// the application is running. Allows easy testing of event response
    /// </summary>
    [CustomEditor(typeof(GameEvent))]
    public class GameEventEditor : Editor
    {
        GameEvent targetGameEvent = null;
        SerializedProperty eventIdProp = null;

        string eventId = null;

        public override void OnInspectorGUI()
        {
            if(targetGameEvent == null)
            {
                targetGameEvent = target as GameEvent;

                eventIdProp = serializedObject.FindProperty("eventId");

                eventId = ReflectionUtilities.GetFieldByName(targetGameEvent, "eventId", 
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic) as string;
            }
            
            // If the item has not had its value initialized, set the name of the asset as the value (as a convenience function)
            if (eventId == null && !string.IsNullOrEmpty(target.name))
            {
                eventIdProp.stringValue = target.name;
                eventId = target.name;
                serializedObject.ApplyModifiedProperties();
            }

            base.OnInspectorGUI();

            GUI.enabled = Application.isPlaying;

            if (GUILayout.Button("Raise Event"))
                targetGameEvent.RaiseEvent();
        }
    }
}
