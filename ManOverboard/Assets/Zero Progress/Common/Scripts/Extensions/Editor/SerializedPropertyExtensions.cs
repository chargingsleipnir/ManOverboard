using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;

namespace ZeroProgress.Common.Editors
{
    /// <summary>
    /// Extensions for the SerializedProperty class
    /// </summary>
    public static class SerializedPropertyExtensions 
    {
        /// <summary>
        /// Adds the specified object to the array (if the provided property is actually an array)
        /// </summary>
        /// <param name="ThisProperty">The array property to add an element to</param>
        /// <param name="NewValue">The value to be added</param>
        public static void AddArrayObjectValue(this SerializedProperty ThisProperty, UnityEngine.Object NewValue)
        {
            if (!ThisProperty.isArray)
                return;

            ThisProperty.arraySize++;

            ThisProperty.GetArrayElementAtIndex(ThisProperty.arraySize - 1).objectReferenceValue = NewValue;
            
            ThisProperty.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Adds an array element to the specified property, if it is an Array property
        /// </summary>
        /// <param name="ThisProperty">The property to add an element to</param>
        /// <returns>The added serialized property reference, or null if not an array property</returns>
        public static SerializedProperty AddArrayElement(this SerializedProperty ThisProperty)
        {
            if (!ThisProperty.isArray)
                return null;

            ThisProperty.arraySize++;

            ThisProperty.serializedObject.ApplyModifiedProperties();

            return ThisProperty.GetArrayElementAtIndex(ThisProperty.arraySize - 1);
        }

        /// <summary>
        /// Adds an array value to the property
        /// </summary>
        /// <param name="ThisProperty">The property to add a value to</param>
        /// <param name="NewValue">The new value to be set. Must match the type returned by the property's
        /// PropertyType value</param>
        public static bool AddArrayValue(this SerializedProperty ThisProperty, System.Object NewValue)
        {
            if (!ThisProperty.isArray)
                return false;

            ThisProperty.arraySize++;

            SerializedProperty elementProp = ThisProperty.GetArrayElementAtIndex(ThisProperty.arraySize - 1);

            bool result = elementProp.TrySetValue(NewValue);

            ThisProperty.serializedObject.ApplyModifiedProperties();
            
            return result;
        }

        /// <summary>
        /// Adds an array value to the property if it doesn't already exist
        /// </summary>
        /// <param name="ThisProperty">The property to add a value to</param>
        /// <param name="NewValue">The new value to be set. Must match the type returned by the property's
        /// PropertyType value</param>
        public static bool AddUniqueArrayValue(this SerializedProperty ThisProperty, System.Object NewValue)
        {
            if (!ThisProperty.isArray)
                return false;

            int arrayIndex = ThisProperty.FindArrayIndexOf(NewValue);

            if (arrayIndex >= 0)
                return false;

            return ThisProperty.AddArrayValue(NewValue);
        }

        /// <summary>
        /// Gets the SerializedProperty that represents the first item in the array
        /// </summary>
        /// <param name="ThisProperty">The property to get the array iterator from</param>
        /// <param name="ArrayLength">The length of the array</param>
        /// <returns>The serialized property representing the first item in the array, or null
        /// if the serialized property isn't an array or is empty</returns>
        public static SerializedProperty GetArrayIterator(this SerializedProperty ThisProperty, out int ArrayLength)
        {
            ArrayLength = 0;

            if (!ThisProperty.isArray)
                return null;

            SerializedProperty copyOfThis = ThisProperty.Copy();

            // Skip generic field
            copyOfThis.Next(true);

            // Advance to array size field
            copyOfThis.Next(true);

            ArrayLength = copyOfThis.intValue;

            // Advance to first array index
            if (!copyOfThis.Next(true))
                return null;

            return copyOfThis;
        }

        /// <summary>
        /// Retrieves the index of the specified item within the array that the property hosts
        /// </summary>
        /// <param name="ThisProperty">The property that is expected to be an array</param>
        /// <param name="ValueToFind">The value to search for</param>
        /// <returns>The index of the item, or -1 if not found or the property isn't an array</returns>
        public static int FindArrayIndexOf(this SerializedProperty ThisProperty, System.Object ValueToFind)
        {
            if (!ThisProperty.isArray)
                return -1;

            for (int i = 0; i < ThisProperty.arraySize; i++)
            {
                SerializedProperty current = ThisProperty.GetArrayElementAtIndex(i);

                if (current == null)
                    continue;

                System.Object propertyValue;

                if (!current.TryGetValue(out propertyValue))
                    continue;

                if (propertyValue.Equals(ValueToFind))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Removes the item from the array
        /// </summary>
        /// <param name="ThisProperty">The property that is expected to be an array</param>
        /// <param name="ValueToRemove">The value to search for</param>
        /// <returns>True if the item was removed, false if not</returns>
        public static bool RemoveFromArray(this SerializedProperty ThisProperty, System.Object ValueToRemove)
        {
            int arrayIndex = ThisProperty.FindArrayIndexOf(ValueToRemove);

            if (arrayIndex < 0)
                return false;

            SerializedProperty found = ThisProperty.GetArrayElementAtIndex(arrayIndex);

            // This is required because DeleteArrayElementAtIndex will only set the value
            // to null if the value isn't null... it only truly removes array elements if the 
            // element value is null
            if (found != null && found.propertyType == SerializedPropertyType.ObjectReference)
                found.objectReferenceValue = null;

            ThisProperty.DeleteArrayElementAtIndex(arrayIndex);

            ThisProperty.serializedObject.ApplyModifiedProperties();

            return true;
        }

        /// <summary>
        /// Retrieves the array values of this property
        /// </summary>
        /// <typeparam name="T">The type to retrieve</typeparam>
        /// <param name="ThisProperty">The property to retrieve the array values for</param>
        /// <returns>A collection of values of the specified type. Note that a string type will work
        /// for any SerializedProperty type, which may not be desired.</returns>
        public static IEnumerable<T> GetArrayValues<T>(this SerializedProperty ThisProperty)
        {
            int arrayLength = 0;

            SerializedProperty arrayIterator = ThisProperty.GetArrayIterator(out arrayLength);

            if (arrayIterator == null)
                return null;
            
            List<System.Object> values = new List<System.Object>();

            int lastIndex = arrayLength - 1;

            for (int i = 0; i < arrayLength; i++)
            {
                System.Object value;

                if (!arrayIterator.TryGetValue(out value))
                    continue;

                values.Add(value);

                if (i < lastIndex)
                    arrayIterator.Next(false);
            }

            return values.Cast<T>();
        }

        /// <summary>
        /// Retrieves the array values of this property in a manner that doesn't return null
        /// (will create an empty collection)
        /// </summary>
        /// <typeparam name="T">The element type</typeparam>
        /// <param name="ThisProperty">The serialized property to get the array values from</param>
        /// <returns>The collection of items (or an empty collection if null)</returns>
        public static IEnumerable<T> GetSafeArrayValues<T>(this SerializedProperty ThisProperty)
        {
            IEnumerable<T> values = ThisProperty.GetArrayValues<T>();

            if (values == null)
                return new List<T>();
            else
                return values;
        }

        /// <summary>
        /// Sets the value of the serialized property based on the type
        /// </summary>
        /// <param name="ThisProperty">The property to set the value for</param>
        /// <param name="NewValue">The value to be set. Must match the type of the serialized property</param>
        /// <returns>True if successfully set, false if the serialized propert is of an unsupported type
        /// or if the casting of the value failed</returns>
        public static bool TrySetValue(this SerializedProperty ThisProperty, System.Object NewValue)
        {
            SerializedPropertyType propType = ThisProperty.propertyType;

            Type valueType = NewValue.GetType();

            switch (propType)
            {
                case SerializedPropertyType.Integer:

                    if (valueType != typeof(int))
                        return false;

                    ThisProperty.intValue = (int)NewValue;
                    break;
                case SerializedPropertyType.Boolean:
                    if (valueType != typeof(bool))
                        return false;

                    ThisProperty.boolValue = (bool)NewValue;
                    break;
                case SerializedPropertyType.Float:

                    if (valueType != typeof(float))
                        return false;

                    ThisProperty.floatValue = (float)NewValue;
                    break;
                case SerializedPropertyType.String:

                    if (valueType != typeof(string))
                        return false;

                    ThisProperty.stringValue = (string)NewValue;
                    break;
                case SerializedPropertyType.Color:

                    if (valueType != typeof(Color))
                        return false;

                    ThisProperty.colorValue = (Color)NewValue;
                    break;
                case SerializedPropertyType.ObjectReference:

                    if (valueType != typeof(UnityEngine.Object))
                        return false;

                    ThisProperty.objectReferenceValue = (UnityEngine.Object)NewValue;
                    break;
                case SerializedPropertyType.Vector2:

                    if (valueType != typeof(Vector2))
                        return false;

                    ThisProperty.vector2Value = (Vector2)NewValue;
                    break;
                case SerializedPropertyType.Vector3:

                    if (valueType != typeof(Vector3))
                        return false;

                    ThisProperty.vector3Value = (Vector3)NewValue;
                    break;
                case SerializedPropertyType.Vector4:

                    if (valueType != typeof(Vector4))
                        return false;

                    ThisProperty.vector4Value = (Vector4)NewValue;
                    break;
                case SerializedPropertyType.Rect:

                    if (valueType != typeof(Rect))
                        return false;

                    ThisProperty.rectValue = (Rect)NewValue;
                    break;
                case SerializedPropertyType.AnimationCurve:

                    if (valueType != typeof(AnimationCurve))
                        return false;

                    ThisProperty.animationCurveValue = (AnimationCurve)NewValue;

                    break;
                case SerializedPropertyType.Bounds:

                    if (valueType != typeof(Bounds))
                        return false;

                    ThisProperty.boundsValue = (Bounds)NewValue;
                    break;
                case SerializedPropertyType.Quaternion:

                    if (valueType != typeof(Quaternion))
                        return false;

                    ThisProperty.quaternionValue = (Quaternion)NewValue;
                    break;
                case SerializedPropertyType.Vector2Int:

                    if (valueType != typeof(Vector2Int))
                        return false;

                    ThisProperty.vector2IntValue = (Vector2Int)NewValue;
                    break;
                case SerializedPropertyType.Vector3Int:

                    if (valueType != typeof(Vector3Int))
                        return false;

                    ThisProperty.vector3IntValue = (Vector3Int)NewValue;
                    break;
                case SerializedPropertyType.RectInt:

                    if (valueType != typeof(RectInt))
                        return false;

                    ThisProperty.rectIntValue = (RectInt)NewValue;
                    break;
                case SerializedPropertyType.BoundsInt:

                    if (valueType != typeof(BoundsInt))
                        return false;

                    ThisProperty.boundsIntValue = (BoundsInt)NewValue;
                    break;
                default:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Retrieves the value of the property
        /// </summary>
        /// <param name="ThisProperty">The property to retrieve the value from</param>
        /// <param name="RetrievedValue">The returned value</param>
        /// <returns>True if successfully retrieved, false if unsupported PropertyType</returns>
        public static bool TryGetValue(this SerializedProperty ThisProperty, out System.Object RetrievedValue)
        {
            SerializedPropertyType propType = ThisProperty.propertyType;

            RetrievedValue = null;

            switch (propType)
            {
                case SerializedPropertyType.Integer:

                    RetrievedValue = ThisProperty.intValue;

                    break;
                case SerializedPropertyType.Boolean:

                    RetrievedValue = ThisProperty.boolValue;

                    break;
                case SerializedPropertyType.Float:
                    
                    RetrievedValue = ThisProperty.floatValue;

                    break;
                case SerializedPropertyType.String:

                    RetrievedValue = ThisProperty.stringValue;

                    break;
                case SerializedPropertyType.Color:
                    
                    RetrievedValue = ThisProperty.colorValue;

                    break;
                case SerializedPropertyType.ObjectReference:

                    RetrievedValue = ThisProperty.objectReferenceValue;
                    break;
                case SerializedPropertyType.Vector2:

                    RetrievedValue = ThisProperty.vector2Value;

                    break;
                case SerializedPropertyType.Vector3:

                    RetrievedValue = ThisProperty.vector3Value;

                    break;
                case SerializedPropertyType.Vector4:

                    RetrievedValue = ThisProperty.vector4Value;

                    break;
                case SerializedPropertyType.Rect:

                    RetrievedValue = ThisProperty.rectValue;

                    break;
                case SerializedPropertyType.AnimationCurve:

                    RetrievedValue = ThisProperty.animationCurveValue;

                    break;
                case SerializedPropertyType.Bounds:

                    RetrievedValue = ThisProperty.boundsValue;

                    break;
                case SerializedPropertyType.Quaternion:

                    RetrievedValue = ThisProperty.quaternionValue;

                    break;
                case SerializedPropertyType.Vector2Int:

                    RetrievedValue = ThisProperty.vector2IntValue;

                    break;
                case SerializedPropertyType.Vector3Int:
                    
                    RetrievedValue = ThisProperty.vector3IntValue;

                    break;
                case SerializedPropertyType.RectInt:

                    RetrievedValue = ThisProperty.rectIntValue;

                    break;
                case SerializedPropertyType.BoundsInt:

                    RetrievedValue = ThisProperty.boundsIntValue;

                    break;

                default:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Retrieves the instance that this property belongs to
        /// </summary>
        /// <param name="thisProperty">The property to get the object instance that 
        /// owns the data this property represents</param>
        /// <returns>The instance of the object the property is representing</returns>
        public static System.Object GetPropertyInstanceObject(this SerializedProperty thisProperty)
        {
            string[] path = thisProperty.propertyPath.Split('.');
            object propertyObject = thisProperty.serializedObject.targetObject;

            foreach (string pathNode in path)
            {
                FieldInfo fieldInfo = propertyObject.GetType().GetField(pathNode, 
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                propertyObject = fieldInfo.GetValue(propertyObject);
            }

            return propertyObject;
        }

        /// <summary>
        /// Retrieves the instance that this property belongs to
        /// </summary>
        /// <param name="thisProperty">The property to get the object instance that 
        /// owns the data this property represents</param>
        /// <returns>The instance of the object the property is representing</returns>
        public static T GetPropertyInstanceObject<T>(this SerializedProperty thisProperty)
        {
            object instanceItem = thisProperty.GetPropertyInstanceObject();

            return (T)instanceItem;
        }
    }
}
