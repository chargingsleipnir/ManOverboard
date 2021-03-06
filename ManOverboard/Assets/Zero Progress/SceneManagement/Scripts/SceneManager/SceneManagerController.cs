﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZeroProgress.Common;

namespace ZeroProgress.SceneManagementUtility
{
    [CreateAssetMenu(fileName = "New Scene Manager",
        menuName = ScriptableObjectPaths.ZERO_PROGRESS_COMMON_PATH + "Scene Manager", order = (int)RootIndexing.SceneManager)]
    public class SceneManagerController : ScriptableObject
    {
        public static int ANY_SCENE_ID = -1;

        public event EventHandler OnTransitionerChanged;

        [SerializeField]
        private SceneVariableContainer sceneVariables;

        [SerializeField]
        public List<SceneModel> scenes = new List<SceneModel>();

        [SerializeField]
        private SceneModel anyScene = new SceneModel(ANY_SCENE_ID, "Any Scene");

        public SceneModel AnyScene
        {
            get { return anyScene; }
            private set { anyScene = value; }
        }

        [SerializeField]
        private int entrySceneId = -1;

        public SceneModel EntryScene
        {
            get { return GetSceneById(entrySceneId); }
        }
        
        private SceneModel activeScene = null;

        public SceneModel ActiveScene
        {
            get { return activeScene; }
            private set { activeScene = value; }
        }

        /// <summary>
        /// The variables used to manage scene transitions
        /// </summary>
        public SceneVariableContainer SceneVariables
        {
            get { return sceneVariables; }
            private set { sceneVariables = value; }
        }
        
        private ISceneTransitioner transitioner;

        /// <summary>
        /// The logic for swapping between scenes
        /// </summary>
        public ISceneTransitioner Transitioner
        {
            get { return transitioner; }
            set
            {
                if (transitioner == value)
                    return;

                if (transitioner != null)
                {
                    transitioner.OnTransitionCompleted.RemoveListener(Transitioner_OnSceneChanged);
                    transitioner.OnTransitionCompleted.RemoveListener(sceneVariables.OnTransitionCompleted);
                }

                transitioner = value;

                if (transitioner != null)
                {
                    transitioner.OnTransitionCompleted.AddListener(Transitioner_OnSceneChanged);
                    transitioner.OnTransitionCompleted.AddListener(sceneVariables.OnTransitionCompleted);
                }

                OnTransitionerChanged.SafeInvoke(this, EventArgs.Empty);
            }
        }

        private bool isBulkUpdateEnabled = false;

#if UNITY_EDITOR

        [SerializeField, Tooltip("True to use this Controller when the Play button is pressed. Use in conjunction with goToOnPlay if " +
            "you want to force a specific transition")]
        private bool useOnPlay = false;

        [SerializeField, Tooltip("If UseOnPlay is true, this will direct the controller to load this specific scene. If this is null then " +
            "the controller will evaluate scenes as normal")]
        private string goToOnPlay = null;

        private bool ignoreEntryCall = false;
#endif

        private void OnEnable()
        {
            activeScene = null;

            if (sceneVariables == null)
                sceneVariables = CreateInstance<SceneVariableContainer>();

            sceneVariables.OnVariableChanged -= SceneVariables_OnVariableChanged;
            sceneVariables.OnVariableChanged += SceneVariables_OnVariableChanged;

            sceneVariables.ResetVariables();            
        }

        public int AddScene(string scenePath)
        {
            if (string.IsNullOrEmpty(scenePath))
                throw new ArgumentException("Invalid scene path provided");

            if (GetSceneByPath(scenePath) != null)
                throw new ArgumentException("Scene with provided path already exists");

            int sceneId = FindNextAvailableId();

            SceneModel newScene = new SceneModel(sceneId, scenePath);

            scenes.Add(newScene);

            if (entrySceneId < 0)
                entrySceneId = newScene.SceneId;

            return newScene.SceneId;
        }

        public SceneModel GetSceneById(int id)
        {
            if (id == ANY_SCENE_ID)
                return AnyScene;

            return scenes.Find((x) => x.SceneId == id);
        }

        public SceneModel GetSceneByPath(string scenePath)
        {
            return scenes.Find((x) => x.SceneAssetPath == scenePath);
        }

