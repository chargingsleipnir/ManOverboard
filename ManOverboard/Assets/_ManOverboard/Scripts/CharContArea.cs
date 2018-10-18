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
        srRef.comp.color = new Color(srRef.comp.color.r, srRef.comp.color.g, srRef.comp.color.b, 0.66f);
        OnMouseEnterCB();
    }

    public void MouseExitCB() {
        srRef.comp.color = new Color(srRef.comp.color.r, srRef.comp.color.g, srRef.comp.color.b, 0.33f);
        OnMouseExitCB();
    }
}
