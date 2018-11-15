using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ZeroProgress.Common;
using ZeroProgress.Common.Collections;

public class MouseTracker : MonoBehaviour {

    private RefShape2DMouseTrackerSet mouseTrackerFullSet;
    private RefShape2DMouseTrackerSet mouseTrackerEnteredSet;
    private RefShape2DMouseTrackerSet mouseTrackerLinkedSet;

    delegate void MouseEventsDel(Vector2 mousePos);
    MouseEventsDel MouseMoveCalls;
    MouseEventsDel MouseDownCalls;
    MouseEventsDel MouseUpCalls;

    private ScriptableVector2 mousePos;
    private Vector2 prevPos;

    private void Awake() {
        mousePos = AssetDatabase.LoadAssetAtPath<ScriptableVector2>("Assets/_ManOverboard/Variables/v2_mouseWorldPos.asset");
        
        Vector3 mouseCalcPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
        prevPos = mousePos.CurrentValue = new Vector2(mouseCalcPos.x, mouseCalcPos.y);

        mouseTrackerFullSet = AssetDatabase.LoadAssetAtPath<RefShape2DMouseTrackerSet>("Assets/_ManOverboard/Variables/Sets/MouseTrackerSetFull.asset");
        mouseTrackerFullSet.OnItemAdded += OnItemAdded;
        mouseTrackerFullSet.OnItemRemoved += OnItemRemoved;

        mouseTrackerEnteredSet = AssetDatabase.LoadAssetAtPath<RefShape2DMouseTrackerSet>("Assets/_ManOverboard/Variables/Sets/MouseTrackerSetEntered.asset");
        mouseTrackerLinkedSet = AssetDatabase.LoadAssetAtPath<RefShape2DMouseTrackerSet>("Assets/_ManOverboard/Variables/Sets/MouseTrackerSetLinked.asset");
    }

    private void OnItemAdded(object sender, SetModifiedEventArgs<RefShape2DMouseTracker> e) {
        ResetMouseMoveDelegate();
        // ? Essentially calling mouse-enter straight away, if item was added with mouse being over-top of it.
        e.Item.MouseMoveCB(mousePos.CurrentValue);
    }
    private void OnItemRemoved(object sender, SetModifiedEventArgs<RefShape2DMouseTracker> e) {
        ResetMouseMoveDelegate();
    }

    private void ResetMouseMoveDelegate() {
        MouseMoveCalls = null;
        for (int i = 0; i < mouseTrackerFullSet.Count; i++)
            MouseMoveCalls += mouseTrackerFullSet[i].MouseMoveCB;
    }
    private void ResetMouseDownDelegate() {
        MouseDownCalls = null;
        for (int i = 0; i < mouseTrackerEnteredSet.Count; i++) {
            MouseDownCalls += mouseTrackerEnteredSet[i].MouseDownCB;
            if (mouseTrackerEnteredSet[i].clickThrough == false)
                break;
        }
    }
    private void ResetMouseUpDelegate() {
        MouseUpCalls = null;

        // Start by calling mouse up on any element that is no longer being hovered but had (and still has) it's mouse down & up calls linked
        for (int i = 0; i < mouseTrackerLinkedSet.Count; i++)
            if(mouseTrackerLinkedSet[i].LinkMouseUpToDown)
                MouseUpCalls += mouseTrackerLinkedSet[i].MouseUpCB;

        // Add the rest of the hovered mouse up calls
        for (int i = 0; i < mouseTrackerEnteredSet.Count; i++) {
            if(!mouseTrackerLinkedSet.Contains(mouseTrackerEnteredSet[i]))
                MouseUpCalls += mouseTrackerEnteredSet[i].MouseUpCB;
            if (mouseTrackerEnteredSet[i].clickThrough == false)
                break;
        }
    }


    private void Start() {
        ResetMouseMoveDelegate();
    }

    private void Update() {
        Vector3 mouseCalcPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
        mousePos.CurrentValue = new Vector2(mouseCalcPos.x, mouseCalcPos.y);

        if (prevPos != mousePos.CurrentValue) {
            prevPos = mousePos.CurrentValue;
            if (MouseMoveCalls != null)
                MouseMoveCalls(mousePos.CurrentValue);
        }

        if (Input.GetMouseButtonDown(0)) {
            ResetMouseDownDelegate();
            if (MouseDownCalls != null)
                MouseDownCalls(mousePos.CurrentValue);
        }

        if (Input.GetMouseButtonUp(0)) {
            ResetMouseUpDelegate();
            if (MouseUpCalls != null)
                MouseUpCalls(mousePos.CurrentValue);

            mouseTrackerLinkedSet.Clear();
        }

        // TODO: Delete when debugging lists no longer required
        DrawLayerMngr.Update(mouseTrackerFullSet, mouseTrackerEnteredSet);
    }
}