        public SceneModel GetSceneByName(string sceneName, StringComparison comparer = StringComparison.OrdinalIgnoreCase)
        {
            return scenes.Find((x) => x.SceneName.Equals(sceneName, comparer));
        }

        public IEnumerable<SceneModel> GetIterableScenes()
        {
            foreach (SceneModel scene in scenes)
            {
                if (scene.IncludeInIteration)
                    yield return scene;
            }
        }

        public SceneModel GetNextScene(SceneModel startScene, bool wrapAround = false, bool canReturnSelf = false)
        {
            int modelIndex = scenes.FindIndex((x) => x == startScene);

            if (modelIndex < 0)
                return null;
            
            do
            {
                modelIndex++;

                if (modelIndex > scenes.Count - 1)
                {
                    if (wrapAround)
                        modelIndex = 0;
                    else
                        return null;
                }

                SceneModel current = scenes[modelIndex];

                if (current.IncludeInIteration)
                    return current;

                if (current == startScene)
                {
                    if (canReturnSelf)
                        return startScene;
                    else
                        return null;
                }
            } while (true);
        }

        public SceneModel GetPreviousScene(SceneModel startScene, bool wrapAround = false, bool canReturnSelf = false)
        {
            int modelIndex = scenes.FindIndex((x) => x == startScene);

            if (modelIndex < 0)
                return null;

            do
            {
                modelIndex--;

                if (modelIndex < 0)
                {
                    if (wrapAround)
                        modelIndex = scenes.Count - 1;
                    else
                        return null;
                }

                SceneModel current = scenes[modelIndex];

                if (current.IncludeInIteration)
                    return current;

                if (current == startScene)
                {
                    if (canReturnSelf)
                        return startScene;
                    else
                        return null;
                }
            } while (true);
        }

        public IEnumerable<string> GetScenePaths()
        {
            return scenes.Select((x) => x.SceneAssetPath);
        }

        public void BeginBulkUpdate()
        {
            isBulkUpdateEnabled = true;
        }

        public void EndBulkUpdate()
        {
            isBulkUpdateEnabled = false;
            Evaluate();
        }

        public void ClearScenes()
        {
            scenes.Clear();
            anyScene.Transitions.Clear();
        }

        public void RemoveScene(string sceneAssetPath)
        {
            int index = scenes.FindIndex((x) => x.SceneAssetPath.Equals(sceneAssetPath));

            if (index < 0)
                return;

            scenes.RemoveAt(index);
        }

        public void ReevaluateLockStatuses()
        {
            anyScene.ReevaluateLockStatus(this);

            foreach (SceneModel scene in scenes)
            {
                scene.ReevaluateLockStatus(this);
            }
        }

        public void TransitionToEntry()
        {
#if UNITY_EDITOR
            if (ignoreEntryCall)
                return;
#endif
            // If no entry scene has been set, just try to find the current scene
            if (EntryScene == null)
            {
                SceneModel model = scenes.Find((x) =>
                    x.SceneAssetPath == SceneManager.GetActiveScene().path);

                ActiveScene = model;
                Evaluate();
                return;
            }
            
            // Check if the scene is already active
            if (SceneManagerExtensions.IsSceneLoadedByPath(EntryScene.SceneAssetPath))
            {
                ActiveScene = EntryScene;
                // it's already active, so do the starting evaluation
                Evaluate();
                return;
            }

            TransitionToScene(EntryScene);
        }

        public void TransitionToScene(string sceneName)
        {
            SceneModel scene = GetSceneByName(sceneName);

            if (scene == null)
            {
                Debug.LogError("Failed to find scene " + sceneName + ", cannot transition");
                return;
            }

            TransitionToScene(scene);

        }

        public void TransitionToScene(SceneModel desiredScene)
        {
            if (Transitioner == null)
            {
                Debug.LogError("Cannot transition scenes, transitioner is null");
                return;
            }

            if (desiredScene == null)
            {
                Debug.LogError("Target scene is null, cannot switch");
                return;
            }

            DoTransition(desiredScene);
        }

        public void TransitionIfUnlocked(string sceneName)
        {
            SceneModel scene = GetSceneByName(sceneName);

            if (scene == null)
            {
                Debug.LogError("Failed to find scene " + sceneName + ", cannot transition");
                return;
            }

            TransitionIfUnlocked(scene);
        }

