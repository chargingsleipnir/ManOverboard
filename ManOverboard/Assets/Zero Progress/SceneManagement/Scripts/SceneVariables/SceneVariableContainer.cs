using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZeroProgress.Common;

namespace ZeroProgress.SceneManagementUtility
{
    /// <summary>
    /// Container of the scene variables for a scene manager
    /// </summary>
    public class SceneVariableContainer : ScriptableObject
    {
        /// <summary>
        /// The collection of scene variables stored in this container
        /// </summary>
        [SerializeField]
        internal List<SceneVariable> sceneVariables =
            new List<SceneVariable>();

        /// <summary>
        /// Event for when a variable has changed
        /// </summary>
        public event EventHandler<EventArgs<string>> OnVariableChanged;

        /// <summary>
        /// Checks if a SceneVariable of the provided name exists or not
        /// </summary>
        /// <param name="name">The name to query for</param>
        /// <returns>True if it exists, false if not</returns>
        public bool DoesVariableExist(string name)
        {
            return GetVariable(name) != null;
        }

        /// <summary>
        /// Retrieves the value of the variable as a System.Object
        /// </summary>
        /// <param name="name">The name of the variable to retrieve</param>
        /// <returns>The value of the variable</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the variable isn't found</exception>
        public System.Object GetObjectValue(string name)
        {
            SceneVariable foundVariable = GetVariable(name);

            if (foundVariable == null)
                throw new KeyNotFoundException("Variable " + name + "doesn't exist");

            return foundVariable.GetValue();
        }

        /// <summary>
        /// Retrieves the value of the variable as a boolean
        /// </summary>
        /// <param name="name">The name of the variable to retrieve</param>
        /// <returns>The value of the variable</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the variable isn't found</exception>
        /// <exception cref="InvalidCastException">Thrown if the variable cannot be cast to desired type</exception>
        public bool GetBoolValue(string name)
        {
            return GetValueAs<bool>(name);
        }

        /// <summary>
        /// Retrieves the value of the variable as an int
        /// </summary>
        /// <param name="name">The name of the variable to retrieve</param>
        /// <returns>The value of the variable</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the variable isn't found</exception>
        /// <exception cref="InvalidCastException">Thrown if the variable cannot be cast to desired type</exception>
        public int GetIntValue(string name)
        {
            return GetValueAs<int>(name);
        }

        /// <summary>
        /// Retrieves the value of the variable as a string
        /// </summary>
        /// <param name="name">The name of the variable to retrieve</param>
        /// <returns>The value of the variable</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the variable isn't found</exception>
        /// <exception cref="InvalidCastException">Thrown if the variable cannot be cast to desired type</exception>
        public string GetStringValue(string name)
        {
            return GetValueAs<string>(name);
        }

        /// <summary>
        /// Retrieves the value of the variable as a float
        /// </summary>
        /// <param name="name">The name of the variable to retrieve</param>
        /// <returns>The value of the variable</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the variable isn't found</exception>
        /// <exception cref="InvalidCastException">Thrown if the variable cannot be cast to desired type</exception>
        public float GetFloatValue(string name)
        {
            return GetValueAs<float>(name);
        }

        /// <summary>
        /// Retrieves the value of the variable as a GameObject
        /// </summary>
        /// <param name="name">The name of the variable to retrieve</param>
        /// <returns>The value of the variable</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the variable isn't found</exception>
        /// <exception cref="InvalidCastException">Thrown if the variable cannot be cast to desired type</exception>
        public GameObject GetGameObjectValue(string name)
        {
            return GetValueAs<GameObject>(name);
        }

