using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;

public class CharActionable : CharBase {
    [SerializeField]
    protected GameObject actionBtnObj;
    private SpriteRenderer actBtnSR;
    private bool actBtnHovered;
    public bool ActBtnHovered {
        get { return actBtnHovered; }
        set {
            actBtnHovered = value;
            ChangeMouseUpWithDownLinks(!value);
        }
    }

    [SerializeField]
    protected GameObject commandPanel;

    protected bool GUIActive;
    protected Rect actBtnRect;
    protected Rect cmdPanelRect;

    protected override void Awake() {
        base.Awake();
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
        contAreaObj.transform.localScale = new Vector3(Utility.GreaterOf(srRef.comp.size.x, actBtnRect.width) + CONT_AREA_BUFFER, srRef.comp.size.y + btnTopToCharTop + CONT_AREA_BUFFER, 1);
    }

    public override void SetActionBtnActive(bool isActive) {
        actionBtnObj.SetActive(isActive);
    }

    public override void SetCommandBtnsActive(bool isActive) {
        commandPanel.SetActive(isActive);
    }

    public override bool GetMenuOpen() {
        return commandPanel.activeSelf;
    }

    public override void MouseDownCB() {
        if (saved || tossed || commandPanel.activeSelf)
            return;

        actionBtnObj.SetActive(true);

        held = true;
        charMouseDownEvent.RaiseEvent(gameObject);
    }
    public override void MouseUpCB() {
        if (saved || tossed || ActBtnHovered || commandPanel.activeSelf || held == false)
            return;

        actionBtnObj.SetActive(false);

        held = false;
        charMouseUpEvent.RaiseEvent();
    }

    public void ActBtnMouseUp() {
        commandPanel.SetActive(true);
        actionBtnObj.SetActive(false);
        ActBtnHovered = false;
        charMouseUpEvent.RaiseEvent();
    }
}
