using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ZeroProgress.SceneManagementUtility
{
    /// <summary>
    /// Click response to load a scene through the provided scene manager
    /// </summary>
    public class SceneLoadClick : MonoBehaviour
    {
        [Tooltip("The scene manager to load the scene through")]
        public SceneManagerController SceneManager;

        [Tooltip("The name of the scene to be loaded")]
        public string SceneName;

        [Tooltip("True to require that the destination scene is currently unlocked, false " +
            "to transition no matter what")]
        public bool RequireUnlocked = true;

        [Tooltip("True to get the scene name from a text component (such as on a button), false " +
            "to not. If true and text component is null, will use the first text component")]
        public bool GetFromTextComp = true;

        [Tooltip("True for retrieving a disabled text box as a valid text provider")]
        public bool AllowDisabledText = true;

        [Tooltip("The text component to get the scene name from if GetFromTextComp is true. If no " +
            "text component can be found, will fallback to use the SceneName field")]
        public Text TextComponent = null;

        /// <summary>
        /// Retrieves the scene name to be used
        /// </summary>
        /// <returns>The scene name from the field, or from the text component, depending which
        /// should be used</returns>
        public string GetSceneName()
        {
            string sceneName = SceneName;

            if (GetFromTextComp)
            {
                if (TextComponent == null)
                    TextComponent = GetComponentInChildren<Text>(AllowDisabledText);

                if (TextComponent != null)
                    sceneName = TextComponent.text;
            }

            return sceneName;
        }

        /// <summary>
        /// Load the scene identified by the SceneName property
        /// </summary>
        public void LoadScene()
        {
            LoadScene(GetSceneName());
        }

        /// <summary>
        /// Load the scene
        /// </summary>
        /// <param name="sceneName">The name of the scene to load</param>
        public void LoadScene(string sceneName)
        {
            if (SceneManager == null)
            {
                Debug.LogError("Scene manager null, cannot load scene");
                return;
            }

            if (RequireUnlocked)
                SceneManager.TransitionIfUnlocked(sceneName);
            else
                SceneManager.TransitionToScene(sceneName);
        }
    }
}
