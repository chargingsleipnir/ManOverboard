using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZeroProgress.Common;
using ZeroProgress.Common.Editors;

namespace ZeroProgress.SceneManagementUtility.Editors
{
    public class SerializedSceneVariableCollection
    {
        public SceneVariableContainer SceneVariablesContainer { get; private set; }

        public SerializedObject SerializedContainer { get; private set; }

        public SerializedProperty VariableListProp { get; private set; }

        public SerializedSceneVariableCollection(SceneVariableContainer variableContainer)
        {
            SceneVariablesContainer = variableContainer;

            SerializedContainer = new SerializedObject(variableContainer);

            VariableListProp = SerializedContainer.FindProperty("sceneVariables");
        }

        internal SerializedSceneVariable GetSerializedVariableAt(int index)
        {
            return new SerializedSceneVariable(VariableListProp.GetArrayElementAtIndex(index));
        }

        internal void CreateNewVariable(string name, System.Type typeToCreate)
        {
            if (!CanCreateVariable(name, typeToCreate))
                return;

            ScriptableObject variableType = 
                AssetDatabaseExtensions.CreateSubScriptableObject(SceneVariablesContainer,
                    typeToCreate, name);

            Undo.SetCurrentGroupName("Variable " + name + " creation");
            int undoGroup = Undo.GetCurrentGroup();

            Undo.RegisterCreatedObjectUndo(variableType, "Created " + name);

            Undo.RegisterCompleteObjectUndo(SceneVariablesContainer, "Created " + name);

            SceneVariablesContainer.CreateNewVariable(name, variableType);

            SerializedContainer.Update();

            Undo.CollapseUndoOperations(undoGroup);
        }

        /// <summary>
        /// Determines if a variable can be created with the provided name and type
        /// 
        /// Handles writing error messages to the log
        /// </summary>
        /// <param name="name">The name of the parameter to create</param>
        /// <param name="typeToCreate">The type of the value container</param>
        /// <returns>True if can create, false if not</returns>
        private bool CanCreateVariable(string name, System.Type typeToCreate)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("New variable requires name");
                return false;
            }

            if (SceneVariablesContainer.DoesVariableExist(name))
            {
                Debug.LogError("Cannot create variable, already exists");
                return false;
            }

            if (typeToCreate == null)
                return false;

            if (!typeToCreate.IsOrIsSubclassOf(typeof(ScriptableObject)))
            {
                Debug.LogError("Container type does not inherit from ScriptableObject, " +
                    "cannot be used as a SceneVariable type");
                return false;
            }

            if (typeToCreate.GetInterface(typeof(IVariable).Name) == null)
            {
                Debug.LogError("Container type does not implement the IVariable " +
                    "interface, cannot be used as a SceneVariable type");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Delete the variable with Undo functionality
        /// </summary>
        /// <param name="index">The index of the variable to be deleted</param>
        internal void DeleteVariable(int index)
        {
            SerializedContainer.UpdateIfRequiredOrScript();

            Undo.SetCurrentGroupName("Delete Scene Variable");
            int undoGroup = Undo.GetCurrentGroup();

            Undo.RegisterCompleteObjectUndo(SerializedContainer.targetObject, "Scene Variable Deleted");

            SerializedSceneVariable serializedVariable = GetSerializedVariableAt(index);

            Undo.DestroyObjectImmediate(serializedVariable.ValueInstance);

            // Need to call delete once because it isn't an object reference value
            VariableListProp.DeleteArrayElementAtIndex(index);

            SerializedContainer.ApplyModifiedProperties();

            Undo.CollapseUndoOperations(undoGroup);

        }

    }
}