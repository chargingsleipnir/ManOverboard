using System.Collections;
using UnityEngine;
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
            float progressModifier = current == null ? 1f : 0.5f;

            yield return LoadSceneAsync(desired, progressModifier);

            if (current != null)
                yield return UnloadSceneAsync(current, 2f);

            OnTransitionCompleted.SafeInvoke(
                new SceneTransitionEventArgs(sceneManager, current, desired));

            swapperCoroutine = null;
        }
    }
}