using UnityEditor;
using ZeroProgress.Common.Editors;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Implementation of the property drawer for the String Filter reference
    /// </summary>
    [CustomPropertyDrawer(typeof(StringFilterRef))]
    public class StringFilterReferenceDrawer : PrimitiveReferenceDrawer<StringFilter, ScriptableStringFilter>
    {
    }

    /// <summary>
    /// Implementation of the property drawer for the GameObjectTagFilter reference
    /// </summary>
    [CustomPropertyDrawer(typeof(GameObjectTagFilterRef))]
    public class GameObjectTagFilterReferenceDrawer : PrimitiveReferenceDrawer<GameObjectTagFilter, ScriptableGameObjectTagFilter>
    {
    }

    /// <summary>
    /// Implementation of the property drawer for the Type Filter reference
    /// </summary>
    [CustomPropertyDrawer(typeof(TypeFilterRef))]
    public class TypeFilterReferenceDrawer : PrimitiveReferenceDrawer<TypeFilter, ScriptableTypeFilter>
    {
    }

    /// <summary>
    /// Implementation of the property drawer for the GameObjectComponentFilter reference
    /// </summary>
    [CustomPropertyDrawer(typeof(GameObjectComponentFilterRef))]
    public class GameObjectComponentReferenceDrawer : PrimitiveReferenceDrawer<GameObjectComponentFilter, ScriptableGameObjectComponentFilter>
    {
    }
}
