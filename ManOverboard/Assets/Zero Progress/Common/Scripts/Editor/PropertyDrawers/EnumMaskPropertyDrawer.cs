using UnityEditor;
using UnityEngine;

namespace ZeroProgress.Common.Editors
{
    [CustomPropertyDrawer(typeof(EnumMaskAttribute))]
    public class EnumMaskPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (SerializedPropertyType.Enum == property.propertyType)
            {
                object current = GetCurrent(property);

                if (current == null)
                    return;
                
                EditorGUI.BeginChangeCheck();

                System.Enum value = EditorGUI.EnumFlagsField(position, label, (System.Enum)current);

                if (EditorGUI.EndChangeCheck())
                    property.intValue = System.Convert.ToInt32(value);                
            }
            else
                EditorGUI.LabelField(position, label, new GUIContent(
                    "This type is not supported as an enum mask."));
        }

        private static object GetCurrent(SerializedProperty property)
        {
            object result = property.serializedObject.targetObject;

            string[] property_names = property.propertyPath.Replace(".Array.data", ".").Split('.');

            foreach (var property_name in property_names)
            {
                object parent = result;
                int indexer_start = property_name.IndexOf('[');
                if (indexer_start < 0)
                {
                    var binding_flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;
                    result = parent.GetType().GetField(property_name, binding_flags).GetValue(parent);
                }
                else if (parent.GetType().IsArray)
                {
                    int indexer_end = property_name.IndexOf(']');
                    string index_string = property_name.Substring(indexer_start + 1, indexer_end - indexer_start - 1);
                    int index = int.Parse(index_string);

                    System.Array array = (System.Array)parent;

                    if (index < array.Length)
                        result = array.GetValue(index);
                    else
                    {
                        result = null;
                        break;
                    }
                }
                else
                {
                    throw new System.MissingFieldException();
                }
            }
            return result;
        }
    }
}