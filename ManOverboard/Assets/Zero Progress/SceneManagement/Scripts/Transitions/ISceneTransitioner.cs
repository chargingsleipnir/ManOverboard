using ZeroProgress.Common;

namespace ZeroProgress.SceneManagementUtility
{
    /// <summary>
    /// Interface for any objects that provide the functionality to transition from
    /// one scene to another
    /// </summary>
    public interface ISceneTransitioner
    {
        /// <summary>
        /// Event for when a transition has started
        /// </summary>
        SceneTransitionEvent OnTransitionStarted { get; set; }

        /// <summary>
        /// Event for when the scene has finished changing
        /// </summary>
        SceneTransitionEvent OnTransitionCompleted { get; set; }

        /// <summary>
        /// Event for updating how far along the loading is
        /// </summary>
        UnityFloatEvent OnLoadProgressChanged { get; set; }
        
        /// <summary>
        /// Performs the transition from the current scene to the destination scene
        /// </summary>
        /// <param name="sceneManager">The Scene Manager that drove the transition</param>
        /// <param name="current">The scene that is currently active</param>
        /// <param name="desired">The scene to transition to</param>
        /// <returns>True if transition started, false if not</returns>
        bool Transition(SceneManagerController sceneManager,
            SceneModel current, SceneModel desired);
    }
}