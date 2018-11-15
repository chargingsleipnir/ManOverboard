using UnityEditor;
using UnityEngine;

namespace ZeroProgress.Common.Editors
{
    /// <summary>
    /// Property drawer for viewing boolean as an enum
    /// </summary>
    [CustomPropertyDrawer(typeof(BoolDropDownAttribute))]
    public class BoolDropDownPropertyDrawer : PropertyDrawer
    {
        /// <summary>
        /// The available options
        /// </summary>
        private static readonly GUIContent[] options = new GUIContent[]
        {
            new GUIContent("True"),
            new GUIContent("False")
        };

        public override void OnGUI(Rect position, 
            SerializedProperty property, GUIContent label)
        {
            bool value = property.boolValue;

            int index = value ? 0 : 1;

            position = EditorGUI.PrefixLabel(position, label);

            index = EditorGUI.Popup(position, index, options);

            property.boolValue = index == 0 ? true : false;

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
