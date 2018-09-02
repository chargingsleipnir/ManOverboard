using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharActionable : CharBase {
    [SerializeField]
    protected GameObject actionBtnObj;
    [SerializeField]
    protected GameObject[] cmdBtns;

    protected bool GUIActive;

    public override void SetActionBtnActive(bool isActive) {
        actionBtnObj.SetActive(isActive);
    }

    public override void SetCommandBtnsActive(bool isActive) {
        for(int i = 0; i < cmdBtns.Length; i++) {
            cmdBtns[i].SetActive(isActive);
        }
    }

    public override Rect GetActionBtnRect(bool scaledValues) {
        if (scaledValues) {
            RectTransform btnRectTrans = actionBtnObj.GetComponent<RectTransform>();
            // Rect transform set to pivot in the middle, whereas unity Rect uses top-left corner, hence modifying position with width/height.
            return new Rect(
                btnRectTrans.position.x - ((btnRectTrans.lossyScale.x * btnRectTrans.rect.width) * 0.5f),
                btnRectTrans.position.y - ((btnRectTrans.lossyScale.y * btnRectTrans.rect.height) * 0.5f),
                btnRectTrans.lossyScale.x * btnRectTrans.rect.width,
                btnRectTrans.lossyScale.y * btnRectTrans.rect.height
            );
        }
        else
            return actionBtnObj.GetComponent<RectTransform>().rect;
    }
}
