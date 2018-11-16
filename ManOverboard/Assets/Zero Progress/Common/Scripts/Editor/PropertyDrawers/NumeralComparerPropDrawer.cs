using UnityEditor;
using UnityEngine;

namespace ZeroProgress.Common.Editors
{
    [CustomPropertyDrawer(typeof(NumeralComparer))]
    public class NumeralComparerPropDrawer : PropertyDrawer
    {
        private static readonly GUIContent[] numeralComparerReps = new GUIContent[]
        {
            new GUIContent("None"),
            new GUIContent("<", "Less Than"),
            new GUIContent("<=", "Less Than or Equal to"),
            new GUIContent("==", "Equal to"),
            new GUIContent("!=", "Not Equal to"),
            new GUIContent(">=", "Greater Than or Equal to"),
            new GUIContent(">", "Greater Than")
        };
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();

            property.enumValueIndex =
                EditorGUI.Popup(position, label, property.enumValueIndex, numeralComparerReps, EditorStyles.popup);

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}