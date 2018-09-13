using UnityEngine;
using ZeroProgress.Common;

public class MouseTracker : MonoBehaviour {

    [SerializeField]
    private Vector2ParamEvent mouseMoveEventOut;
    [SerializeField]
    private Vector2ParamEvent mouseDownEventOut;
    [SerializeField]
    private Vector2ParamEvent mouseUpEventOut;

    [SerializeField]
    private Vector2Reference mousePos;
    private Vector2 prevPos;

    private void Awake() {
        Vector3 mouseCalcPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
        prevPos = mousePos.Value = new Vector2(mouseCalcPos.x, mouseCalcPos.y);
    }

    private void Update() {
        Vector3 mouseCalcPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
        mousePos.Value = new Vector2(mouseCalcPos.x, mouseCalcPos.y);

        if (prevPos != mousePos.Value) {
            prevPos = mousePos.Value;
            mouseMoveEventOut.RaiseEvent(mousePos.Value);
        }

        if (Input.GetMouseButtonDown(0))
            mouseDownEventOut.RaiseEvent(mousePos.Value);

        if (Input.GetMouseButtonUp(0))
            mouseUpEventOut.RaiseEvent(mousePos.Value);
    }
}
