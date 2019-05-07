using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.IO;

public class GameCtrl : MonoBehaviour {

    public static GameCtrl instance = null;

    private static JSONRoot root;

    // ============================== GENERAL ==============================

    private void Awake() {

        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;

        DontDestroyOnLoad(gameObject);

#if UNITY_ANDROID
        Screen.fullScreen = false;
#endif

        Init();
    }

    public void Init() {
        root = null;

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

    private void GetJSONRoot(string JSONString) {
        Debug.Log("Running function GetJSONRoot");
        root = JsonUtility.FromJson<JSONRoot>(JSONString);
        Debug.Log(root);
    }

    private IEnumerator LoadLocalData(string filePath) {
        Debug.Log("Running coroutine LoadLocalData");
        UnityWebRequest www = UnityWebRequest.Get(filePath);
        yield return www.SendWebRequest();
        GetJSONRoot(www.downloadHandler.text);
    }

    public void LoadLevelData() {
        string filename = "LevelData.json";
        string filePath = Path.Combine(Application.streamingAssetsPath, filename);

#if UNITY_ANDROID
        StartCoroutine("LoadLocalData", filePath);    
#else
        if (File.Exists(filePath)) {
            GetJSONRoot(File.ReadAllText(filePath));
        }
        else {
            Debug.Log("Failed to retrieve level data");
        }
#endif      
    }

    public int GetLevelMaxCharLoss(int starVal) {
        return root.level[GetCurrLevel() - Consts.LEVEL_DATA_IDX_DIFF].maxCharLoss[starVal];
    }
    public int GetLevelMaxCharLoss(int level, int starVal) {
        return root.level[level - Consts.LEVEL_DATA_IDX_DIFF].maxCharLoss[starVal];
    }
}