        public void TransitionIfUnlocked(SceneModel desiredScene)
        {
            if (Transitioner == null)
            {
                Debug.LogError("Cannot transition scenes, transitioner is null");
                return;
            }

            if (desiredScene == null)
            {
                Debug.LogError("Target scene is null, cannot switch");
                return;
            }

            if (desiredScene.IsUnlocked)
                Transitioner.Transition(this, activeScene, desiredScene);
        }

        public void Evaluate()
        {
            ReevaluateLockStatuses();

            if (ActiveScene == null)
                return;

            SceneModel dest = ActiveScene.GetFirstPassingTransition(this);

            if (dest == null && ActiveScene.UseAnySceneTransitions)
            {
                dest = AnyScene.GetFirstPassingTransition(this);

                if (dest != null && dest.SceneId == ANY_SCENE_ID)
                    dest = ActiveScene;
            }
            if (dest != null)
                TransitionToScene(dest);
        }

        private void SceneVariables_OnVariableChanged(object sender, EventArgs<string> e)
        {
            if (!isBulkUpdateEnabled)
                Evaluate();
        }

        private void Transitioner_OnSceneChanged(SceneTransitionEventArgs e)
        {
            if (e.DestinationScene == null)
            {
                Debug.LogError("Transitioned-to scene not provided, cannot set Active Scene");
                return;
            }

            SceneModel existingScene = GetSceneById(e.DestinationScene.SceneId);

            if (existingScene == null)
            {
                Debug.LogError("Transitioned-to scene not a part of the scene manager, cannot set Active Scene");
                return;
            }
            
            ActiveScene = existingScene;
            Evaluate();
        }

        private int FindNextAvailableId()
        {
            int sceneId = 0;

            do
            {
                ++sceneId;
            } while (scenes.Find( (x) => x.SceneId == sceneId) != null);

            return sceneId;
        }

        private void DoTransition(SceneModel desiredScene)
        {
            Transitioner.Transition(this, ActiveScene, desiredScene);
        }

#if UNITY_EDITOR
        private static SceneManagerController playModeController = null;

        private static bool wasEntryOpened = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void LoadEntrySceneOnPlayMode()
        {
            IEnumerable<SceneManagerController> controllers =
                Common.Editors.AssetDatabaseExtensions.GetAssetsOfType<SceneManagerController>();

            playModeController = null;
            wasEntryOpened = false;

            SceneManagerController controllerToUse = controllers.FirstOrDefault((x) => x.useOnPlay);

            if (controllerToUse == null || controllerToUse.EntryScene == null)
                return;
            
            playModeController = controllerToUse;
            
            playModeController.ignoreEntryCall = true;
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            
            if (!SceneManagerExtensions.IsSceneLoadedByPath(controllerToUse.EntryScene.SceneAssetPath))
                SceneManager.LoadScene(controllerToUse.EntryScene.SceneName);            
        }

        private static void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (playModeController == null)
                return;

            if (!wasEntryOpened)
            {
                if (arg0.path == playModeController.EntryScene.SceneAssetPath)
                    wasEntryOpened = true;

                if (!wasEntryOpened)
                    return;
            }

            if (string.IsNullOrEmpty(playModeController.goToOnPlay))
            {
                playModeController.ignoreEntryCall = false;
                playModeController = null;
                return;
            }

            if (arg0.path == playModeController.GetSceneByName(playModeController.goToOnPlay).SceneAssetPath)
            {
                playModeController.ignoreEntryCall = false;
                playModeController = null;
            }
            else
            {
                if (playModeController.Transitioner == null)
                    playModeController.OnTransitionerChanged += PlayModeController_OnTransitionerChanged;
                else
                    playModeController.TransitionToScene(playModeController.goToOnPlay);
            }

            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        }

        private static void PlayModeController_OnTransitionerChanged(object sender, EventArgs e)
        {
            if (playModeController == null || string.IsNullOrEmpty(playModeController.goToOnPlay))
                return;

            if (playModeController.Transitioner == null)
                return;

            playModeController.OnTransitionerChanged -= PlayModeController_OnTransitionerChanged;
            playModeController.TransitionToScene(playModeController.goToOnPlay);

            playModeController = null;
        }
#endif
    }
}