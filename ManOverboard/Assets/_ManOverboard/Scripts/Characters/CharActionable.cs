using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;

public class CharActionable : CharBase {
    [SerializeField]
    protected GameObject actionBtnObj;
    [SerializeField]
    protected GameObject commandPanel;

    protected bool GUIActive;
    protected Rect actionBtnRect;
    protected Rect cmdPanelRect;

    protected override void Awake() {
        base.Awake();
        actionBtnRect = GetCanvasElemRect(actionBtnObj, true);
    }

    public override void ApplyTransformToContArea(GameObject contAreaObj) {
        actionBtnRect = GetCanvasElemRect(actionBtnObj, true);
        float btnTopToCharTop = actionBtnRect.height > 0 ? actionBtnRect.yMax - (transform.position.y + (sr.size.y * 0.5f)) : 0;
        contAreaObj.transform.position = new Vector3(transform.position.x, transform.position.y + (btnTopToCharTop * 0.5f), (float)Consts.ZLayers.Front + 0.1f);
        contAreaObj.transform.localScale = new Vector3(Utility.GreaterOf(sr.size.x, actionBtnRect.width) + CONT_AREA_BUFFER, sr.size.y + btnTopToCharTop + CONT_AREA_BUFFER, 1);
    }

    public override void SetActionBtnActive(bool isActive) {
        actionBtnObj.SetActive(isActive);
    }

    public override void SetCommandBtnsActive(bool isActive) {
        commandPanel.SetActive(isActive);
    }

    public override bool CmdPanelHovered() {
        cmdPanelRect = GetCanvasElemRect(commandPanel, true);
        return cmdPanelRect.Contains(mousePos);
    }

    public override bool GetMenuOpen() {
        return commandPanel.activeSelf;
    }

    protected override void OnMouseDown() {
        if (saved || tossed)
            return;

        actionBtnObj.SetActive(true);
        commandPanel.SetActive(false);

        charMouseDownEvent.RaiseEvent(gameObject);
    }

    protected override void OnMouseUp() {
        if (saved || tossed)
            return;

        actionBtnObj.SetActive(false);
        if (actionBtnRect.Contains(mousePos.Value))
            commandPanel.SetActive(true);

        charMouseUpEvent.RaiseEvent();
    }




    // Internal/helper methods ==========================================

    protected Rect GetCanvasElemRect(GameObject obj, bool scaledValues) {
        if (scaledValues) {
            RectTransform btnRectTrans = obj.GetComponent<RectTransform>();
            // Rect transform set to pivot in the middle, whereas unity Rect uses top-left corner, hence modifying position with width/height.
            return new Rect(
                btnRectTrans.position.x - ((btnRectTrans.lossyScale.x * btnRectTrans.rect.width) * 0.5f),
                btnRectTrans.position.y - ((btnRectTrans.lossyScale.y * btnRectTrans.rect.height) * 0.5f),
                btnRectTrans.lossyScale.x * btnRectTrans.rect.width,
                btnRectTrans.lossyScale.y * btnRectTrans.rect.height
            );
        }
        else
            return obj.GetComponent<RectTransform>().rect;
    }
}
