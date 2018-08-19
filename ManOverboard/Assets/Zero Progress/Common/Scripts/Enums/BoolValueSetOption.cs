namespace ZeroProgress.Common
{
    /// <summary>
    /// The possible options for how a boolean should be set
    /// </summary>
    public enum BoolValueSetOption
    {
        /// <summary>
        /// Not specified
        /// </summary>
        NOT_SET,
        /// <summary>
        /// Set the boolean to True
        /// </summary>
        TRUE,
        /// <summary>
        /// Set the boolean to False
        /// </summary>
        FALSE,
        /// <summary>
        /// Don't set the boolean at all
        /// </summary>
        UNCHANGED,
        /// <summary>
        /// Toggle the boolean value
        /// </summary>
        INVERSE
    }

    /// <summary>
    /// Helper class for translating the BoolValueSetOption into a boolean value
    /// </summary>
    public static class BoolValueSetOptionExtensions
    {
        /// <summary>
        /// Gets the boolean value related to the specified SetOption
        /// </summary>
        /// <param name="SetOption">How to modify the current boolean value</param>
        /// <param name="CurrentValue">The current bool value</param>
        /// <returns>The new boolean value after 'conversion'</returns>
        public static bool GetBoolValue(this BoolValueSetOption SetOption, bool CurrentValue)
        {
            switch (SetOption)
            {
                case BoolValueSetOption.TRUE:
                    return true;
                case BoolValueSetOption.FALSE:
                    return false;
                case BoolValueSetOption.UNCHANGED:
                    return CurrentValue;
                case BoolValueSetOption.INVERSE:
                    return !CurrentValue;
                case BoolValueSetOption.NOT_SET:
                default:
                    throw new System.ArgumentException("Set Option out of bounds, cannot get bool value");
            }
        }
    }
}
