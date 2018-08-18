using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;
using UnityEngine.UI;

public class DispIntRefText : MonoBehaviour {

    private Text textComp;
    public IntReference intRef;

    private void Start() {
        textComp = GetComponent<Text>();
        textComp.text = intRef.Value.ToString();
    }

    // TODO: If a callback on ScriptableInt gets implemented, cease this infernal polling.
    private void Update() {
        textComp.text = intRef.Value.ToString();
    }
}
