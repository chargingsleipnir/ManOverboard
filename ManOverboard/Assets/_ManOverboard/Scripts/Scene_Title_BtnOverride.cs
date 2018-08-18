using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Scene_Title_BtnOverride : MonoBehaviour {

    private Button btn;

    private void Start() {
        btn = GetComponent<Button>();

        btn.onClick.AddListener(OnClickOverride);
    }

    public void OnClickOverride() {
        //SceneNavigation.LevelOverview();
    }
}
