namespace ZeroProgress.Common
{
    /// <summary>
    /// Options for applying comparisons between numerals
    /// </summary>
    public enum NumeralComparer
    {
        NOT_SET,
        LESS_THAN,
        LESS_THAN_OR_EQUAL,
        EQUAL,
        NOT_EQUAL,
        GREATER_THAN_OR_EQUAL,
        GREATER_THAN
    }

    /// <summary>
    /// Extension methods for the numeral comparer extensions
    /// </summary>
    public static class NumeralComparerExtensions
    {
        /// <summary>
        /// Compares two integers using the enumeration value
        /// </summary>
        /// <param name="thisComparer">The comparer to use</param>
        /// <param name="value">The value to compare</param>
        /// <param name="compareValue">The value to compare against</param>
        /// <returns>True if passed or if NOT_SET is the enum value</returns>
        public static bool EvaluateComparison(this NumeralComparer thisComparer, 
            int value, int compareValue)
        {
            switch (thisComparer)
            {
                case NumeralComparer.LESS_THAN:
                    return value < compareValue;
                case NumeralComparer.LESS_THAN_OR_EQUAL:
                    return value <= compareValue;
                case NumeralComparer.EQUAL:
                    return value == compareValue;
                case NumeralComparer.NOT_EQUAL:
                    return value != compareValue;
                case NumeralComparer.GREATER_THAN_OR_EQUAL:
                    return value >= compareValue;
                case NumeralComparer.GREATER_THAN:
                    return value > compareValue;
                case NumeralComparer.NOT_SET:
                default:
                    return true;
            }
        }

        /// <summary>
        /// Compares two floats using the enumeration value
        /// </summary>
        /// <param name="thisComparer">The comparer to use</param>
        /// <param name="value">The value to compare</param>
        /// <param name="compareValue">The value to compare against</param>
        /// <returns>True if passed or if NOT_SET is the enum value</returns>
        public static bool EvaluateComparison(this NumeralComparer thisComparer,
            float value, float compareValue)
        {
            switch (thisComparer)
            {
                case NumeralComparer.LESS_THAN:
                    return value < compareValue;
                case NumeralComparer.LESS_THAN_OR_EQUAL:
                    return value <= compareValue;
                case NumeralComparer.EQUAL:
                    return value == compareValue;
                case NumeralComparer.NOT_EQUAL:
                    return value != compareValue;
                case NumeralComparer.GREATER_THAN_OR_EQUAL:
                    return value >= compareValue;
                case NumeralComparer.GREATER_THAN:
                    return value > compareValue;
                case NumeralComparer.NOT_SET:
                default:
                    return true;
            }
        }
    }
}
