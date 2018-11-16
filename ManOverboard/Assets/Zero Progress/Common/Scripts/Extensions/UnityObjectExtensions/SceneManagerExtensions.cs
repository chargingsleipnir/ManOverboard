using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

namespace ZeroProgress.Common
{
    public static class SceneManagerExtensions
    {
        /// <summary>
        /// Get all currently loaded scenes
        /// </summary>
        /// <returns>Collection of all currently loaded scenes</returns>
        public static IEnumerable<Scene> GetLoadedScenes()
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene current = SceneManager.GetSceneAt(i);

                yield return current;
            }
        }

        /// <summary>
        /// Checks if the scene at the provided path (relative to the Assets folder)
        /// is already loaded
        /// </summary>
        /// <param name="path">The path relative to the Assets folder</param>
        /// <returns>True if the scene is currently loaded, false if not</returns>
        public static bool IsSceneLoadedByPath(string path)
        {
            return GetLoadedScenes().Select((x) => x.path).Contains(path);
        }

    }
}