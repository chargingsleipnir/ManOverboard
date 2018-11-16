using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ZeroProgress.SceneManagementUtility.Editors
{
    public class SerializedSceneVariable
    {
        public SerializedProperty VariableElementProp { get; private set; }

        public SerializedProperty NameProp { get; private set; }

        public SerializedProperty ResetOnTransProp { get; private set; }

        public SerializedProperty ValueProp { get; private set; }

        public ScriptableObject ValueInstance
        {
            get
            {
                return ValueProp.objectReferenceValue as ScriptableObject;
            }
        }

        public SerializedSceneVariable(SerializedProperty variableElementProp)
        {
            VariableElementProp = variableElementProp;

            NameProp = VariableElementProp.FindPropertyRelative("Name");
            ResetOnTransProp = VariableElementProp.FindPropertyRelative("ResetOnTransition");
            ValueProp = VariableElementProp.FindPropertyRelative("value");
        }

        public SerializedObject GetSerializedValue()
        {
            return new SerializedObject(ValueProp.objectReferenceValue);
        }
    }
}