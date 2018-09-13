using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;

public class CharContArea : MonoBehaviour, IMouseEnterDetector, IMouseExitDetector {

    public delegate void MouseEnterExitDel();
    MouseEnterExitDel OnMouseEnterCB;
    MouseEnterExitDel OnMouseExitCB;

    private SpriteRenderer sr;

    private void Awake() {
        sr = GetComponent<SpriteRenderer>();
    }

    public void SetMouseCBs(MouseEnterExitDel EnterCB, MouseEnterExitDel ExitCB) {
        OnMouseEnterCB = EnterCB;
        OnMouseExitCB = ExitCB;
        MouseEnterCB();
    }

    public void MouseEnterCB() {
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.66f);
        OnMouseEnterCB();
    }

    public void MouseExitCB() {
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.33f);
        OnMouseExitCB();
    }
}
