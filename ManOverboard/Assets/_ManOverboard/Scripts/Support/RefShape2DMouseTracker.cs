using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using ZeroProgress.Common;

public class RefShape2DMouseTracker : MonoBehaviour, IGameEventListener<Vector2> {

    [SerializeField]
    private RefShape refShape;

    private Vector2ParamEvent mouseMoveEventIn;
    private Vector2ParamEvent mouseDownEventIn;
    private Vector2ParamEvent mouseUpEventIn;

    private bool containsPointCurr;

    [SerializeField]
    private bool linkMouseUpToDown;
    public bool LinkMouseUpToDown {
        get { return linkMouseUpToDown; }
        set { linkMouseUpToDown = value; }
    }
    private bool mouseDownWithinBounds;

    [SerializeField]
    private UnityEvent mouseDownEvent;
    [SerializeField]
    private UnityEvent mouseUpEvent;
    [SerializeField]
    private UnityEvent mouseEnterEvent;
    [SerializeField]
    private UnityEvent mouseExitEvent;

    private IMouseDownDetector[] mouseDownScripts;
    private IMouseUpDetector[] mouseUpScripts;
    private IMouseEnterDetector[] mouseEnterScripts;
    private IMouseExitDetector[] mouseExitScripts;

    private void Awake() {
        if (refShape == null)
            return;

        mouseMoveEventIn = AssetDatabase.LoadAssetAtPath<Vector2ParamEvent>("Assets/_ManOverboard/Events/WithParam/v2_MouseMove.asset");
        mouseDownEventIn = AssetDatabase.LoadAssetAtPath<Vector2ParamEvent>("Assets/_ManOverboard/Events/WithParam/v2_MouseDown.asset");
        mouseUpEventIn = AssetDatabase.LoadAssetAtPath<Vector2ParamEvent>("Assets/_ManOverboard/Events/WithParam/v2_MouseUp.asset");
    }

    private void Start() {
        if (refShape == null)
            return;

        mouseDownWithinBounds = false;
    }

    private void OnEnable() {
        if (refShape == null)
            return;

        mouseDownScripts = GetComponents<IMouseDownDetector>();
        mouseUpScripts = GetComponents<IMouseUpDetector>();
        mouseEnterScripts = GetComponents<IMouseEnterDetector>();
        mouseExitScripts = GetComponents<IMouseExitDetector>();

        if (mouseMoveEventIn != null)
            mouseMoveEventIn.RegisterListener(this);
        if (mouseDownEventIn != null)
            mouseDownEventIn.RegisterListener(this);
        if (mouseUpEventIn != null)
            mouseUpEventIn.RegisterListener(this);
    }

    private void OnDisable() {
        if (refShape == null)
            return;

        if (mouseMoveEventIn != null)
            mouseMoveEventIn.UnregisterListener(this);
        if (mouseDownEventIn != null)
            mouseDownEventIn.UnregisterListener(this);
        if (mouseUpEventIn != null)
            mouseUpEventIn.UnregisterListener(this);
    }

    public void OnEventRaised(string eventId) {
    }    

    public void OnEventRaised(string eventId, Vector2 mousePos) {
        if (eventId == "0")
            MouseMoveCB(mousePos);
        else if (eventId == "1")
            MouseDownCB(mousePos);
        else if (eventId == "2")
            MouseUpCB(mousePos);
    }

    public void MouseDownCB(Vector2 mousePos) {
        if (refShape.ContainsPoint(mousePos)) {
            if (linkMouseUpToDown)
                mouseDownWithinBounds = true;

            for(int i = 0; i < mouseDownScripts.Length; i++) {
                mouseDownScripts[i].MouseDownCB();
            }
            mouseDownEvent.Invoke();
        }
    }

    public void MouseUpCB(Vector2 mousePos) {
        if (linkMouseUpToDown) {
            // even if option is checked, the mouse being clicked and dragged into these bounds should still do mouse up,
            // even without the down connection
            if (mouseDownWithinBounds) {
                mouseDownWithinBounds = false;
                for (int i = 0; i < mouseUpScripts.Length; i++) {
                    mouseUpScripts[i].MouseUpCB();
                }
                mouseUpEvent.Invoke();
                return;
            }
        }
        mouseDownWithinBounds = false;

        if (refShape.ContainsPoint(mousePos)) {
            for (int i = 0; i < mouseUpScripts.Length; i++) {
                mouseUpScripts[i].MouseUpCB();
            }
            mouseUpEvent.Invoke();
        }
    }

    public void MouseMoveCB(Vector2 mousePos) {
        if (containsPointCurr) {
            if (!refShape.ContainsPoint(mousePos)) {
                for (int i = 0; i < mouseExitScripts.Length; i++) {
                    mouseExitScripts[i].MouseExitCB();
                }
                mouseExitEvent.Invoke();
                containsPointCurr = false;
            }
        }
        else {
            if (refShape.ContainsPoint(mousePos)) {
                for (int i = 0; i < mouseEnterScripts.Length; i++) {
                    mouseEnterScripts[i].MouseEnterCB();
                }
                mouseEnterEvent.Invoke();
                containsPointCurr = true;
            }
        }
    }
}

[CustomEditor(typeof(RefShape2DMouseTracker))]
[CanEditMultipleObjects]
public class RefShape2DMouseTrackerEditor : Editor {

    SerializedProperty refShape;
    SerializedProperty linkMouseUpToDown;

    SerializedProperty mouseDownEvent;
    SerializedProperty mouseUpEvent;
    SerializedProperty mouseEnterEvent;
    SerializedProperty mouseExitEvent;

    bool useEvents;

    private void OnEnable() {
        refShape = serializedObject.FindProperty("refShape");
        linkMouseUpToDown = serializedObject.FindProperty("linkMouseUpToDown");

        mouseDownEvent = serializedObject.FindProperty("mouseDownEvent");
        mouseUpEvent = serializedObject.FindProperty("mouseUpEvent");
        mouseEnterEvent = serializedObject.FindProperty("mouseEnterEvent");
        mouseExitEvent = serializedObject.FindProperty("mouseExitEvent");

        useEvents = false;
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        EditorGUILayout.LabelField("It only take 1 instance of this component to activate all IMouseDetector callbacks on this GameObject");

        EditorGUILayout.PropertyField(refShape);
 
        EditorGUILayout.PropertyField(linkMouseUpToDown);

        useEvents = EditorGUILayout.Foldout(useEvents, "Public event responses", true);
        if (useEvents) {
            EditorGUILayout.PropertyField(mouseDownEvent);
            EditorGUILayout.PropertyField(mouseUpEvent);
            EditorGUILayout.PropertyField(mouseEnterEvent);
            EditorGUILayout.PropertyField(mouseExitEvent);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
