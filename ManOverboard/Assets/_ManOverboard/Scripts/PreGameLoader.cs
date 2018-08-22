using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreGameLoader : MonoBehaviour {

    public GameCtrl gameCtrl;

    private void Awake() {
        gameCtrl.LoadLevelData();
        gameCtrl.LayerCollisionSettings();
    }

    void Start () {
        gameCtrl.GoToTitle();
    }
}
