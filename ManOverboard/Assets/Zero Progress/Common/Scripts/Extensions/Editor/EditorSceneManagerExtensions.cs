using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace ZeroProgress.Common
{
    public static class EditorSceneManagerExtensions
    {
        /// <summary>
        /// Iterates all scenes and returns all modified ones
        /// </summary>
        /// <returns>All scenes currently marked dirty</returns>
        public static IEnumerable<Scene> GetModifiedScenes()
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene current = SceneManager.GetSceneAt(i);

                if (current.isDirty)
                    yield return current;
            }
        }
    }
}