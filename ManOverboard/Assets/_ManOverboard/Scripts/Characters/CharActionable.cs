using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;

public class CharActionable : CharBase {

    protected DelPassItemType DelSetItemType;
    protected DelPassWaterRemoveData DelStartWaterRemove;
    protected DelPassWaterRemoveCo DelStopWaterRemove;
    protected DelVoid FadeLevel;
    protected DelVoid UnFadeLevel;

    protected Coroutine activeCo;

    protected ItemBase activeItem;

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
        set {
            commandPanel.SetActive(value);
            if (value == true)
                state = Consts.CharState.MenuOpen;
            else if(state == Consts.CharState.MenuOpen)
                state = Consts.CharState.Default;
        }
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

        float btnTopToCharTop = actBtnRect.height > 0 ? actBtnRect.yMax - (transform.position.y + (srRef.comp.sprite.bounds.size.y * 0.5f)) : 0;
        contAreaObj.transform.position = new Vector3(transform.position.x, transform.position.y + (btnTopToCharTop * 0.5f), (float)Consts.ZLayers.FrontOfWater);
        contAreaObj.transform.localScale = new Vector3(Utility.GreaterOf(srRef.comp.sprite.bounds.size.x, actBtnRect.width) + Consts.CONT_AREA_BUFFER, srRef.comp.sprite.bounds.size.y + btnTopToCharTop + Consts.CONT_AREA_BUFFER, 1);
    }

    public override void MouseDownCB() {
        if (IsCommandPanelOpen || saved || tossed)
            return;

        held = true;

        if (state == Consts.CharState.Default)
            IsActionBtnActive = true;
        else
            IsCancelBtnActive = true;

        OnMouseDownCB(gameObject);
    }
    public override void MouseUpCB() {
        if (IsCommandPanelOpen || saved || tossed || held == false)
            return;

        held = false;

        if (state == Consts.CharState.Default)
            IsActionBtnActive = false;
        else
            IsCancelBtnActive = false;

        OnMouseUpCB();
    }

    public override void SetCallbacks(DelPassItemType setTypeCB, DelPassWaterRemoveData startCoCB, DelPassWaterRemoveCo stopCoCB, DelVoid fadeLevelCB, DelVoid unfadeLevelCB) {
        DelSetItemType = setTypeCB;
        DelStartWaterRemove = startCoCB;
        DelStopWaterRemove = stopCoCB;
        FadeLevel = fadeLevelCB;
        UnFadeLevel = unfadeLevelCB;
    }

    public override void OverheadButtonActive(bool isActive) {
        if (state == Consts.CharState.Default)
            IsActionBtnActive = isActive;
        else
            IsCancelBtnActive = isActive;
    }

    // TODO: Check if given commands are available, and disable buttons if not.
    // In the case of crewman, the button for scooping water should be disabled if no scooping items are available.

    public virtual void CheckCommandViability() {}

    public void OpenCommandPanel() {
        IsCommandPanelOpen = true;
        IsActionBtnActive = false;
        CheckCommandViability();
        FadeLevel();
    }


    public virtual void CancelAction() {
        IsCancelBtnActive = false;
        state = Consts.CharState.Default;
        UnFadeLevel();
    }
}
