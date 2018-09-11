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
        set { actBtnHovered = value; Debug.Log(actBtnHovered); }
    }

    [SerializeField]
    protected GameObject commandPanel;

    protected bool GUIActive;
    protected Rect actBtnRect;
    protected Rect cmdPanelRect;

    protected override void Awake() {
        base.Awake();
        actBtnSR = actionBtnObj.GetComponent<SpriteRenderer>();
        //actBtnRect = GetCanvasElemRect(actionBtnObj, true);
    }

    public override void ApplyTransformToContArea(GameObject contAreaObj) {
        //actBtnRect = GetCanvasElemRect(actionBtnObj, true);
        actBtnRect = new Rect(
            actionBtnObj.transform.position.x,
            actionBtnObj.transform.position.y,
            actBtnSR.size.x * actionBtnObj.transform.lossyScale.x,
            actBtnSR.size.y * actionBtnObj.transform.lossyScale.y
        );

        float btnTopToCharTop = actBtnRect.height > 0 ? actBtnRect.yMax - (transform.position.y + (sr.size.y * 0.5f)) : 0;
        contAreaObj.transform.position = new Vector3(transform.position.x, transform.position.y + (btnTopToCharTop * 0.5f), (float)Consts.ZLayers.GrabbedObjHighlight + 0.1f);
        contAreaObj.transform.localScale = new Vector3(Utility.GreaterOf(sr.size.x, actBtnRect.width) + CONT_AREA_BUFFER, sr.size.y + btnTopToCharTop + CONT_AREA_BUFFER, 1);
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

        Debug.Log("Made it this far");
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


    // Internal/helper methods ==========================================

    //protected Rect GetCanvasElemRect(GameObject obj, bool scaledValues) {
    //    if (scaledValues) {
    //        RectTransform btnRectTrans = obj.GetComponent<RectTransform>();
    //        // Rect transform set to pivot in the middle, whereas unity Rect uses top-left corner, hence modifying position with width/height.
    //        return new Rect(
    //            btnRectTrans.position.x - ((btnRectTrans.lossyScale.x * btnRectTrans.rect.width) * 0.5f),
    //            btnRectTrans.position.y - ((btnRectTrans.lossyScale.y * btnRectTrans.rect.height) * 0.5f),
    //            btnRectTrans.lossyScale.x * btnRectTrans.rect.width,
    //            btnRectTrans.lossyScale.y * btnRectTrans.rect.height
    //        );
    //    }
    //    else
    //        return obj.GetComponent<RectTransform>().rect;
    //}
}
