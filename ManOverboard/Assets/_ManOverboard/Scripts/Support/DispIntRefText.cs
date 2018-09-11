using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;
using UnityEngine.UI;

public class DispIntRefText : MonoBehaviour {

    private Text textComp;
    [SerializeField]
    private string introMsg;
    [SerializeField]
    private IntReference intRef;

    private void Awake() {
        textComp = GetComponent<Text>();
    }

    private void Start() {
        textComp.text = introMsg + intRef.Value.ToString();
    }

    public void UpdateText() {
        textComp.text = introMsg + intRef.Value.ToString();
    }
}
