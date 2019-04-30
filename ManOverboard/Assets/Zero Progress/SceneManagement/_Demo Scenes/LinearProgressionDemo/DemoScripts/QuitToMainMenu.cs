using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.SceneManagementUtility;

public class QuitToMainMenu : MonoBehaviour {

    public SceneManagerController sceneController;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyUp(KeyCode.Q))
        {
            sceneController.SceneVariables.SetValue("Quit", true);
        }
	}
}
