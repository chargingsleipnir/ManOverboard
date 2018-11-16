using System;
using UnityEngine;

namespace ZeroProgress.SceneManagementUtility
{
    /// <summary>
    /// Simple scene variable setter as a response to Unity Events
    /// </summary>
    /// <typeparam name="T">The type to set</typeparam>
    public class SceneVariableSetter<T> : MonoBehaviour
    {
        [Tooltip("The controller to set the variable for")]
        public SceneManagerController SceneController;

        [Tooltip("The name of the variable to be set")]
        public string VariableName;

        /// <summary>
        /// Sets the variable of the specified name
        /// </summary>
        /// <param name="value">The value to be set</param>
        public virtual void SetVariable(T value)
        {
            if (SceneController == null)
                throw new ArgumentNullException("Scene Controller null, cannot set variable");

            SceneController.SceneVariables.SetValue(VariableName, value);
        }
    }
}