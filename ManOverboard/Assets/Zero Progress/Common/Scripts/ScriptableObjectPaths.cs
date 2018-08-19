namespace ZeroProgress.Common
{
    /// <summary>
    /// Container for asset menu pathing when defining asset menus for scriptable objects
    /// </summary>
    public static class ScriptableObjectPaths
    {
        /// <summary>
        /// Base path for all ZeroProgress related paths
        /// </summary>
        public const string ZERO_PROGRESS_COMMON_PATH = "Zero Progress/";

        /// <summary>
        /// Path for assets that are related to collections
        /// </summary>
        public const string ZERO_PROGRESS_COLLECTIONS_PATH = ZERO_PROGRESS_COMMON_PATH + "Collections/";

        /// <summary>
        /// Path for assets that are related to scriptable events
        /// </summary>
        public const string ZERO_PROGRESS_EVENTS_PATH = ZERO_PROGRESS_COMMON_PATH + "Events/";

        /// <summary>
        /// Path for assets that are related to scriptable variables
        /// </summary>
        public const string ZERO_PROGRESS_PRIMITIVES_PATH = ZERO_PROGRESS_COMMON_PATH + "Primitives/";

        /// <summary>
        /// Path for assets that are related to filtering
        /// </summary>
        public const string ZERO_PROGRESS_FILTERS_PATH = ZERO_PROGRESS_COMMON_PATH + "Filters/";
    }
}
