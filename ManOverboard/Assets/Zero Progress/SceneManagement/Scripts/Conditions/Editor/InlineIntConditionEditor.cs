using System;
using UnityEditor;
using UnityEngine;
using ZeroProgress.Common;

namespace ZeroProgress.SceneManagementUtility.Editors
{
    [CustomEditor(typeof(IntSceneCondition))]
    public class InlineIntConditionEditor : InlineSceneConditionEditor
    {
        public override float GetInlineEditorHeight()
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnInlineEditorGUI(Rect rect)
        {
            serializedObject.Update();

            SerializedProperty variableNameProp = serializedObject.FindProperty("VariableName");
            SerializedProperty desiredValueProp = serializedObject.FindProperty("DesiredValue");
            SerializedProperty compareModeProp = serializedObject.FindProperty("CompareMode");

            Rect[] splits = rect.SplitRectHorizontally(3, 4);
            
            variableNameProp.stringValue = RenderVariableSelector(splits[0],
                string.Empty, variableNameProp.stringValue, VariablesList, IsValidVariableType);

            EditorGUI.PropertyField(splits[1], compareModeProp, GUIContent.none);

            EditorGUI.PropertyField(splits[2], desiredValueProp, GUIContent.none);

            serializedObject.ApplyModifiedProperties();
        }

        private static bool IsValidVariableType(Type variableType)
        {
            return variableType == typeof(int) ||
                variableType == typeof(long) ||
                variableType == typeof(short);
        }
    }
}