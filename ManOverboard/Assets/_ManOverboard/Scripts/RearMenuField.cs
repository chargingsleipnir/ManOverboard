using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RearMenuField : MonoBehaviour, IMouseUpDetector {

    public LevelManager Mngr { get; set; }

    public void MouseUpCB() {
        Mngr.ReturnToNeutral(); 
    }
}
