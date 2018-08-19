using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameCtrl : MonoBehaviour {

    public void RestartCurrent() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToTitle() {
        //Debug.Log("GoToTitle called");
        SceneManager.LoadScene(0);
    }

    public void GoToLevelOverview() {
        //Debug.Log("GoToLevelOverview called");
        SceneManager.LoadScene(1);
    }

    public void GoToLevel(int levelNum) {
        //Debug.Log("GoToLevel called");
        SceneManager.LoadScene(levelNum + 1);
    }
}
