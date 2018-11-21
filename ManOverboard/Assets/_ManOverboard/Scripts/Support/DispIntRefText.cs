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
    private ScriptableInt intRef;

    private void Awake() {
        textComp = GetComponent<Text>();
    }

    private void Start() {
        textComp.text = introMsg + intRef.CurrentValue.ToString();
    }

    public void UpdateText() {
        textComp.text = introMsg + intRef.CurrentValue.ToString();
    }
}
