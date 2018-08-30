using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharContArea : MonoBehaviour {

    public delegate void MouseEnterExitDel();
    MouseEnterExitDel OnMouseEnterCB;
    MouseEnterExitDel OnMouseExitCB;

    private SpriteRenderer sr;
    private BoxCollider2D bc;

    private void Awake() {
        sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider2D>();
    }

    private void MouseEnter() {
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.66f);
        OnMouseEnterCB();
    }

    private void MouseExit() {
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.33f);
        OnMouseExitCB();
    }

    // TODO: Add bools to specifically make these trigger callbacks only on change.
    public void CheckMouseOverlap(Vector2 mousePos) {
        if (bc.OverlapPoint(mousePos))
            MouseEnter();
        else
            MouseExit();
    }

    public void SetMouseCBs(MouseEnterExitDel EnterCB, MouseEnterExitDel ExitCB) {
        OnMouseEnterCB = EnterCB;
        OnMouseExitCB = ExitCB;
        MouseEnter();
    }
}
