using System.Collections.Generic;
using UnityEngine;

namespace ZeroProgress.SceneManagementUtility
{
    [System.Serializable]
    public sealed class SceneModel
    {
        [SerializeField]
        private int sceneId;

        public int SceneId
        {
            get { return sceneId; }
            internal set { sceneId = value; }
        }

        [SerializeField,
            Tooltip("Indicates whether or not this scene should be included while iterating")]
        private bool includeInIteration = true;

        public bool IncludeInIteration
        {
            get { return includeInIteration; }
            set { includeInIteration = value; }
        }
        
        [SerializeField]
        private string sceneName;

        public string SceneName
        {
            get { return sceneName; }
            private set { sceneName = value; }
        }

        [SerializeField]
        private string sceneAssetPath;

        public string SceneAssetPath
        {
            get { return sceneAssetPath; }
            private set { sceneAssetPath = value; }
        }

        [SerializeField]
        private bool useAnySceneTransitions = true;

        public bool UseAnySceneTransitions
        {
            get { return useAnySceneTransitions; }
            set { useAnySceneTransitions = value; }
        }
        
        private System.Object sceneStats;
        
        public System.Object SceneStats
        {
            get { return sceneStats; }
            set { sceneStats = value; }
        }
        
        [SerializeField]
        private List<SceneTransition> transitions = new List<SceneTransition>();

        public List<SceneTransition> Transitions
        {
            get { return transitions; }
            private set { transitions = value; }
        }

        [SerializeField]
        private List<SceneCondition> lockConditions = new List<SceneCondition>();

        public List<SceneCondition> LockConditions
        {
            get { return lockConditions; }
            private set { lockConditions = value; }
        }

        /// <summary>
        /// Indicates the current lock status of the level
        /// </summary>
        public bool IsUnlocked { get; set; }

        public SceneModel(int sceneId, string assetPath)
        {
            this.sceneId = sceneId;
            SceneAssetPath = assetPath;
            SceneName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
        }
   
        /// <summary>
        /// Gets the Scene Stats as the specified type
        /// </summary>
        /// <typeparam name="T">The type to retrieve the stats as</typeparam>
        /// <returns>default(T) if the stats are null</returns>
        public T GetSceneStatsAs<T>()
        {
            if (SceneStats == null)
                return default(T);

            return (T)SceneStats;
        }

        /// <summary>
        /// Checks if the level is now unlocked
        /// </summary>
        /// <param name="manager">The manager to check the variables of</param>
        /// <returns>True if unlocked, false if not</returns>
        public bool ReevaluateLockStatus(SceneManagerController manager)
        {
            // Unlocked if no conditions
            bool evaluation = true;

            foreach (SceneCondition lockCondition in LockConditions)
            {
                evaluation = lockCondition.IsMet(manager);

                if (!evaluation)
                    break;
            }

            IsUnlocked = evaluation;
            return evaluation;
        }

        public SceneTransition CreateNewTransition(SceneModel destination)
        {
            return CreateNewTransition(destination.SceneId);
        }

        public SceneTransition CreateNewTransition(int destinationId)
        {
            SceneTransition transition = new SceneTransition();

            transition.destinationSceneId = destinationId;

            this.transitions.Add(transition);

            return transition;
        }

        /// <summary>
        /// Gets all of the unique scenes connected to this scene
        /// 
        /// (Does not get scenes that have transitions where this scene is the destination)
        /// </summary>
        /// <param name="ignoreSelf">True to ignore transitions to self, false to allow</param>
        /// <returns>A collection of all of the unique scenes that this scene can transition to</returns>
        public IEnumerable<int> GetConnectedScenes(bool ignoreSelf = true)
        {
            HashSet<int> connectedScenes = new HashSet<int>();

            foreach (SceneTransition transition in Transitions)
            {
                if (transition.DestinationSceneId == SceneId && ignoreSelf)
                    continue;

                connectedScenes.Add(transition.DestinationSceneId);
            }

            return connectedScenes;
        }

        /// <summary>
        /// Determines if there is a transition to the
        /// specified scene index
        /// </summary>
        /// <param name="sceneIndex">The index of the scene to check connection</param>
        /// <returns>True if connected, false if not</returns>
        public bool ConnectsTo(int sceneIndex)
        {
            return transitions.Find((x) => x.destinationSceneId == sceneIndex) != null;
        }

        /// <summary>
        /// Retrieves the first scene that is considered a valid transition
        /// </summary>
        /// <param name="sceneManager">The collection of variables and scenes to be processed</param>
        /// <returns>The scene model of the first passing transition, or null if none pass</returns>
        public SceneModel GetFirstPassingTransition(SceneManagerController sceneManager)
        {
            foreach (SceneTransition transition in Transitions)
            {
                if (transition.IsMuted)
                    continue;

                SceneModel destScene = sceneManager.GetSceneById(transition.DestinationSceneId);

                if (destScene == null)
                {
                    Debug.LogError("Scene Index of " + transition.DestinationSceneId + " not found. Skipping transition");
                    continue;
                }

                if (!destScene.IsUnlocked)
                    continue;

                if (transition.CanTransition(sceneManager))
                    return destScene;
            }

            return null;
        }

        /// <summary>
        /// Find the transition model that represents a connection to the provided index
        /// </summary>
        /// <param name="destId">The index of the destination scene</param>
        /// <returns>The first matching transition, or null if none found</returns>
        public SceneTransition GetFirstTransitionTo(int destId)
        {
            return Transitions.Find((x) => x.DestinationSceneId == destId);
        }

        /// <summary>
        /// Find the transition model that represents a connection to the provided scene
        /// </summary>
        /// <param name="destScene">The scene model representing the desired destination scene</param>
        /// <returns>The first matching transition, or null if none found</returns>
        public SceneTransition GetFirstTransitionTo(SceneModel destScene)
        {
            return GetFirstTransitionTo(destScene.SceneId);
        }
    }
}