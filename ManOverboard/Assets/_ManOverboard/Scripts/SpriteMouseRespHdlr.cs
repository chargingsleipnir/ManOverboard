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

    [SerializeField]
    private RefShape2DMouseTracker dragRadTracker;

    public void Init() {
        mousePos = Resources.Load<ScriptableVector2>("ScriptableObjects/v2_mouseWorldPos");
        Utility.RepositionZ(transform, (float)Consts.ZLayers.FrontOfWater);
        SetActive(false);
    }

    public void SetActive(bool beActive) {
        if(beActive) {
            state = SpriteMouseRespState.Click;
            transform.position = new Vector3(mousePos.CurrentValue.x, mousePos.CurrentValue.y, (float)Consts.ZLayers.FrontOfWater);
        }
        else {
            state = SpriteMouseRespState.Default;
        }

        gameObject.SetActive(beActive);
        dragRadTracker.enabled = beActive;
    }

    public void MouseMoveCB() {
        //ChangeColour(null, null, null, 0.66f);
        if (clickRadRefCircle.ContainsPoint(mousePos.CurrentValue)) {
            // Came back into inner circle after once being out of it.
            if (state == SpriteMouseRespState.Toss) {

                // TODO: This is a blanket function - I can surely do something more specific/tailored to this situation
                lvlMngr.ResetAll();
                //lvlMngr.HeldSpriteExitTossArea();
            }
        }
        else {
            state = SpriteMouseRespState.Toss;
            lvlMngr.HeldSpriteEnterTossArea();
        }
    }

    // Character dragged outside max drag range
    public void MouseExitCB() {
        //ChangeColour(null, null, null, 0.33f);

        // TODO: This is a blanket function - I can surely do something more specific/tailored to this situation
        lvlMngr.ResetAll();
        //lvlMngr.HeldSpriteExitTossArea();
    }

    public void MouseUpCB() {
        if (state == SpriteMouseRespState.Click) {
            lvlMngr.HeldSpriteClick();
        }
        else if(state == SpriteMouseRespState.Toss) {
            lvlMngr.HeldSpriteToss();
        }
    }
}
