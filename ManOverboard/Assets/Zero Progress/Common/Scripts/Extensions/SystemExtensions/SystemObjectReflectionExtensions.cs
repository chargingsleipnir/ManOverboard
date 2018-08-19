using System.Reflection;

namespace ZeroProgress.Common.Reflection
{
    /// <summary>
    /// Adds reflection utility methods as extensions to the System.Object class
    /// </summary>
    public static class SystemObjectReflectionExtensions
    {
        /// <summary>
        /// Retrieves the value of a field by its name
        /// </summary>
        /// <typeparam name="T">The type of value to be returned</typeparam>
        /// <param name="thisObj">The object to be searched for the field</param>
        /// <param name="fieldName">Name of the value to extract the value from</param>
        /// <param name="bindingFlags">How to go about searching the object</param>
        /// <returns>The value of the field</returns>
        public static T Reflection_GetFieldValue<T>(this object thisObj, 
            string fieldName, BindingFlags bindingFlags = BindingFlags.Public)
        {
            return ReflectionUtilities.GetFieldByName<T>(thisObj, fieldName, bindingFlags);
        }

        /// <summary>
        /// Retrieves the value of a field by its name, without throwing an exception
        /// </summary>
        /// <typeparam name="T">The type of value to be returned</typeparam>
        /// <param name="thisObj">The object to be searched for the field</param>
        /// <param name="fieldName">Name of the value to extract the value from</param>
        /// <param name="bindingFlags">How to go about searching the object</param>
        /// <param name="value">The extracted value</param>
        /// <returns>True if found, false if not. Value will contain the extracted value if true is returned</returns>
        public static bool Reflection_TryGetFieldValue<T>(this object thisObj,
            string fieldName, out T outValue, BindingFlags bindingFlags = BindingFlags.Public)
        {
            return ReflectionUtilities.TryGetFieldByName<T>(thisObj,
                fieldName, bindingFlags, out outValue);
        }

        /// <summary>
        /// Retrieves the value of a field by its name
        /// </summary>
        /// <typeparam name="T">The type of value to be returned</typeparam>
        /// <param name="thisObj">The object to be searched for the field</param>
        /// <param name="fieldName">Name of the value to extract the value from</param>
        /// <param name="bindingFlags">How to go about searching the object</param>
        /// <returns>The value of the field</returns>
        public static object Reflection_GetFieldValue(this object thisObj,
            string fieldName, BindingFlags bindingFlags = BindingFlags.Public)
        {
            return ReflectionUtilities.GetFieldByName(thisObj, fieldName, bindingFlags);
        }

        /// <summary>
        /// Sets the value of a field by its name
        /// </summary>
        /// <param name="obj">The object to be searched for the field</param>
        /// <param name="fieldName">Name of the value to extract the value from</param>
        /// <param name="value">The value to set</param>
        /// <param name="bindingFlags">How to go about searching the object</param>
        /// <returns>The value of the field</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the field under the specified name can't be found</exception>
        public static void Reflection_SetFieldValue(this object thisObj,
            string fieldName, object value, BindingFlags bindingFlags = BindingFlags.Public)
        {
            ReflectionUtilities.SetFieldByName(thisObj, fieldName, bindingFlags, value);
        }

        /// <summary>
        /// Sets the value of a field by its name, without throwing an exception if it isn't found
        /// </summary>
        /// <param name="obj">The object to be searched for the field</param>
        /// <param name="fieldName">Name of the value to extract the value from</param>
        /// <param name="value">The value to set</param>
        /// <param name="bindingFlags">How to go about searching the object</param>
        /// <returns>True if the field was successfully set, false if the field couldn't be found</returns>
        public static bool Reflection_TrySetFieldValue(this object thisObj,
            string fieldName, object value, BindingFlags bindingFlags = BindingFlags.Public)
        {
            return ReflectionUtilities.TrySetFieldByName(thisObj, fieldName, bindingFlags, value);
        }
    }
}