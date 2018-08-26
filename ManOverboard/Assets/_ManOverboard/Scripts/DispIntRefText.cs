using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;
using UnityEngine.UI;

public class DispIntRefText : MonoBehaviour {

    private Text textComp;
    public string introMsg;
    public IntReference intRef;

    private void Start() {
        textComp = GetComponent<Text>();
        textComp.text = introMsg + intRef.Value.ToString();
    }

    // TODO: If a callback on ScriptableInt gets implemented, cease this infernal polling.
    private void Update() {
        textComp.text = introMsg + intRef.Value.ToString();
    }
}
