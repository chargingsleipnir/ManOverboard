using UnityEditor;
using ZeroProgress.Common.Reflection;

namespace ZeroProgress.Common.Editors
{
    /// <summary>
    /// Extension methods for the Editor class
    /// </summary>
    public static class EditorExtensions {

        /// <summary>
        /// Draws the inspector excluding the provided properties
        /// </summary>
        /// <param name="thisEditor">The editor to handle the drawing</param>
        /// <param name="serializedObject">The serialized object containing
        /// the required information</param>
        /// <param name="exclusions">Names of all of the properties to be excluded</param>
        public static void DrawExcluding(this Editor thisEditor, 
            SerializedObject serializedObject, params string[] exclusions)
        {
            ReflectionUtilities.InvokeMethod(thisEditor, "DrawPropertiesExcluding",
                System.Reflection.BindingFlags.Static | 
                System.Reflection.BindingFlags.FlattenHierarchy |
                System.Reflection.BindingFlags.NonPublic,
                serializedObject, exclusions);
        }

        /// <summary>
        /// Draws the inspector excluding the script field that appears
        /// at the top for every editor
        /// </summary>
        /// <param name="thisEditor">The editor to handle the drawing</param>
        /// <param name="serializedObject">The serialized object containing
        /// the required information</param>
        public static void DrawExcludingScript(this Editor thisEditor,
            SerializedObject serializedObject)
        {
            thisEditor.DrawExcluding(serializedObject, "m_Script");
        }
    }
}