        /// <summary>
        /// Sets the value of the specified variable
        /// </summary>
        /// <param name="name">The name of the variable to be set</param>
        /// <param name="newValue">The value to assign to the variable</param>
        public void SetValue(string name, System.Object newValue)
        {
            SceneVariable foundVariable = GetVariable(name);

            if (foundVariable == null)
                throw new KeyNotFoundException("Variable " + name + " doesn't exist");

            System.Object oldValue = foundVariable.GetValue();

            if (oldValue.Equals(newValue))
                return;

            foundVariable.SetValue(newValue);

            OnVariableChanged.SafeInvoke(this, new EventArgs<string>() { Value = name });
        }

        /// <summary>
        /// Creates a new scene variable
        /// </summary>
        /// <param name="name">The name to give the variable</param>
        /// <param name="valueContainer">The container of the value it will store</param>
        public void CreateNewVariable(string name, ScriptableObject valueContainer)
        {
            if (!(valueContainer is IVariable))
                throw new ArgumentException("Provided value container is not an IVariable, cannot create Scene Variable");

            SceneVariable newVar = new SceneVariable()
            {
                Name = name
            };

            newVar.SetContainer(valueContainer);

            AddVariable(newVar, false);
        }

        /// <summary>
        /// Gets the name and underlying type of all variables
        /// </summary>
        /// <returns>Key value pair for each variable detailing the name and
        /// underlying variable type</returns>
        public IEnumerable<KeyValuePair<string, Type>> GetVariableDetails()
        {
            return sceneVariables.Select((x) =>
            {
                System.Object value = x.GetValue();

                Type type = null;

                if (value != null)
                    type = value.GetType();

                return new KeyValuePair<string, Type>(x.Name, type);
            });
        }

        /// <summary>
        /// Resets all variables to their original values
        /// </summary>
        public void ResetVariables()
        {
            foreach (SceneVariable variable in sceneVariables)
            {
                variable.Reset();
            }
        }

        /// <summary>
        /// Callback for when a transition is completed
        /// </summary>
        /// <param name="destScene">The scene that was transitioned to</param>
        public void OnTransitionCompleted(SceneTransitionEventArgs e)
        {
            foreach (SceneVariable variable in sceneVariables)
            {
                if (variable.ResetOnTransition)
                    variable.Reset();
            }
        }
        
        /// <summary>
        /// Adds a variable to the collection
        /// </summary>
        /// <param name="variable">The variable to add</param>
        /// <param name="overwriteIfExist">True to overwrite if a variable of the name already
        /// exists, false to ignore</param>
        internal void AddVariable(SceneVariable variable, bool overwriteIfExist = false)
        {
            if (DoesVariableExist(variable.Name))
            {
                if (overwriteIfExist)
                    sceneVariables.RemoveAll((x) => x.Name == variable.Name);
                else
                    return;
            }

            sceneVariables.Add(variable);
        }

        /// <summary>
        /// Remove the variable of the specified name
        /// </summary>
        /// <param name="name">The name of the variable to remove</param>
        internal void RemoveVariable(string name)
        {
            sceneVariables.RemoveAll((x) => x.Name == name.ToLowerInvariant());
        }

        /// <summary>
        /// Remove the variable
        /// </summary>
        /// <param name="variable">The variable to remove</param>
        internal void RemoveVariable(SceneVariable variable)
        {
            sceneVariables.Remove(variable);
        }

        /// <summary>
        /// Helper to retrieve a variable by name
        /// </summary>
        /// <param name="name">The name of the variable to retrieve</param>
        /// <returns>The found variable, or null</returns>
        internal SceneVariable GetVariable(string name)
        {
            return sceneVariables.Find((x) => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Get the value as the provided type
        /// </summary>
        /// <typeparam name="T">The type to get the value as</typeparam>
        /// <param name="variableName">The name of the variable to get</param>
        /// <returns>The value as the specified type, or default(T) if the
        /// stored value is null</returns>
        /// <exception cref="InvalidCastException">Thrown if the provided type can't be cast</exception>
        private T GetValueAs<T>(string variableName)
        {
            System.Object value = GetObjectValue(variableName);

            if (value == null)
                return default(T);

            return (T)value;
        }
    }
}