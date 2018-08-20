using UnityEditor;
using UnityEngine;

namespace ZeroProgress.Common.Editors
{
    /// <summary>
    /// PropertyDrawer for an Animation Parameter reference in order to display the animation 
    /// parameter name, but store the hash behind the scenes
    /// </summary>
    [CustomPropertyDrawer(typeof(AnimParamReference))]
    public class AnimParamReferenceDrawer : PrimitiveReferenceDrawer<int, ScriptableAnimParam>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (popupStyle == null)
            {
                popupStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions"));
                popupStyle.imagePosition = ImagePosition.ImageOnly;
            }

            Rect beforeLabelPos = new Rect(position);

            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);

            EditorGUI.BeginChangeCheck();

            SerializedProperty useStraightValue = property.FindPropertyRelative("UseStraightValue");
            SerializedProperty straightValue = property.FindPropertyRelative("StraightValue");
            SerializedProperty variableValue = property.FindPropertyRelative("ScriptableValue");
            SerializedProperty paramNameValue = property.FindPropertyRelative("AnimParamName");

            bool isUsingStraightValue = useStraightValue.boolValue;
            
            Rect buttonRect = new Rect(position);
            buttonRect.width = popupStyle.fixedWidth + popupStyle.margin.right;
            position.xMin = buttonRect.xMax;
            position.height = EditorGUIUtility.singleLineHeight;

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            int result = EditorGUI.Popup(buttonRect, isUsingStraightValue ? 0 : 1, valueSelectionOptions, popupStyle);

            isUsingStraightValue = useStraightValue.boolValue = result == 0;

            EditorGUI.PropertyField(position, isUsingStraightValue ? paramNameValue : variableValue, GUIContent.none);

            if (EditorGUI.EndChangeCheck())
            {
                if (isUsingStraightValue)
                    straightValue.intValue = Animator.StringToHash(paramNameValue.stringValue);

                property.serializedObject.ApplyModifiedProperties();
            }

            if (isUsingStraightValue)
            {
                EditorGUI.BeginDisabledGroup(true);

                position = beforeLabelPos;
                position.y += EditorGUIUtility.singleLineHeight;
                position.height = EditorGUIUtility.singleLineHeight;

                EditorGUI.indentLevel++;
                position = EditorGUI.IndentedRect(position);

                EditorGUI.PropertyField(position, straightValue, new GUIContent("Param Hash"));

                EditorGUI.EndDisabledGroup();
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty useStraightValue = property.FindPropertyRelative("UseStraightValue");

            float propertyHeight = EditorGUIUtility.singleLineHeight;

            if (useStraightValue == null)
                return propertyHeight;

            if (useStraightValue.boolValue)
                propertyHeight = propertyHeight * 2;

            return propertyHeight;
        }
    }
}