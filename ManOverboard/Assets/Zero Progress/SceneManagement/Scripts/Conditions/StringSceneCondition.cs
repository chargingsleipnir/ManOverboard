using System;
using UnityEngine;

namespace ZeroProgress.SceneManagementUtility
{
    /// <summary>
    /// Condition to be applied against a single string
    /// </summary>
    public class StringSceneCondition : SingleVarSceneCondition
    {
        [Tooltip("The value to compare against")]
        public string DesiredValue = string.Empty;

        [Tooltip("How the values should be compared")]
        public StringComparison CompareMode = StringComparison.OrdinalIgnoreCase;

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

        /// <summary>
        /// Determines if the provided value is an integer and matches
        /// the comparison rules
        /// </summary>
        /// <param name="variableValue">The value of the variable to evaluate</param>
        /// <returns>True if met, false if not</returns>
        protected override bool IsConditionMet(object variableValue)
        {
            string varVal = variableValue as String;

            if (varVal == null)
                return false;

            return varVal.Equals(DesiredValue, CompareMode);
        }
    }
}