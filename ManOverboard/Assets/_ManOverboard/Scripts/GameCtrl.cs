using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class GameCtrl : MonoBehaviour {

    const int LEVEL_SCENE_IDX_DIFF = 2;
    const int LEVEL_DATA_IDX_DIFF = 1;

    // ============================== SCENE MANAGEMENT ==============================

    public int GetCurrLevel() {
        return SceneManager.GetActiveScene().buildIndex - LEVEL_SCENE_IDX_DIFF;
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
        SceneManager.LoadScene(levelNum + LEVEL_SCENE_IDX_DIFF);
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
        return root.level[GetCurrLevel() - LEVEL_DATA_IDX_DIFF].maxCharLoss[starVal];
    }
    public int GetLevelMaxCharLoss(int level, int starVal) {
        return root.level[level - LEVEL_DATA_IDX_DIFF].maxCharLoss[starVal];
    }
}
