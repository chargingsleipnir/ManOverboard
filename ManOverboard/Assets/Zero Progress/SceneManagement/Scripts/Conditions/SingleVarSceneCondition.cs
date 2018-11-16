using UnityEngine;

namespace ZeroProgress.SceneManagementUtility
{
    /// <summary>
    /// Base scene condition that stores the name of a variable to be evaluated
    /// </summary>
    public abstract class SingleVarSceneCondition : SceneCondition
    {
        [SerializeField]
        public string VariableName;
        
        /// <summary>
        /// Determines if this condition is met
        /// </summary>
        /// <param name="param">The scene manager which contains all the variables
        /// we may need to evaluate</param>
        /// <returns>True if met, false if not</returns>
        public sealed override bool IsMet(SceneManagerController param)
        {
            if (string.IsNullOrEmpty(VariableName))
            {
                if (Application.isPlaying)
                    Debug.LogError("No variable identifier provided, cannot evaluate condition");

                return false;
            }

            System.Object varVal = param.SceneVariables.GetObjectValue(VariableName);

            if (varVal == null && IgnoreNullValues())
                return false;

            return IsConditionMet(varVal);
        }

        /// <summary>
        /// Evaluates the value of the variable to determine if the condition is met
        /// </summary>
        /// <param name="variableValue">The value of the variable. If IgnoreNullValues returns
        /// true, this will never be null</param>
        /// <returns>True if the condition is met, false if not</returns>
        protected abstract bool IsConditionMet(System.Object variableValue);

        /// <summary>
        /// Used to determine whether or not to skip variable evaluation if the
        /// value is null
        /// </summary>
        /// <returns>True to skip EvaluateVariable call if the value is null, false
        /// to pass the null to evaluation</returns>
        protected abstract bool IgnoreNullValues();
    }
}