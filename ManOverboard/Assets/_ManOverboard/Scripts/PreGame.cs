using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreGame : MonoBehaviour {

    [SerializeField]
    private GameCtrl gameCtrl;

    void Start() {
        gameCtrl.Init();
        gameCtrl.GoToTitle();
    }
}
