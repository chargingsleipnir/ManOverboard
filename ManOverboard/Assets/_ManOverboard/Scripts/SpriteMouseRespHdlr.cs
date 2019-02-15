using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ZeroProgress.Common;

public class SpriteMouseRespHdlr : MonoBehaviour {

    protected LevelManager lvlMngr;
    public LevelManager LvlMngr { set { lvlMngr = value; } }

    private ScriptableVector2 mousePos;

    enum SpriteMouseRespState {
        Default,
        Click,
        Toss
    }
    SpriteMouseRespState state;   

    [SerializeField]
    private RefCircle2D clickRadRefCircle;
    private SpriteBase clickRadSB;

    [SerializeField]
    private RefShape2DMouseTracker dragRadTracker;
    private SpriteBase dragRadSB;

    public void Init() {
        mousePos = Resources.Load<ScriptableVector2>("ScriptableObjects/v2_mouseWorldPos");
        Utility.RepositionZ(transform, (float)Consts.ZLayers.FrontOfWater);
        SetActive(false);

        clickRadSB = clickRadRefCircle.GetComponent<SpriteBase>();
        dragRadSB = dragRadTracker.GetComponent<SpriteBase>();
    }

    public void SetActive(bool beActive) {
        if(beActive) {
            state = SpriteMouseRespState.Click;
            transform.position = new Vector3(mousePos.CurrentValue.x, mousePos.CurrentValue.y, (float)Consts.ZLayers.FrontOfWater);
            clickRadSB.ChangeColour(null, null, null, 0.4f);
        }
        else {
            state = SpriteMouseRespState.Default;
        }

        gameObject.SetActive(beActive);
        dragRadTracker.enabled = beActive;
    }

    public void MouseMoveCB() {
        if (clickRadRefCircle.ContainsPoint(mousePos.CurrentValue)) {
            clickRadSB.ChangeColour(null, null, null, 0.4f);
            dragRadSB.ChangeColour(null, null, null, 0.2f);

            // Came back into inner circle after once being out of it.
            if (state == SpriteMouseRespState.Toss)
                lvlMngr.HeldSpriteRelease();
        }
        else {
            clickRadSB.ChangeColour(null, null, null, 0.2f);
            dragRadSB.ChangeColour(null, null, null, 0.4f);

            state = SpriteMouseRespState.Toss;
            lvlMngr.HeldSpriteEnterTossArea();
        }
    }

    // Character dragged outside max drag range
    public void MouseExitCB() {
        dragRadSB.ChangeColour(null, null, null, 0.2f);

        lvlMngr.HeldSpriteRelease();
    }

    public void MouseUpCB() {
        clickRadSB.ChangeColour(null, null, null, 0.2f);
        dragRadSB.ChangeColour(null, null, null, 0.2f);

        if (state == SpriteMouseRespState.Click) {
            lvlMngr.HeldSpriteClick();
        }
        else if(state == SpriteMouseRespState.Toss) {
            lvlMngr.HeldSpriteToss();
        }
    }
}
