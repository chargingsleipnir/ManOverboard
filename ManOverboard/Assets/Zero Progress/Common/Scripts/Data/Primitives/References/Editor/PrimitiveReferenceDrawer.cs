using UnityEditor;
using UnityEngine;

namespace ZeroProgress.Common.Editors
{
    public class PrimitiveReferenceDrawer<T, T1> : PropertyDrawer where T1 : ScriptablePrimitive<T>
    {
        protected readonly string[] valueSelectionOptions =
        {
        "Use Straight Value",
        "Use Reference Value"
    };

        protected GUIStyle popupStyle;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (popupStyle == null)
            {
                popupStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions"));
                popupStyle.imagePosition = ImagePosition.ImageOnly;
            }

            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);

            EditorGUI.BeginChangeCheck();

            SerializedProperty useStraightValue = property.FindPropertyRelative("UseStraightValue");
            SerializedProperty straightValue = property.FindPropertyRelative("StraightValue");
            SerializedProperty variableValue = property.FindPropertyRelative("ScriptableValue");

            Rect buttonRect = new Rect(position);
            buttonRect.yMin += popupStyle.margin.top;
            buttonRect.width = popupStyle.fixedWidth + popupStyle.margin.right;
            position.xMin = buttonRect.xMax;

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            int result = EditorGUI.Popup(buttonRect, useStraightValue.boolValue ? 0 : 1, valueSelectionOptions, popupStyle);

            useStraightValue.boolValue = result == 0;

            EditorGUI.PropertyField(position, useStraightValue.boolValue ? straightValue : variableValue, GUIContent.none);

            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}