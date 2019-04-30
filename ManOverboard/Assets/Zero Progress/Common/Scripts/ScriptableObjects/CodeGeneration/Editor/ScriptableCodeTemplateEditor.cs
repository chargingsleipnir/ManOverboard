using UnityEditor;
using UnityEngine;

namespace ZeroProgress.Common.Editors
{
    /// <summary>
    /// Editor for the ScriptableCodeTemplate asset
    /// </summary>
    [CustomEditor(typeof(ScriptableCodeTemplate))]
    public class ScriptableCodeTemplateEditor : Editor
    {
        private const string FOCUS_IDENTIFIER = "CodeTemplateText";

        private Vector2 scrollPosition;

        private string lastFocusedControl = "";

        public override void OnInspectorGUI()
        {
            SerializedProperty valueProperty = serializedObject.FindProperty("currentValue");
            SerializedProperty defaultProperty = serializedObject.FindProperty("DefaultValue");

            DrawPropertiesExcluding(serializedObject, "DefaultValue", "currentValue", "parameters");
            
            EditorGUILayout.LabelField("Code Template");

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            GUI.SetNextControlName(FOCUS_IDENTIFIER);

            valueProperty.stringValue = EditorGUILayout.TextArea(valueProperty.stringValue, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));

            defaultProperty.stringValue = valueProperty.stringValue;
            
            EditorGUILayout.EndScrollView();

            serializedObject.ApplyModifiedProperties();

            string nameOfFocusedControl = GUI.GetNameOfFocusedControl();

            if (nameOfFocusedControl != FOCUS_IDENTIFIER && lastFocusedControl == FOCUS_IDENTIFIER)
            {
                Reflection.ReflectionUtilities.InvokeMethod(target, "ParseParameters",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, valueProperty.stringValue);

                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
            }

            lastFocusedControl = GUI.GetNameOfFocusedControl();
        }
    }
}