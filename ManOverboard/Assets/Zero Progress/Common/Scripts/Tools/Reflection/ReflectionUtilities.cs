using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ZeroProgress.Common.Reflection
{
    public class UnityObjectFieldMapping
    {
        public string FieldName;
        public UnityEngine.Object UnityObject;

        public UnityObjectFieldMapping(string fieldName, UnityEngine.Object unityObject)
        {
            this.FieldName = fieldName;
            this.UnityObject = unityObject;
        }
    }

    /// <summary>
    /// Collection of helper methods related to reflection
    /// </summary>
    public static class ReflectionUtilities
    {
        /// <summary>
        /// Retrieves the value of a field by its name
        /// </summary>
        /// <typeparam name="T">The type of value to be returned</typeparam>
        /// <param name="obj">The object to be searched for the field</param>
        /// <param name="fieldName">Name of the value to extract the value from</param>
        /// <param name="bindingFlags">How to go about searching the object</param>
        /// <returns>The value of the field</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the field under the specified name can't be found</exception>
        public static T GetFieldByName<T>(object obj, string fieldName, BindingFlags bindingFlags)
        {
            FieldInfo fieldInfo = obj.GetType().GetField(fieldName, bindingFlags);

            if (fieldInfo == null)
                throw new KeyNotFoundException("Field not found under specified name: " + fieldName);

            return (T)fieldInfo.GetValue(obj);
        }

        /// <summary>
        /// Retrieves the value of a field by its name, without throwing an exception
        /// </summary>
        /// <typeparam name="T">The type of value to be returned</typeparam>
        /// <param name="obj">The object to be searched for the field</param>
        /// <param name="fieldName">Name of the value to extract the value from</param>
        /// <param name="bindingFlags">How to go about searching the object</param>
        /// <param name="value">The extracted value</param>
        /// <returns>True if found, false if not. Value will contain the extracted value if true is returned</returns>
        public static bool TryGetFieldByName<T>(object obj, string fieldName, BindingFlags bindingFlags, out T value)
        {
            FieldInfo fieldInfo = obj.GetType().GetField(fieldName, bindingFlags);

            if (fieldInfo == null)
            {
                value = default(T);
                return false;
            }

            value = (T)fieldInfo.GetValue(obj);
            return true;
        }

        /// <summary>
        /// Retrieves the value of a field by its name
        /// </summary>
        /// <param name="obj">The object to be searched for the field</param>
        /// <param name="fieldName">Name of the value to extract the value from</param>
        /// <param name="bindingFlags">How to go about searching the object</param>
        /// <returns>The value of the field</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the field under the specified name can't be found</exception>
        public static Object GetFieldByName(object obj, string fieldName, BindingFlags bindingFlags)
        {
            FieldInfo fieldInfo = obj.GetType().GetField(fieldName, bindingFlags);

            if (fieldInfo == null)
                throw new KeyNotFoundException("Field not found under specified name: " + fieldName);

            return fieldInfo.GetValue(obj);
        }

        /// <summary>
        /// Sets the value of a field by its name
        /// </summary>
        /// <param name="obj">The object to be searched for the field</param>
        /// <param name="fieldName">Name of the value to extract the value from</param>
        /// <param name="bindingFlags">How to go about searching the object</param>
        /// <param name="value">The value to set</param>
        /// <returns>The value of the field</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the field under the specified name can't be found</exception>
        public static void SetFieldByName(object obj, string fieldName, BindingFlags bindingFlags, object value)
        {
            FieldInfo fieldInfo = obj.GetType().GetField(fieldName, bindingFlags);

            if (fieldInfo == null)
                throw new KeyNotFoundException("Field not found under specified name: " + fieldName);

            fieldInfo.SetValue(obj, value);
        }

        /// <summary>
        /// Sets the value of a field by its name, without throwing an exception if it isn't found
        /// </summary>
        /// <param name="obj">The object to be searched for the field</param>
        /// <param name="fieldName">Name of the value to extract the value from</param>
        /// <param name="bindingFlags">How to go about searching the object</param>
        /// <param name="value">The value to set</param>
        /// <returns>True if the field was successfully set, false if the field couldn't be found</returns>
        public static bool TrySetFieldByName(object obj, string fieldName, BindingFlags bindingFlags, object value)
        {
            FieldInfo fieldInfo = obj.GetType().GetField(fieldName, bindingFlags);

            if (fieldInfo == null)
                return false;

            fieldInfo.SetValue(obj, value);
            return true;
        }

        /// <summary>
        /// Uses reflection to find all fields that are a descendent of UnityEngine.Object
        /// </summary>
        /// <param name="obj">The object to search through</param>
        /// <param name="searchChildren">*NOT IMPLEMENTED* True to search through nested POCO instances
        /// for their unity objects as well, false to stick to the top level</param>
        /// <returns>A collection of unity engine objects (empty collection if none are found)</returns>
        public static IEnumerable<UnityEngine.Object> FindAllUnityObjects(object obj, bool searchChildren = false)
        {
            return FindAllUnityObjectsWithName(obj, searchChildren).Select((x) => x.UnityObject);
        }

        /// <summary>
        /// Uses reflection to find all fields that are a descendent of UnityEngine.Object and
        /// includes the name of the field with it
        /// </summary>
        /// <param name="obj">The object to search through</param>
        /// <param name="searchChildren">*NOT IMPLEMENTED* True to search through nested POCO instances
        /// for their unity objects as well, false to stick to the top level</param>
        /// <returns>A collection of unity engine objects with the name of the field they were extracted from
        /// (empty collection if none are found)</returns>
        public static IEnumerable<UnityObjectFieldMapping> FindAllUnityObjectsWithName(object obj, bool searchChildren = false)
        {
            List<UnityObjectFieldMapping> foundItems = new List<UnityObjectFieldMapping>();

            if (obj == null)
                return foundItems;

            Type itemType = obj.GetType();

            if (itemType.IsSubclassOf(typeof(UnityEngine.Object)))
                return foundItems;

            IEnumerable<FieldInfo> fields = FindAllUnitySerializableFields(obj);

            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsSubclassOf(typeof(UnityEngine.Object)))
                {
                    System.Object fieldValue = field.GetValue(obj);

                    if (fieldValue == null)
                        continue;

                    UnityEngine.Object fieldValueAsUnityObject = fieldValue as UnityEngine.Object;

                    if (fieldValueAsUnityObject == null)
                        continue;

                    if (foundItems.Find((x) => x.UnityObject == fieldValueAsUnityObject) != null)
                        continue;

                    foundItems.Add(new UnityObjectFieldMapping(field.Name, fieldValueAsUnityObject));
                }
            }

            return foundItems;
        }

        /// <summary>
        /// Searches through an object and retrieves all of the unity serializable fields within.
        /// 
        /// A unity serializable field is any field that is public (and not with the attribute
        /// [Non-Serialized]) or any field that is private and with the [SerializeField] attribute.
        /// It also cannot be a generic type
        /// </summary>
        /// <param name="obj">The object to search through</param>
        /// <returns>A collection of FieldInfo representing all discovered Unity-Serializable fields</returns>
        public static IEnumerable<FieldInfo> FindAllUnitySerializableFields(object obj)
        {
            if (obj == null)
                return new List<FieldInfo>();

            Type itemType = obj.GetType();
            
            IEnumerable<FieldInfo> fields = itemType.GetFields(BindingFlags.Public |
                BindingFlags.NonPublic | BindingFlags.Instance);

            return fields.Where((x) => IsUnitySerializableField(x));
        }

        /// <summary>
        /// Helper to determine if the field is a Unity-Serializable field or not
        /// </summary>
        /// <param name="field">The field to determine serializability of </param>
        /// <returns>True if unity can serialize this field, false if not</returns>
        public static bool IsUnitySerializableField(FieldInfo field)
        {
            object[] attributes = field.GetCustomAttributes(true);

            if (attributes == null || attributes.Length == 0)
                return field.IsPublic;

            if (attributes.OfType<SerializableAttribute>().FirstOrDefault() != null)
                return true;

            if (attributes.OfType<NonSerializedAttribute>().FirstOrDefault() != null)
                return false;

            return field.IsPublic;
        }
    }
}