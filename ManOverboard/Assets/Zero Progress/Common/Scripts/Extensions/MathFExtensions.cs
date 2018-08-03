using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Custom Math/Number related functions
    /// </summary>
    public static class MathFExtensions
    {
        /// <summary>
        /// An approximation function that allows the specifying of a custom Epsilon value
        /// </summary>
        /// <param name="Value">The value to check</param>
        /// <param name="CompareValue">The value to check against</param>
        /// <param name="Epsilon">The epsilon for comparison</param>
        /// <returns>True if value is within epsilon to the compare value, false if not</returns>
        public static bool Approximately(float Value, float CompareValue, float Epsilon = 0.001f)
        {
            return Mathf.Abs(Value - CompareValue) <= Epsilon;
        }

        /// <summary>
        /// Helper to check if a value is close to zero
        /// </summary>
        /// <param name="Value">The value to check</param>
        /// <param name="Epsilon">The epsilon for comparison</param>
        /// <returns>True if the value is within epsilon of zero, false if not</returns>
        public static bool NearZero(float Value, float Epsilon = 0.001f)
        {
            return Approximately(Value, 0f, Epsilon);
        }

        /// <summary>
        /// Rounds a value to the specified value if it's within epsilon
        /// </summary>
        /// <param name="Value">The value to check</param>
        /// <param name="DesiredValue">The value to round towards</param>
        /// <param name="Epsilon">The epsilon for comparison</param>
        /// <returns>The DesiredValue if Value is within epsilon to it, otherwise the Value unchanged</returns>
        public static float RoundTo(float Value, float DesiredValue, float Epsilon = 0.001f)
        {
            if (Approximately(Value, DesiredValue, Epsilon))
                return DesiredValue;
            else
                return Value;
        }

        /// <summary>
        /// Rounds the values of the vector to the desired value if it's within epsilon
        /// </summary>
        /// <param name="Values">The vector values to round</param>
        /// <param name="DesiredValue">The value to round towards</param>
        /// <param name="Epsilon">The epsilon for comparison</param>
        /// <returns>The vector with each component having RoundTo applied</returns>
        public static Vector3 RoundValuesTo(Vector3 Values, float DesiredValue, float Epsilon = 0.001f)
        {
            for (int i = 0; i < 3; i++)
            {
                Values[i] = RoundTo(Values[i], DesiredValue, Epsilon);               
            }

            return Values;
        }
    }
}