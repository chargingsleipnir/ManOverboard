﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;

public class CharActionable : CharBase {

    protected int strength;

    [SerializeField]
    protected GameObject actionBtnObj;
    public override bool IsActionBtnActive {
        get { return actionBtnObj.activeSelf; }
        set { actionBtnObj.SetActive(value); }
    }

    [SerializeField]
    protected GameObject cancelBtnObj;
    public override bool IsCancelBtnActive {
        get { return cancelBtnObj.activeSelf; }
        set { cancelBtnObj.SetActive(value); }
    }

    private SpriteRenderer actBtnSR;

    [SerializeField]
    protected GameObject commandPanel;
    public override bool IsCommandPanelOpen {
        get { return commandPanel.activeSelf; }
        set { commandPanel.SetActive(value); }
    }

    protected bool GUIActive;
    protected Rect actBtnRect;
    protected Rect cmdPanelRect;

    [SerializeField]
    protected Transform trans_ItemUseHand;

    protected override void Awake() {
        base.Awake();
        strength = 50;
        actBtnSR = actionBtnObj.GetComponent<SpriteRenderer>();
    }

    public override void ApplyTransformToContArea(GameObject contAreaObj) {
        actBtnRect = new Rect(
            actionBtnObj.transform.position.x,
            actionBtnObj.transform.position.y,
            actBtnSR.size.x * actionBtnObj.transform.lossyScale.x,
            actBtnSR.size.y * actionBtnObj.transform.lossyScale.y
        );

        float btnTopToCharTop = actBtnRect.height > 0 ? actBtnRect.yMax - (transform.position.y + (srRef.comp.size.y * 0.5f)) : 0;
        contAreaObj.transform.position = new Vector3(transform.position.x, transform.position.y + (btnTopToCharTop * 0.5f), (float)Consts.ZLayers.FrontOfWater);
        contAreaObj.transform.localScale = new Vector3(Utility.GreaterOf(srRef.comp.size.x, actBtnRect.width) + Consts.CONT_AREA_BUFFER, srRef.comp.size.y + btnTopToCharTop + Consts.CONT_AREA_BUFFER, 1);
    }

    public override void MouseDownCB() {
        if (IsCommandPanelOpen)
            return;

        base.MouseDownCB();

        if (state == Consts.CharState.Default)
            IsActionBtnActive = true;
        else
            IsCancelBtnActive = true;
    }
    public override void MouseUpCB() {
        if (IsCommandPanelOpen)
            return;

        base.MouseUpCB();

        if (state == Consts.CharState.Default)
            IsActionBtnActive = false;
        else
            IsCancelBtnActive = false;
    }

    public override void OverheadButtonActive(bool isActive) {
        if (state == Consts.CharState.Default)
            IsActionBtnActive = isActive;
        else
            IsCancelBtnActive = isActive;
    }

    public void OpenCommandPanel() {
        IsCommandPanelOpen = true;
        IsActionBtnActive = false;
        IsCancelBtnActive = false;
        state = Consts.CharState.MenuOpen;
    }

    public virtual void CancelAction() {
        IsCancelBtnActive = false;
        state = Consts.CharState.Default;
    }
}
