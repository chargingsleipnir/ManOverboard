using UnityEditor;
using UnityEngine;

namespace ZeroProgress.Common.Editors
{
    /// <summary>
    /// Editor for GameEvents that creates a 'test' button that is only enabled while
    /// the application is running. Allows easy testing of event response
    /// </summary>
    [CustomEditor(typeof(GameEvent))]
    public class GameEventEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = Application.isPlaying;

            GameEvent e = target as GameEvent;

            if (GUILayout.Button("Raise Event"))
                e.RaiseEvent();
        }
    }
}
