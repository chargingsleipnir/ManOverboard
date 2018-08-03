using UnityEditor;
using UnityEngine;

namespace ZeroProgress.Common.Editors
{
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

            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);

            EditorGUI.BeginChangeCheck();

            SerializedProperty useStraightValue = property.FindPropertyRelative("UseStraightValue");
            SerializedProperty straightValue = property.FindPropertyRelative("StraightValue");
            SerializedProperty variableValue = property.FindPropertyRelative("ScriptableValue");
            SerializedProperty paramNameValue = property.FindPropertyRelative("AnimParamName");

            bool isUsingStraightValue = useStraightValue.boolValue;

            if (isUsingStraightValue)
            {
                position.height *= 0.5f;
                position.height -= 1f;
            }

            Rect buttonRect = new Rect(position);
            buttonRect.yMin += popupStyle.margin.top;
            buttonRect.width = popupStyle.fixedWidth + popupStyle.margin.right;
            position.xMin = buttonRect.xMax;

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

                position.y += EditorGUI.GetPropertyHeight(isUsingStraightValue ? paramNameValue : variableValue);
                position.y += popupStyle.margin.top;
                position.yMax -= 1f;
                position.xMin = 34f;

                EditorGUI.PropertyField(position, straightValue, new GUIContent("Param Hash"));

                EditorGUI.EndDisabledGroup();
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float bottomMargin = 3f;
            float propertyRows = 2f;

            SerializedProperty useStraightValue = property.FindPropertyRelative("UseStraightValue");

            float propertyHeight = base.GetPropertyHeight(property, label);

            if (useStraightValue == null)
            {
                Debug.LogError("It's Null");
                return propertyHeight;
            }
            if (useStraightValue.boolValue)
                propertyHeight = propertyHeight * propertyRows + bottomMargin;

            return propertyHeight;
        }
    }
}