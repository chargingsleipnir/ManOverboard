using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZeroProgress.Common;

namespace ZeroProgress.SceneManagementUtility
{
    public abstract class BaseTransitionerBehaviour : MonoBehaviour, ISceneTransitioner
    {
        private SceneTransitionEvent onTransitionStarted = new SceneTransitionEvent();

        public SceneTransitionEvent OnTransitionStarted
        {
            get { return onTransitionStarted; }
            set { onTransitionStarted = value; }
        }

        private SceneTransitionEvent onTransitionCompleted = new SceneTransitionEvent();

        public SceneTransitionEvent OnTransitionCompleted
        {
            get { return onTransitionCompleted; }
            set { onTransitionCompleted = value; }
        }

        private UnityFloatEvent onLoadProgressChanged = new UnityFloatEvent();

        public UnityFloatEvent OnLoadProgressChanged
        {
            get { return onLoadProgressChanged; }
            set { onLoadProgressChanged = value; }
        }
        
        public abstract bool Transition(SceneManagerController sceneManager, 
            SceneModel current, SceneModel desired);

        protected IEnumerator LoadAdditiveSceneAsync(SceneModel desired, float progressModifier = 1f)
        {
            AsyncOperation loadOp = SceneManager.LoadSceneAsync(desired.SceneName, LoadSceneMode.Additive);

            if (loadOp == null)
            {
                Debug.LogError("Failed to load scene: " + desired.SceneName);
                yield break;
            }

            float previousProgress = 0f;

            while (!loadOp.isDone)
            {
                float progress = loadOp.progress * progressModifier;

                if (progress != previousProgress)
                    onLoadProgressChanged.Invoke(progress);

                previousProgress = progress;
                yield return null;
            }
            
            Debug.Log("Loaded new scene: " + desired.SceneName);
        }

        protected IEnumerator UnloadSceneAsync(SceneModel target, float progressModifier = 1f)
        {
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(target.SceneName);

            float previousProgress = 0f;

            while (!unloadOp.isDone)
            {
                float progress = unloadOp.progress * progressModifier;

                if (progress != previousProgress)
                    onLoadProgressChanged.Invoke(progress);

                previousProgress = progress;
                yield return null;
            }

            Debug.Log("Unloaded old scene: " + target.SceneName);
        }
    }
}
