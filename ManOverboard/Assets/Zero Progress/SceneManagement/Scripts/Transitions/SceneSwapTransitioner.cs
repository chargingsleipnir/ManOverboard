using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZeroProgress.Common;

namespace ZeroProgress.SceneManagementUtility
{
    public class SceneSwapTransitioner : BaseTransitionerBehaviour
    {
        protected Coroutine swapperCoroutine = null;

        public override bool Transition(SceneManagerController sceneManager,
            SceneModel current, SceneModel desired)
        {
            if (swapperCoroutine != null)
                return false;

            OnTransitionStarted.SafeInvoke(new SceneTransitionEventArgs(sceneManager, current, desired));

            swapperCoroutine = StartCoroutine(SwapScenes(sceneManager, current, desired));

            return true;
        }

        protected virtual IEnumerator SwapScenes(SceneManagerController sceneManager,
            SceneModel current, SceneModel desired)
        {
            yield return SceneManager.LoadSceneAsync(desired.SceneName, LoadSceneMode.Single);

            OnTransitionCompleted.SafeInvoke(
                new SceneTransitionEventArgs(sceneManager, current, desired));

            swapperCoroutine = null;
        }
    }
}