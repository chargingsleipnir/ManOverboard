using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZeroProgress.SceneManagementUtility;

public class LevelButtonsMaker : MonoBehaviour {

    public SceneManagerController SceneManager;

    public Button LevelButtonPrefab;

    public Transform ButtonParent;
    
    // Use this for initialization
    void Start ()
    { 
        if (SceneManager == null)
            Debug.LogError("Scene manager not provided, cannot generate level buttons");

        if (LevelButtonPrefab == null)
            Debug.LogError("Level Button Prefab not provided, cannot generate level buttons");

        IEnumerable<SceneModel> scenes = SceneManager.GetIterableScenes();

        foreach (SceneModel scene in scenes)
        {
            Button newButton = Instantiate(LevelButtonPrefab, ButtonParent);

            Text buttonText = newButton.GetComponentInChildren<Text>();

            if (buttonText != null)
                buttonText.text = scene.SceneName;

            newButton.onClick.AddListener(() => SceneManager.TransitionToScene(scene));
        }
	}
}
