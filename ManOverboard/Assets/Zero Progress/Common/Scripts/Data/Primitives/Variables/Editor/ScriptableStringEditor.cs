using UnityEditor;

namespace ZeroProgress.Common.Editors
{
    [CustomEditor(typeof(ScriptableString))]
    public class ScriptableStringEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            ScriptableString scriptableString = target as ScriptableString;

            if (scriptableString.DefaultValue == null && !string.IsNullOrEmpty(target.name))
            {
                scriptableString.DefaultValue = target.name;
                serializedObject.ApplyModifiedProperties();
            }

            base.OnInspectorGUI();
        }
    }
}
