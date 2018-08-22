using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;
using UnityEngine.UI;

public class DispStrRefText : MonoBehaviour {

    private Text textComp;
    public StringReference strRef;

    private void Start() {
        textComp = GetComponent<Text>();
        textComp.text = strRef.Value;
    }

    // TODO: If a callback on ScriptableInt gets implemented, cease this infernal polling.
    private void Update() {
        textComp.text = strRef.Value;
    }
}
