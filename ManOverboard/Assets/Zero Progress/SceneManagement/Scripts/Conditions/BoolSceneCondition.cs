using UnityEngine;

namespace ZeroProgress.SceneManagementUtility
{
    /// <summary>
    /// A condition that checks if the scene variable value
    /// is comparable to a desired boolean value
    /// </summary>
    public sealed class BoolSceneCondition : SingleVarSceneCondition
    {
        [Tooltip("The value that the boolean should be")]
        public bool DesiredValue = true;

        /// <summary>
        /// Evaluates the value of the variable to determine if the condition is met
        /// </summary>
        /// <param name="variableValue">The value of the variable. If IgnoreNullValues returns
        /// true, this will never be null</param>
        /// <returns>True if the condition is met, false if not</returns>
        protected override bool IsConditionMet(System.Object variableValue)
        {
            if (!(variableValue is bool))
            {
                Debug.LogError("Value cannot be cast to bool");
                return false;
            }

            return (bool)variableValue == DesiredValue;
        }

        /// <summary>
        /// Used to determine whether or not to skip variable evaluation if the
        /// value is null
        /// </summary>
        /// <returns>True to skip EvaluateVariable call if the value is null, false
        /// to pass the null to evaluation</returns>
        protected override bool IgnoreNullValues()
        {
            return true;
        }
    }
}