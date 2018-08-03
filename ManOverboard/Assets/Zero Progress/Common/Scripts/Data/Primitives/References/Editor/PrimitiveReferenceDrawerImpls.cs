using UnityEditor;
using UnityEngine;

namespace ZeroProgress.Common.Editors
{
    [CustomPropertyDrawer(typeof(FloatReference))]
    public class FloatReferenceDrawer : PrimitiveReferenceDrawer<float, ScriptableFloat>
    {
    }

    [CustomPropertyDrawer(typeof(IntReference))]
    public class IntReferenceDrawer : PrimitiveReferenceDrawer<int, ScriptableInt>
    {
    }

    [CustomPropertyDrawer(typeof(StringReference))]
    public class StringReferenceDrawer : PrimitiveReferenceDrawer<string, ScriptableString>
    {
    }

    [CustomPropertyDrawer(typeof(Vector3Reference))]
    public class Vector3ReferenceDrawer : PrimitiveReferenceDrawer<Vector3, ScriptableVector3>
    {
    }
}