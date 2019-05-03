using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreGame : MonoBehaviour {

    [SerializeField]
    private GameCtrl gameCtrl;

    private void Awake()
    {
        DontDestroyOnLoad(this);

#if UNITY_ANDROID
        Screen.fullScreen = false;
#endif

    }
    void Start() {
        gameCtrl.Init();
    }
}
