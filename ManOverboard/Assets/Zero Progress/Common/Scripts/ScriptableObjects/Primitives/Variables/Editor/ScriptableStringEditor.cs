using UnityEditor;

namespace ZeroProgress.Common.Editors
{
    /// <summary>
    /// Editor for the scriptable string type
    /// </summary>
    [CustomEditor(typeof(ScriptableString))]
    public class ScriptableStringEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            ScriptableString scriptableString = target as ScriptableString;

            // If the item has not had its value initialized, set the name of the asset as the value (as a convenience function)
            if (scriptableString.DefaultValue == null && !string.IsNullOrEmpty(target.name))
            {
                SerializedProperty defaultValueProp = serializedObject.FindProperty("DefaultValue");

                defaultValueProp.stringValue = target.name;
                serializedObject.ApplyModifiedProperties();
            }

            base.OnInspectorGUI();
        }
    }
}
