using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;
using ZeroProgress.Common.Collections;

public class MouseTracker : MonoBehaviour {

    //[SerializeField]
    //private Vector2ParamEvent mouseMoveEventOut;
    //[SerializeField]
    //private Vector2ParamEvent mouseDownEventOut;
    //[SerializeField]
    //private Vector2ParamEvent mouseUpEventOut;

    [SerializeField]
    private ComponentSet mouseTrackers;

    [SerializeField]
    private Vector2Reference mousePos;

    private void Update() {
        Vector3 mouseCalcPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
        mousePos.Value = new Vector2(mouseCalcPos.x, mouseCalcPos.y);

        //mouseMoveEventOut.RaiseEvent();

        //if (Input.GetMouseButtonDown(0))
        //    mouseDownEventOut.RaiseEvent();

        //if (Input.GetMouseButtonUp(0))
        //    mouseUpEventOut.RaiseEvent();

        //for (int i = mouseTrackers.Count - 1; i > -1; i--) {
        //    ((IMouseTrackerCBs)mouseTrackers[i]).MouseMoveCB(mousePos.Value);
        //}
        //if (Input.GetMouseButtonDown(0)) {
        //    for (int i = mouseTrackers.Count - 1; i > -1; i--) {
        //        ((IMouseTrackerCBs)mouseTrackers[i]).MouseDownCB(mousePos.Value);
        //    }
        //}
        //else if (Input.GetMouseButtonUp(0)) {
        //    for (int i = mouseTrackers.Count - 1; i > -1; i--) {
        //        ((IMouseTrackerCBs)mouseTrackers[i]).MouseUpCB(mousePos.Value);
        //    }
        //}

        for (int i = 0; i < mouseTrackers.Count; i++) {
            ((IMouseTrackerCBs)mouseTrackers[i]).MouseMoveCB(mousePos.Value);
        }
        if (Input.GetMouseButtonDown(0)) {
            for (int i = 0; i < mouseTrackers.Count; i++) {
                ((IMouseTrackerCBs)mouseTrackers[i]).MouseDownCB(mousePos.Value);
            }
        }
        else if (Input.GetMouseButtonUp(0)) {
            for (int i = 0; i < mouseTrackers.Count; i++) {
                ((IMouseTrackerCBs)mouseTrackers[i]).MouseUpCB(mousePos.Value);
            }
        }

        //foreach (IMouseTrackerCBs tracker in mouseTrackers) {
        //    tracker.MouseMoveCB(mousePos.Value);
        //}

        //if (Input.GetMouseButtonDown(0)) {
        //    foreach (IMouseTrackerCBs tracker in mouseTrackers) {
        //        tracker.MouseDownCB(mousePos.Value);
        //    }
        //}
        //else if (Input.GetMouseButtonUp(0)) {
        //    foreach (IMouseTrackerCBs tracker in mouseTrackers) {
        //        tracker.MouseUpCB(mousePos.Value);
        //    }
        //}
    }
}
