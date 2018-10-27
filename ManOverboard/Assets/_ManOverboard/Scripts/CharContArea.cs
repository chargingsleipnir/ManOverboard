using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;

public class CharContArea : SpriteBase, IMouseEnterDetector, IMouseExitDetector {

    public delegate void MouseEnterExitDel();
    MouseEnterExitDel OnMouseEnterCB;
    MouseEnterExitDel OnMouseExitCB;

    public void SetMouseCBs(MouseEnterExitDel EnterCB, MouseEnterExitDel ExitCB) {
        OnMouseEnterCB = EnterCB;
        OnMouseExitCB = ExitCB;
    }

    public void MouseEnterCB() {
        ChangeColour(null, null, null, 0.66f);
        OnMouseEnterCB();
    }

    public void MouseExitCB() {
        ChangeColour(null, null, null, 0.33f);
        OnMouseExitCB();
    }
}
