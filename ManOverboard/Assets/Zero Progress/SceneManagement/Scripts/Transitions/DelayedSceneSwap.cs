using System.Collections;
using UnityEngine;

namespace ZeroProgress.SceneManagementUtility
{
    public class DelayedSceneSwap : SceneSwapTransitioner
    {
        public float WaitTime = 3.0f;

        protected override IEnumerator SwapScenes(SceneManagerController sceneManager, 
            SceneModel current, SceneModel desired)
        {
            yield return new WaitForSeconds(WaitTime);

            yield return base.SwapScenes(sceneManager, current, desired);
        }
    }
}