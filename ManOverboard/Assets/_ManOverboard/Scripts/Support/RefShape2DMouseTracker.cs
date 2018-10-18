using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using ZeroProgress.Common;
using ZeroProgress.Common.Collections;

[RequireComponent(typeof(SpriteBase))]
public class RefShape2DMouseTracker : MonoBehaviour {

    [SerializeField]
    private RefShape refShape;
    public bool trackThrough;

    private bool lateStartLaunched;

    // Holding reference to SpriteBase for easy access to each object's draw layer
    public SpriteBase SB { get; private set; }

    // TODO: Specify component type - WAY too many conversions happening
    private ComponentSet mouseTrackerFullSet;
    private ComponentSet mouseTrackerEnteredSet;

    public bool containsMousePoint;

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
        SB = GetComponent<SpriteBase>();
        lateStartLaunched = false;

        if (refShape == null)
            return;

        mouseTrackerFullSet = AssetDatabase.LoadAssetAtPath<ComponentSet>("Assets/_ManOverboard/Variables/Sets/MouseTrackerFullSet.asset");
        mouseTrackerEnteredSet = AssetDatabase.LoadAssetAtPath<ComponentSet>("Assets/_ManOverboard/Variables/Sets/MouseTrackerEnteredSet.asset");
    }

    private void OnEnable() {
        if (refShape == null || lateStartLaunched == false)
            return;

        AddToSet();
    }

    private void Start() {
        if (refShape == null)
            return;

        mouseDownScripts = GetComponents<IMouseDownDetector>();
        mouseUpScripts = GetComponents<IMouseUpDetector>();
        mouseEnterScripts = GetComponents<IMouseEnterDetector>();
        mouseExitScripts = GetComponents<IMouseExitDetector>();

        // * Absolutely necessary to have this short delay
        StartCoroutine(LateStart());

        mouseDownWithinBounds = false;
    }

    private IEnumerator LateStart() {
        yield return 0;
        lateStartLaunched = true;
        OnEnable();
    }

    private void OnDisable() {
        if (refShape == null)
            return;

        RemoveFromSet();
    }

    private void AddToSet() {
        if (mouseTrackerFullSet.Contains(this))
            return;

        int insertionPnt = -1;
        for (int i = 0; i < mouseTrackerFullSet.Count; i++) {
            if (SB.IsCloserThan((mouseTrackerFullSet[i] as RefShape2DMouseTracker).SB)) {
                insertionPnt = i;
                break;
            }
        }
        if (insertionPnt > -1) {
            mouseTrackerFullSet.Insert(insertionPnt, this);

            // TODO - Maybe find a more direct/quicker approach to correcting this set.
            if (containsMousePoint)
                RedoEnteredSet();
        }
        else {
            mouseTrackerFullSet.Add(this);
            if (containsMousePoint)
                mouseTrackerEnteredSet.Add(this);
        }


        //for (int i = 0; i < refShapeMouseTrackerSet.Count; i++)
        //    Debug.Log(refShapeMouseTrackerSet[i]);
    }
    private void AddToSet(List<Component> range) {
        int insertionPnt = -1;
        for (int i = 0; i < mouseTrackerFullSet.Count; i++) {
            if ((range[0] as RefShape2DMouseTracker).SB.IsCloserThan((mouseTrackerFullSet[i] as RefShape2DMouseTracker).SB)) {
                insertionPnt = i;
                break;
            }
        }
        if (insertionPnt > -1)
            mouseTrackerFullSet.InsertRange(insertionPnt, range);
        else
            mouseTrackerFullSet.AddRange(range);

        RedoEnteredSet();
    }
    public void RemoveFromSet() {
        mouseTrackerFullSet.Remove(this);
        mouseTrackerEnteredSet.Remove(this);
    }

    public void RepositionInTrackerSet() {
        // TODO: Need to modify this, for not just this item, but any children also effected
        int currIdx = mouseTrackerFullSet.IndexOf(this);
        List<Component> transferSet = new List<Component>();

        for (int i = 0; i < mouseTrackerFullSet.Count; i++) {
            if (mouseTrackerFullSet[i] == this) {
                transferSet.Add(this);
            }
            else if ((mouseTrackerFullSet[i] as RefShape2DMouseTracker).SB.IsChildOf(SB)) {
                transferSet.Add(mouseTrackerFullSet[i]);
            }
            // ? I should be able to stop check at the first instance of a non-child found. How could a non-child be closer than a child? Shouldn't be possible
            else {
                if(transferSet.Count > 0)
                    break;
            }
        }

        if (transferSet.Count == 1) {
            RemoveFromSet();
            AddToSet();
        }
        else if (transferSet.Count > 1) {
            mouseTrackerFullSet.RemoveRangeFromSet(transferSet);
            mouseTrackerEnteredSet.RemoveRangeFromSet(transferSet);
            AddToSet(transferSet);
        }
    }

    private void RedoEnteredSet() {
        mouseTrackerEnteredSet.Clear();
        for (int i = 0; i < mouseTrackerFullSet.Count; i++) {
            if ((mouseTrackerFullSet[i] as RefShape2DMouseTracker).containsMousePoint)
                mouseTrackerEnteredSet.Add(mouseTrackerFullSet[i]);
        }
    }


    public void MouseDownCB(Vector2 mousePos) {
        if (refShape.ContainsPoint(mousePos)) {
            if (linkMouseUpToDown)
                mouseDownWithinBounds = true;

            for(int i = 0; i < mouseDownScripts.Length; i++)
                mouseDownScripts[i].MouseDownCB();

            mouseDownEvent.Invoke();
        }
    }

    public void MouseUpCB(Vector2 mousePos) {
        if (linkMouseUpToDown) {
            // even if option is checked, the mouse being clicked and dragged into these bounds should still do mouse up,
            // even without the down connection
            if (mouseDownWithinBounds) {
                mouseDownWithinBounds = false;
                for (int i = 0; i < mouseUpScripts.Length; i++)
                    mouseUpScripts[i].MouseUpCB();

                mouseUpEvent.Invoke();
                return;
            }
        }
        mouseDownWithinBounds = false;

        if (refShape.ContainsPoint(mousePos)) {
            for (int i = 0; i < mouseUpScripts.Length; i++)
                mouseUpScripts[i].MouseUpCB();

            mouseUpEvent.Invoke();
        }
    }

    public void MouseMoveCB(Vector2 mousePos) {
        if (containsMousePoint) {
            if (!refShape.ContainsPoint(mousePos)) {
                for (int i = 0; i < mouseExitScripts.Length; i++)
                    mouseExitScripts[i].MouseExitCB();

                mouseExitEvent.Invoke();
                containsMousePoint = false;
                mouseTrackerEnteredSet.Remove(this);
            }
        }
        else {
            if (refShape.ContainsPoint(mousePos)) {
                for (int i = 0; i < mouseEnterScripts.Length; i++)
                    mouseEnterScripts[i].MouseEnterCB();

                mouseEnterEvent.Invoke();
                containsMousePoint = true;
                // TODO: Find a more efficient way to add "this" to entered list at this point
                RedoEnteredSet();
            }
        }
    }
}

[CustomEditor(typeof(RefShape2DMouseTracker))]
[CanEditMultipleObjects]
public class RefShape2DMouseTrackerEditor : Editor {

    SerializedProperty refShape;
    SerializedProperty trackThrough;
    SerializedProperty linkMouseUpToDown;

    SerializedProperty mouseDownEvent;
    SerializedProperty mouseUpEvent;
    SerializedProperty mouseEnterEvent;
    SerializedProperty mouseExitEvent;

    bool useEvents;

    private void OnEnable() {
        refShape = serializedObject.FindProperty("refShape");
        trackThrough = serializedObject.FindProperty("trackThrough");
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
        EditorGUILayout.PropertyField(trackThrough);
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