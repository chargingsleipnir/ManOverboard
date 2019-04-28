using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ZeroProgress.Common
{
    [CustomPropertyDrawer(typeof(AnimParamSetterValue))]
    public class AnimParamSetterValuePropDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect valRect = new Rect(position);
            valRect.width = 140f;
            valRect.x = position.xMax - valRect.width;

            Rect typeRect = new Rect(position);
            typeRect.width = position.width * 0.3f;
            typeRect.x = valRect.x - typeRect.width;

            Rect paramRect = new Rect(position);
            paramRect.width = position.width - typeRect.width - valRect.width;


            SerializedProperty animParamProp = property.FindPropertyRelative(nameof(AnimParamSetterValue.AnimationParameter));
            SerializedProperty paramTypeProp = property.FindPropertyRelative(nameof(AnimParamSetterValue.ParameterType));
            
            AnimParamType paramType = (AnimParamType)paramTypeProp.enumValueIndex;

            SerializedProperty valueProp = GetValueProperty(property, paramType);

            GUIContent valueLabel = GUIContent.none;

            if (paramType == AnimParamType.TRIGGER)
                valueLabel = new GUIContent("Set Trigger");

            EditorGUI.BeginChangeCheck();

            EditorGUI.PropertyField(paramRect, animParamProp, GUIContent.none);
            EditorGUI.PropertyField(typeRect, paramTypeProp, GUIContent.none);

            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 90f;
            EditorGUI.PropertyField(valRect, valueProp, valueLabel);
            EditorGUIUtility.labelWidth = labelWidth;

            EditorGUI.EndChangeCheck();
        }

        private SerializedProperty GetValueProperty(SerializedProperty parentProp, AnimParamType paramType)
        {
            switch (paramType)
            {
                case AnimParamType.BOOL:
                    return parentProp.FindPropertyRelative(nameof(AnimParamSetterValue.BoolValue));
                case AnimParamType.FLOAT:
                    return parentProp.FindPropertyRelative(nameof(AnimParamSetterValue.FloatValue));
                case AnimParamType.INT:
                    return parentProp.FindPropertyRelative(nameof(AnimParamSetterValue.IntValue));
                case AnimParamType.TRIGGER:
                    return parentProp.FindPropertyRelative(nameof(AnimParamSetterValue.BoolValue));
                default:
                    return null;
            }
        }
    }
}