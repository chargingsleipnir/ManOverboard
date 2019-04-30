using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Attribute to indicate a reference should use the drop down selector
    /// to determine if straight value or reference value should be used
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ReferenceDropdownAttribute : PropertyAttribute
    {        
    }
}