namespace ZeroProgress.Common
{
    /// <summary>
    /// Acts as a primary indexer for the root (within the ZeroProgress submenu)
    /// 
    /// Menu items are sorted lowest priority to highest, even within submenus. Ties sort in calling order. 
    /// If your item has a priority of 11 or more than the previous item, Unity will create a separator 
    /// before your item. Custom menu items with no specified priority are given a default priority of 1000.

    /// Sort priority for custom submenu groups themselves(as opposed to the menu item) is a bit tricky.
    /// A submenu group will be sorted at whatever priority the custom menu item had when it was first 
    /// created.So if you create a new custom menu item(Item A) with priority 900 within a custom 
    /// submenu(within the Window menu), that submenu will appear at 900 in the Window menu and so will 
    /// the first item in the submenu.An item inside that submenu with 911 priority will sort in the 
    /// submenu after Item A, with a separator.

    /// As a quick aside, if you change a menu item’s existing sort priority, you may not see it reflected 
    /// in the Menus. I think Unity is caching the value, so to get it to update you can either restart your Editor, 
    /// or do some gymnastics where you first remove the Priority from your attribute, compile, then add the new priority.
    ///
    /// Notes taken from https://blog.redbluegames.com/guide-to-extending-unity-editors-menus-b2de47a746db
    /// </summary>
    public enum RootIndexing
    {
        PackageExportTemplate = -100,
        SceneManager = -99,
        Collections = 100,
        Events = 101,
        Filters = 102,
        Primitives = 103
    }

    /// <summary>
    /// Enum to provide a sort order for the scriptable collections
    /// </summary>
    public enum ScriptableCollectionsMenuIndexing
    {
        ComponentSet = RootIndexing.Collections,
        GameObjectSet,
        StringSet
    }

    /// <summary>
    /// Enum to provide a sort order for the scriptable events
    /// </summary>
    public enum ScriptableEventsMenuIndexing
    {
        VoidEvent = RootIndexing.Events,
        ComponentEvent,
        FloatEvent,
        GameObjectEvent,
        IntEvent,
        StringEvent,
        Vector2Event,
        Vector3Event
    }

    /// <summary>
    /// Enum to provide a sort order for the scriptable filters
    /// </summary>
    public enum ScriptableFiltersMenuIndexing
    {
        ComponentFilter = RootIndexing.Filters,
        GameObjectFilter,
        TagFilter,
        StringFilter,
        TypeFilter,
        AssetFilter = TypeFilter + 20,
    }

    /// <summary>
    /// Enum to provide a sort order for the scriptable primitives
    /// </summary>
    public enum ScriptableVariablesMenuIndexing
    {
        AnimParam = RootIndexing.Primitives,
        BoolParam,
        FloatParam,
        IntParam,
        StringParam,
        Vector2Param,
        Vector3Param
    }
}