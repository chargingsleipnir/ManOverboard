using UnityEditor;

namespace ZeroProgress.Common.Editors
{
    /// <summary>
    /// Helper for managing timing in an editor context
    /// </summary>
    public class EditorTiming
    {
        /// <summary>
        /// The time of the last 'frame'
        /// </summary>
        private static double lastCacheTime = EditorApplication.timeSinceStartup;

        /// <summary>
        /// The last calculated delta time
        /// </summary>
        private static float deltaTime;
        
        /// <summary>
        /// Static constructor for registering to the update event
        /// </summary>
        static EditorTiming()
        {
            Update();
            EditorApplication.update -= Update;
            EditorApplication.update += Update;
        }

        /// <summary>
        /// Update the cached time with the current time
        /// </summary>
        private static void Update()
        {
            deltaTime = (float)(EditorApplication.timeSinceStartup - lastCacheTime);

            lastCacheTime = EditorApplication.timeSinceStartup;
        }

        /// <summary>
        /// Retrieves the difference in time between the last recorded time
        /// and the current recorded time
        /// </summary>
        /// <returns>The delta time</returns>
        public static float GetDeltaTime()
        {
            return deltaTime;
        }
        
    }
}
