using UnityEditor;
using UnityEngine;

namespace ZeroProgress.Common.Editors
{
    /// <summary>
    /// Implementation of the property drawer for the float reference
    /// </summary>
    [CustomPropertyDrawer(typeof(ReferenceDropdownAttribute))]
    public class FloatReferenceDrawer : PrimitiveReferenceDrawer<float, ScriptableFloat>
    {
    }

    /// <summary>
    /// Implementation of the property drawer for the int reference
    /// </summary>
    [CustomPropertyDrawer(typeof(ReferenceDropdownAttribute))]
    public class IntReferenceDrawer : PrimitiveReferenceDrawer<int, ScriptableInt>
    {
    }

    /// <summary>
    /// Implementation of the property drawer for the string reference
    /// </summary>
    [CustomPropertyDrawer(typeof(ReferenceDropdownAttribute))]
    public class StringReferenceDrawer : PrimitiveReferenceDrawer<string, ScriptableString>
    {
    }

    /// <summary>
    /// Implementation of the property drawer for the Vector3 reference
    /// </summary>
    [CustomPropertyDrawer(typeof(ReferenceDropdownAttribute))]
    public class Vector3ReferenceDrawer : PrimitiveReferenceDrawer<Vector3, ScriptableVector3>
    {
    }
}