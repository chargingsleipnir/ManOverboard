using System;
using UnityEngine.Events;

namespace ZeroProgress.SceneManagementUtility
{
    /// <summary>
    /// Event args for when a scene transition is taking place
    /// </summary>
    public class SceneTransitionEventArgs
    {
        /// <summary>
        /// The controller that directed the transition
        /// </summary>
        public SceneManagerController SceneManager;

        /// <summary>
        /// The scene the transition was started on
        /// </summary>
        public SceneModel InitialScene;

        /// <summary>
        /// The scene to be loaded
        /// </summary>
        public SceneModel DestinationScene;

        /// <summary>
        /// Default constructor
        /// </summary>
        public SceneTransitionEventArgs()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sceneManager">The controller that directed the transition</param>
        /// <param name="initialScene">The scene the transition was started on</param>
        /// <param name="destScene">The scene to be loaded</param>
        public SceneTransitionEventArgs(SceneManagerController sceneManager,
            SceneModel initialScene, SceneModel destScene)
        {
            SceneManager = sceneManager;
            InitialScene = initialScene;
            DestinationScene = destScene;
        }
    }

    /// <summary>
    /// Unity event for Scene Transition responses
    /// </summary>
    [Serializable]
    public class SceneTransitionEvent : UnityEvent<SceneTransitionEventArgs>
    {
    }
}