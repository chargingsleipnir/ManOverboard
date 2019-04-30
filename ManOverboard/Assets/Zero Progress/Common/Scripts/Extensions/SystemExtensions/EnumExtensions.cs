using System.Collections.Generic;

/// <summary>
/// Extensions for enums
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Retrieves all enabled flags on the provided input
    /// assuming the backing data-structure is an int for the enum
    /// </summary>
    /// <param name="input">The input to evaluate</param>
    /// <returns>The collection of flags found to be enabled</returns>
    public static IEnumerable<System.Enum> GetEnabledFlagsInt(this System.Enum input)
    {
        int inputValue = (int)System.Convert.ChangeType(input, typeof(int));

        foreach (System.Enum value in System.Enum.GetValues(input.GetType()))
        {
            int enumValue = (int)System.Convert.ChangeType(value, typeof(int));

            // Ignore 0 values
            if (enumValue == 0)
                continue;

            if ((inputValue & enumValue) == enumValue)
                yield return value;
        }
    }
}
