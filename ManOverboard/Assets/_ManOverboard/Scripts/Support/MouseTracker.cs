using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ZeroProgress.Common;
using ZeroProgress.Common.Collections;

public class MouseTracker : MonoBehaviour {

    // TODO: Specify component type - WAY too many conversions happening
    private ComponentSet mouseTrackerFullSet;
    private ComponentSet mouseTrackerEnteredSet;

    delegate void MouseEventsDel(Vector2 mousePos);
    MouseEventsDel MouseMoveCalls;
    MouseEventsDel MouseDownCalls;
    MouseEventsDel MouseUpCalls;

    [SerializeField]
    private Vector2Reference mousePos;
    private Vector2 prevPos;

    private void Awake() {
        Vector3 mouseCalcPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
        prevPos = mousePos.Value = new Vector2(mouseCalcPos.x, mouseCalcPos.y);

        mouseTrackerFullSet = AssetDatabase.LoadAssetAtPath<ComponentSet>("Assets/_ManOverboard/Variables/Sets/MouseTrackerFullSet.asset");
        mouseTrackerFullSet.OnItemAdded += OnItemAdded;
        mouseTrackerFullSet.OnItemRemoved += OnItemRemoved;

        mouseTrackerEnteredSet = AssetDatabase.LoadAssetAtPath<ComponentSet>("Assets/_ManOverboard/Variables/Sets/MouseTrackerEnteredSet.asset");
    }

    private void OnItemAdded(object sender, SetModifiedEventArgs<Component> e) {
        RedoMouseMoveDelegate();
        // ? Essentially calling mouse-enter straight away, if item was adde with mouse being over-top of it.
        //(e.Item as RefShape2DMouseTracker).MouseMoveCB(mousePos.Value);
    }
    private void OnItemRemoved(object sender, SetModifiedEventArgs<Component> e) {
        RedoMouseMoveDelegate();
    }

    private void RedoMouseMoveDelegate() {
        MouseMoveCalls = null;
        for (int i = 0; i < mouseTrackerFullSet.Count; i++)
            MouseMoveCalls += (mouseTrackerFullSet[i] as RefShape2DMouseTracker).MouseMoveCB;
    }
    private void RedoMouseDownDelegate() {
        MouseDownCalls = null;
        for (int i = 0; i < mouseTrackerEnteredSet.Count; i++) {
            MouseDownCalls += (mouseTrackerEnteredSet[i] as RefShape2DMouseTracker).MouseDownCB;
            if ((mouseTrackerEnteredSet[i] as RefShape2DMouseTracker).trackThrough == false)
                break;
        }
    }
    private void RedoMouseUpDelegate() {
        MouseUpCalls = null;
        for (int i = 0; i < mouseTrackerEnteredSet.Count; i++) {
            MouseUpCalls += (mouseTrackerEnteredSet[i] as RefShape2DMouseTracker).MouseUpCB;
            if ((mouseTrackerEnteredSet[i] as RefShape2DMouseTracker).trackThrough == false)
                break;
        }
    }


    private void Start() {
        RedoMouseMoveDelegate();
    }

    private void Update() {
        Vector3 mouseCalcPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
        mousePos.Value = new Vector2(mouseCalcPos.x, mouseCalcPos.y);

        if (prevPos != mousePos.Value) {
            prevPos = mousePos.Value;
            if (MouseMoveCalls != null) {
                MouseMoveCalls(mousePos.Value);
            }
        }

        if (Input.GetMouseButtonDown(0)) {
            RedoMouseDownDelegate();
            if (MouseDownCalls != null) {
                MouseDownCalls(mousePos.Value);
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            RedoMouseUpDelegate();
            if (MouseUpCalls != null)
                MouseUpCalls(mousePos.Value);
        }

        // TODO: Delete when debugging lists no longer required
        DrawLayerMngr.Update(mouseTrackerFullSet, mouseTrackerEnteredSet);
    }
}
