using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RearMenuField : SpriteBase, IMouseUpDetector {

    public LevelManager LvlMngr { get; set; }

    public void MouseUpCB() {
        LvlMngr.ResetAll(); 
    }
}
