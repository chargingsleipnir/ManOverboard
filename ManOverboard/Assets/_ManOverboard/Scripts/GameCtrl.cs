using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class GameCtrl : MonoBehaviour {


    // ============================== SCENE MANAGEMENT ==============================

    public int GetCurrLevel() {
        return SceneManager.GetActiveScene().buildIndex - 1;
    }

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



    // ============================== DATA LOADING ==============================

    JSONRoot root;

    public void LoadLevelData() {
        string filePath = Path.Combine(Application.streamingAssetsPath, "LevelData.json");
        if (File.Exists(filePath)) {
            string jsonString = File.ReadAllText(filePath);
            root = JsonUtility.FromJson<JSONRoot>(jsonString);
            //Debug.Log(root.level[0].maxCharLoss[0]);
        }
    }

    public int GetLevelMaxCharLoss(int starVal) {
        return root.level[GetCurrLevel() - 1].maxCharLoss[starVal];
    }
    public int GetLevelMaxCharLoss(int level, int starVal) {
        if(root == null) {
            LoadLevelData();
        }

        return root.level[level - 1].maxCharLoss[starVal];
    }
}
