using UnityEditor;
using UnityEngine;

namespace ZeroProgress.Common.Editors
{
    [CustomEditor(typeof(ScriptableAnimParam))]
    public class ScriptableAnimParamEditor : Editor
    {
        private void InitializeWithFileName()
        {
            ScriptableAnimParam scriptableAnimParam = target as ScriptableAnimParam;

            if (scriptableAnimParam.AnimParamName == null && !string.IsNullOrEmpty(target.name))
            {
                scriptableAnimParam.AnimParamName = target.name;
                scriptableAnimParam.DefaultValue = Animator.StringToHash(target.name);
                serializedObject.ApplyModifiedProperties();
            }
        }

        public override void OnInspectorGUI()
        {
            InitializeWithFileName();

            EditorGUI.BeginChangeCheck();
            
            SerializedProperty paramNameProp = serializedObject.FindProperty("AnimParamName");
            SerializedProperty defaultValueProp = serializedObject.FindProperty("DefaultValue");

            EditorGUILayout.PropertyField(paramNameProp);

            EditorGUI.BeginDisabledGroup(true);

            EditorGUILayout.PropertyField(defaultValueProp, new GUIContent("Param Hash"));

            EditorGUI.EndDisabledGroup();
            
            if (EditorGUI.EndChangeCheck())
            {
                defaultValueProp.intValue = Animator.StringToHash(paramNameProp.stringValue);
                serializedObject.ApplyModifiedProperties();
            }
        }

    }
}
