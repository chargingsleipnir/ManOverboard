using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMouseTrackerCBs {
    void MouseDownCB(Vector2 pos);
    void MouseUpCB(Vector2 pos);
    void MouseMoveCB(Vector2 pos);
}
