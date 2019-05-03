using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class GameCtrl : MonoBehaviour {

    private JSONRoot root;
    private bool dataLoaded;

    // ============================== GENERAL ==============================

    public void Init() {
        root = null;
        dataLoaded = false;

        LoadLevelData();
        Consts.Init();
        DrawLayerMngr.Init();
    }

    // ============================== SCENE MANAGEMENT ==============================

    public int GetCurrLevel() {
        return SceneManager.GetActiveScene().buildIndex - Consts.LEVEL_SCENE_IDX_DIFF;
    }

    public void RestartCurrent() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToTitle() {
        SceneManager.LoadScene(1);
    }

    public void GoToLevelOverview() {
        SceneManager.LoadScene(2);
    }

    public void GoToLevel(int levelNum) {
        SceneManager.LoadScene(levelNum + Consts.LEVEL_SCENE_IDX_DIFF);
    }

    public void Quit() {
        Application.Quit();
    }

    // ============================== DATA LOADING ==============================

    public bool LoadLevelData() {
        if (dataLoaded)
            return true;

        string filePath = Path.Combine(Application.streamingAssetsPath, "LevelData.json");
        if (File.Exists(filePath)) {
            string jsonString = File.ReadAllText(filePath);
            root = JsonUtility.FromJson<JSONRoot>(jsonString);
            dataLoaded = true;
        }
        else {
            Debug.Log("Failed to retrieve level data");
        }

        return dataLoaded;        
    }

    public int GetLevelMaxCharLoss(int starVal) {
        LoadLevelData();
        return root.level[GetCurrLevel() - Consts.LEVEL_DATA_IDX_DIFF].maxCharLoss[starVal];
    }
    public int GetLevelMaxCharLoss(int level, int starVal) {
        LoadLevelData();
        return root.level[level - Consts.LEVEL_DATA_IDX_DIFF].maxCharLoss[starVal];
    }
}
