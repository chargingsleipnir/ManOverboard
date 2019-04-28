using System.Collections.Generic;
using UnityEngine;

namespace ZeroProgress.SceneManagementUtility
{
    /// <summary>
    /// Base class for functionality to retrieve scenes in the project
    /// </summary>
    public abstract class SceneHunter : ScriptableObject
    {
        /// <summary>
        /// Retrieves the paths to all of the assets that represent scenes of
        /// the current project
        /// </summary>
        /// <returns>Collection of paths representing game scenes</returns>
        public abstract IEnumerable<string> GetScenePaths();
    }
}