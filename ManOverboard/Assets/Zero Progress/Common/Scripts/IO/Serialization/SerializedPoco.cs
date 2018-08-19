using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using ZeroProgress.Common.Reflection;

namespace ZeroProgress.Common
{
    /// <summary>
    /// A container to serialize poco classes
    /// </summary>
    [Serializable]
    public class SerializedPoco : ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector]
        private string pocoJsonSerialized = null;

        [SerializeField, HideInInspector]
        private List<UnityEngine.Object> unityObjects = null;

        /// <summary>
        /// The item to be serialized
        /// </summary>
        private System.Object item;
        
        public System.Object Value
        {
            get { return item; }
        }


        /// <summary>
        /// Retrieves the item as the specified type
        /// </summary>
        /// <typeparam name="T">The type to retrieve</typeparam>
        /// <returns>The value as the desired type, or null if
        /// null or couldn't cast</returns>
        public T ValueAs<T>() where T:class
        {
            if (item == null)
                return null;

            return item as T;
        }
        
        /// <summary>
        /// Sets the contained item with the desired value
        /// while also checking to ensure it's a valid type
        /// </summary>
        /// <param name="newValue">The new value to assign</param>
        /// <exception cref="ArgumentException">Thrown if the newValue is
        /// not an accepted type</exception>
        public void SetValue(System.Object newValue)
        {
            if(newValue == null)
            {
                item = newValue;
                return;
            }
            
            Type valueType = newValue.GetType();

            if (valueType.IsSubclassOf(typeof(UnityEngine.Object)))
                throw new ArgumentException("Value cannot be a unity item");
            
            item = newValue;
        }

        public void OnBeforeSerialize()
        {
            pocoJsonSerialized = SerializePoco();
        }
        
        /// <summary>
        /// Serializes the details about the poco to work with the unity serializer
        /// </summary>
        /// <param name="includeType">True to include type information in the serialization</param>
        /// <returns>The text representing the serialized information</returns>
        private string SerializePoco(bool includeType = true)
        {
            if (item == null)
                return null;

            Dictionary<string, System.Object> fieldMapping = new Dictionary<string, object>();

            unityObjects = ReflectionUtilities.FindAllUnityObjects(item).ToList();

            IEnumerable<FieldInfo> fields = ReflectionUtilities.FindAllUnitySerializableFields(item);

            foreach (var field in fields)
            {
                SerializeField(field, fieldMapping, unityObjects);
            }

            if (includeType)
            {
                string typeRep = item.GetType().AssemblyQualifiedName;
                return typeRep + "\n" + MiniJson.Serialize(fieldMapping);
            }
            else
                return MiniJson.Serialize(fieldMapping);
        }

        /// <summary>
        /// Extracts the information about the field to be stored in the dictionary for serialization
        /// </summary>
        /// <param name="field">The field to be saved</param>
        /// <param name="fieldMapping">The mapping of field names to objects</param>
        /// <param name="unityObjectList">The collection of unity objects to be serialized</param>
        private void SerializeField(FieldInfo field, 
            Dictionary<string, System.Object> fieldMapping, List<UnityEngine.Object> unityObjectList)
        {
            System.Object fieldValue = field.GetValue(item);

            if (field.FieldType.IsSubclassOf(typeof(UnityEngine.Object)))
            {
                UnityEngine.Object fieldValueAsUnityObj = fieldValue as UnityEngine.Object;

                if (fieldValueAsUnityObj != null)
                {
                    int index = unityObjectList.FindIndex(x => x == fieldValueAsUnityObj);
                    fieldMapping.Add(field.Name, index);
                }
            }
            else
            {
                if (fieldValue != null)
                    fieldMapping.Add(field.Name, fieldValue);
            }
        }

        public void OnAfterDeserialize()
        {
            if (string.IsNullOrEmpty(pocoJsonSerialized))
                return;
            
            Type itemType = ExtractType(ref pocoJsonSerialized);

            if (itemType == null)
                return;

            item = Activator.CreateInstance(itemType);

            DeserializePoco(item, pocoJsonSerialized);
        }

        /// <summary>
        /// Deserializes the POCO class
        /// </summary>
        /// <param name="objectToPopulate">The object to deserialize the fields to</param>
        /// <param name="json">The json to be deserialized</param>
        private void DeserializePoco(System.Object objectToPopulate, string json)
        {
            if (string.IsNullOrEmpty(json))
                return;

            Dictionary<string, object> values = MiniJson.Deserialize(json) as Dictionary<string, object>;

            if (values == null)
                return;

            IEnumerable<FieldInfo> fields = ReflectionUtilities.FindAllUnitySerializableFields(item);

            foreach (var field in fields)
            {
                DeserializeField(field, objectToPopulate, values);
            }

            // Null it as there is no point taking up runtime memory
            unityObjects = null;
            pocoJsonSerialized = null;
        }

        /// <summary>
        /// Helper to pull the type out of the json string
        /// </summary>
        /// <param name="jsonString">The json string to extract the type out of. Taken by 
        /// reference so that the text representing the type can be removed after parsing</param>
        /// <returns>The type that indicated the items type</returns>
        private Type ExtractType(ref string jsonString)
        {
            int endOfLineIndex = pocoJsonSerialized.IndexOf("\n");

            if (endOfLineIndex < 0)
            {
                Debug.LogError("Missing type information");
                return null;
            }

            string typeRep = pocoJsonSerialized.Substring(0, endOfLineIndex);

            Type type = Type.GetType(typeRep);

            if (type == null)
            {
                Debug.LogError("Type could not be resolved");
                return null;
            }

            if(type.IsSubclassOf(typeof(UnityEngine.Object)))
            {
                Debug.LogError("Invalid type was serialized somehow");
                return null;
            }

            pocoJsonSerialized = pocoJsonSerialized.Remove(0, typeRep.Length);

            return type;
        }

        /// <summary>
        /// Helper to manage the deserialization of the field
        /// </summary>
        /// <param name="field">The field to be deserialized</param>
        /// <param name="fieldOwner">The owner of the field to be deserialized</param>
        /// <param name="serializedValues">The collection of values to be deserialized</param>
        private void DeserializeField(FieldInfo field, System.Object fieldOwner,
            Dictionary<string, System.Object> serializedValues)
        {
            try
            {
                System.Object fieldValue;

                if (!serializedValues.TryGetValue(field.Name, out fieldValue))
                    return;

                if (fieldValue == null)
                    return;
                
                if (field.FieldType.IsSubclassOf(typeof(UnityEngine.Object)))
                {
                    int fieldIndex = Convert.ToInt32(fieldValue);

                    if (fieldIndex < 0 || fieldIndex >= unityObjects.Count)
                        return;

                    field.SetValue(fieldOwner, unityObjects[fieldIndex] as UnityEngine.Object);
                }
                else
                {
                    field.SetValue(fieldOwner, fieldValue);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}