using UnityEditor;
using UnityEngine;

namespace ZeroProgress.Common.Editors
{
    /// <summary>
    /// Editor for ScriptableAnimationParameters to allow viewing/setting the information
    /// by animation parameter name rather than by hash
    /// </summary>
    [CustomEditor(typeof(ScriptableAnimParam))]
    public class ScriptableAnimParamEditor : Editor
    {
        /// <summary>
        /// Helper used to get the assets' filename and apply it to the Animation Param value in case
        /// the parameter asset is named the same (convenience to save typing it twice)
        /// </summary>
        private void InitializeWithFileName()
        {
            ScriptableAnimParam scriptableAnimParam = target as ScriptableAnimParam;

            if (scriptableAnimParam.AnimParamName == null && !string.IsNullOrEmpty(target.name))
            {
                SerializedProperty paramNameProp = serializedObject.FindProperty("AnimParamName");
                SerializedProperty defaultValueProp = serializedObject.FindProperty("DefaultValue");

                paramNameProp.stringValue = target.name;
                defaultValueProp.intValue = Animator.StringToHash(target.name);

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
