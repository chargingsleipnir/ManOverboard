using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.IO;

public class GameCtrl : MonoBehaviour {

    private JSONRoot root;
    private bool dataLoaded;
    private bool startRan;

    // ============================== GENERAL ==============================

    private void Awake() {
        startRan = false;

        DontDestroyOnLoad(this);

#if UNITY_ANDROID
        Screen.fullScreen = false;
#endif

    }

    public void Start() {
        if (startRan)
            return;

        root = null;
        dataLoaded = false;

        LoadLevelData();
        Consts.Init();
        DrawLayerMngr.Init();
        startRan = true;
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
        root = JsonUtility.FromJson<JSONRoot>(JSONString);
        dataLoaded = true;
    }

    private IEnumerator LoadLocalData(string filePath) {
        UnityWebRequest www = UnityWebRequest.Get(filePath);
        yield return www.SendWebRequest();
        GetJSONRoot(www.downloadHandler.text);
    }

    public bool LoadLevelData() {
        if (dataLoaded)
            return true;

        string filename = "LevelData.json";
        string filePath = Path.Combine(Application.streamingAssetsPath, filename);

#if UNITY_ANDROID
        //filePath = "jar:file://" + Application.dataPath + "!/assets/" + filename;
        StartCoroutine("LoadLocalData", filePath);    
#else
        //filePath = Application.dataPath + "/StreamingAssets/" + filename;
        if (File.Exists(filePath)) {
            GetJSONRoot(File.ReadAllText(filePath));
        }
        else {
            Debug.Log("Failed to retrieve level data");
        }
#endif

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
