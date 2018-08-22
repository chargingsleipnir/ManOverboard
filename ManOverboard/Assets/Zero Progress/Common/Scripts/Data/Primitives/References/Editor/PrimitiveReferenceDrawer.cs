using UnityEditor;
using UnityEngine;

namespace ZeroProgress.Common.Editors
{
    /// <summary>
    /// PropertyDrawer for the primitive reference types
    /// </summary>
    /// <typeparam name="T">The type that the scriptableprimitive hosts</typeparam>
    /// <typeparam name="T1">The type of scriptableprimitive</typeparam>
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
            
            GUIContent straightValLabel = GUIContent.none;

            // Only show the straight value dropdown label if it is a custom class with children elements (need to ignore string for example)
            if (useStraightValue.boolValue && straightValue.hasChildren && straightValue.propertyType == SerializedPropertyType.Generic)
            {
                straightValLabel = new GUIContent("(straight value)");

                // shift to next line to make it a little neater
                position.yMin += EditorGUIUtility.singleLineHeight;
                position.height += EditorGUIUtility.singleLineHeight;
            }

            EditorGUI.PropertyField(position, useStraightValue.boolValue ? straightValue : variableValue, straightValLabel, true);

            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty useStraightValue = property.FindPropertyRelative("UseStraightValue");
            SerializedProperty straightValue = property.FindPropertyRelative("StraightValue");
            SerializedProperty variableValue = property.FindPropertyRelative("ScriptableValue");

            float height = EditorGUI.GetPropertyHeight(useStraightValue.boolValue ? straightValue : variableValue);

            if (useStraightValue.boolValue)
                height += EditorGUIUtility.singleLineHeight;

            return height;
        }
    }
}