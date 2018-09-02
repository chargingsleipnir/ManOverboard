using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharContArea : MonoBehaviour {

    public delegate void MouseEnterExitDel();
    MouseEnterExitDel OnMouseEnterCB;
    MouseEnterExitDel OnMouseExitCB;

    private SpriteRenderer sr;
    private BoxCollider2D bc;

    private bool prevOverlapState;
    private bool currOverlapState;

    private void Awake() {
        sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider2D>();
    }

    private void Start() {
        prevOverlapState = false;
        currOverlapState = false;
    }

    private void MouseEnter() {
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.66f);
        OnMouseEnterCB();
    }

    private void MouseExit() {
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.33f);
        OnMouseExitCB();
    }

    // Added bools to specifically make these trigger callbacks only on change.
    // I had to do this manually since this object is behind another sprite, and thus, not recieving mouse events. (I couldn't find any efficient way to override this behaviour)
    public void CheckMouseOverlap(Vector2 mousePos) {
        if (bc.OverlapPoint(mousePos)) {
            currOverlapState = true;
            if(!prevOverlapState)
                MouseEnter();
        }
        else {
            currOverlapState = false;
            if(prevOverlapState)
                MouseExit();
        }

        prevOverlapState = currOverlapState;
    }

    public void SetMouseCBs(MouseEnterExitDel EnterCB, MouseEnterExitDel ExitCB) {
        OnMouseEnterCB = EnterCB;
        OnMouseExitCB = ExitCB;
        MouseEnter();
    }
}
