using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.SceneManagementUtility;

public class DeadPlayer : MonoBehaviour {

    public SceneManagerController SceneManager;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.D))
            SceneManager.SceneVariables.SetValue("PlayerDead", true);

	}
}
