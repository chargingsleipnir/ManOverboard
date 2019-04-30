using System;
using UnityEngine;
using ZeroProgress.Common;

namespace ZeroProgress.SceneManagementUtility
{
    /// <summary>
    /// Condition to be applied against a single float
    /// </summary>
    public class FloatSceneCondition : SingleVarSceneCondition
    {
        [Tooltip("The value to compare against")]
        public float DesiredValue = 0f;

        [Tooltip("How the values should be compared")]
        public NumeralComparer CompareMode = NumeralComparer.GREATER_THAN;

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
            try
            {
                float value = Convert.ToSingle(variableValue);
                return CompareMode.EvaluateComparison(value, DesiredValue);
            }
            catch (Exception)
            {
                Debug.LogError("Failed to cast to float");
                return false;
            }
        }
    }
}