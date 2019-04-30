using System;
using UnityEditor;
using UnityEngine;
using ZeroProgress.Common;
using ZeroProgress.Common.Editors;

namespace ZeroProgress.SceneManagementUtility.Editors
{
    [CustomEditor(typeof(BoolSceneCondition))]
    public class InlineBoolConditionEditor : InlineSceneConditionEditor
    {
        private BoolDropDownPropertyDrawer boolDropDown = new BoolDropDownPropertyDrawer();

        public override float GetInlineEditorHeight()
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }

        public override void OnInlineEditorGUI(Rect rect)
        {
            serializedObject.Update();

            SerializedProperty variableNameProp = serializedObject.FindProperty("VariableName");
            SerializedProperty desiredValueProp = serializedObject.FindProperty("DesiredValue");

            Rect variableRect = new Rect(rect);
            variableRect.xMax = variableRect.center.x - 2;

            variableNameProp.stringValue = RenderVariableSelector(variableRect,
                string.Empty, variableNameProp.stringValue, VariablesList, IsValidVariableType);
            
            Rect toggleRect = new Rect(rect);
            toggleRect.xMin = variableRect.xMax + 4;

            boolDropDown.OnGUI(toggleRect, desiredValueProp, GUIContent.none);

            serializedObject.ApplyModifiedProperties();
        }

        private static bool IsValidVariableType(Type variableType)
        {
            return variableType == typeof(bool);
        }
    }
}