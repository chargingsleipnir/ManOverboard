using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteBase))]
public class RefShape2DMouseTracker : MonoBehaviour {

    [SerializeField]
    private RefShape refShape;
    public bool clickThrough;

    private bool lateStartLaunched;

    // Holding reference to SpriteBase for easy access to each object's draw layer
    public SpriteBase SB { get; private set; }

    private RefShape2DMouseTrackerSet mouseTrackerFullSet;
    private RefShape2DMouseTrackerSet mouseTrackerEnteredSet;
    private RefShape2DMouseTrackerSet mouseTrackerLinkedSet;

    delegate void MouseEventsDel();
    MouseEventsDel MouseMoveCalls;
    MouseEventsDel MouseEnterCalls;
    MouseEventsDel MouseExitCalls;
    MouseEventsDel MouseDownCalls;
    MouseEventsDel MouseUpCalls;

    public bool containsMousePoint;

    [SerializeField]
    private bool linkMouseUpToDown;
    public bool LinkMouseUpToDown {
        get { return linkMouseUpToDown; }
        set { linkMouseUpToDown = value; }
    }
    private bool origLinkState;

    private bool mouseDownWithinBounds;

    [SerializeField]
    private UnityEvent mouseDownEvent;
    [SerializeField]
    private UnityEvent mouseUpEvent;
    [SerializeField]
    private UnityEvent mouseMoveEvent;
    [SerializeField]
    private UnityEvent mouseEnterEvent;
    [SerializeField]
    private UnityEvent mouseExitEvent;

    private IMouseDownDetector[] mouseDownScripts;
    private IMouseUpDetector[] mouseUpScripts;
    private IMouseMoveDetector[] mouseMoveScripts;
    private IMouseEnterDetector[] mouseEnterScripts;
    private IMouseExitDetector[] mouseExitScripts;

    private void Awake() {
        SB = GetComponent<SpriteBase>();
        origLinkState = linkMouseUpToDown;

        mouseTrackerFullSet = Resources.Load<RefShape2DMouseTrackerSet>("ScriptableObjects/MouseTrackerSets/MouseTrackerSetFull");
        mouseTrackerEnteredSet = Resources.Load<RefShape2DMouseTrackerSet>("ScriptableObjects/MouseTrackerSets/MouseTrackerSetEntered");
        mouseTrackerLinkedSet = Resources.Load<RefShape2DMouseTrackerSet>("ScriptableObjects/MouseTrackerSets/MouseTrackerSetLinked");

        lateStartLaunched = false;

        if (refShape == null)
            return;
    }

    private void OnEnable() {
        if (refShape == null || lateStartLaunched == false)
            return;

        // TODO: Need to consider children/range of items
        AddToSet();
    }

    private void Start() {
        if (refShape == null)
            return;

        mouseDownScripts = GetComponents<IMouseDownDetector>();
        mouseUpScripts = GetComponents<IMouseUpDetector>();
        mouseMoveScripts = GetComponents<IMouseMoveDetector>();
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

        // TODO: Need to consider children/range of items
        RemoveFromSet();
    }

    public void SetOrigLinkState() {
        LinkMouseUpToDown = origLinkState;
    }

    private void AddToSet() {
        if (mouseTrackerFullSet.Contains(this))
            return;

        int insertionPnt = -1;
        for (int i = 0; i < mouseTrackerFullSet.Count; i++) {
            if (SB.IsCloserThan(mouseTrackerFullSet[i].SB)) {
                insertionPnt = i;
                break;
            }
        }
        if (insertionPnt > -1) {
            mouseTrackerFullSet.Insert(insertionPnt, this);

            // TODO - Maybe find a more direct/quicker approach to correcting this set.
            if (containsMousePoint)
                ResetEnteredSet();
        }
        else {
            mouseTrackerFullSet.Add(this);
            if (containsMousePoint)
                mouseTrackerEnteredSet.Add(this);
        }


        //for (int i = 0; i < refShapeMouseTrackerSet.Count; i++)
        //    Debug.Log(refShapeMouseTrackerSet[i]);
    }
    private void AddToSet(List<RefShape2DMouseTracker> range) {
        int insertionPnt = -1;
        for (int i = 0; i < mouseTrackerFullSet.Count; i++) {
            if (range[0].SB.IsCloserThan(mouseTrackerFullSet[i].SB)) {
                insertionPnt = i;
                break;
            }
        }
        if (insertionPnt > -1)
            mouseTrackerFullSet.InsertRange(insertionPnt, range);
        else
            mouseTrackerFullSet.AddRange(range);

        ResetEnteredSet();
    }
    public void RemoveFromSet() {
        mouseTrackerFullSet.Remove(this);
        mouseTrackerEnteredSet.Remove(this);
    }

    public void RepositionInTrackerSet() {
        List<RefShape2DMouseTracker> transferSet = new List<RefShape2DMouseTracker>();

        for (int i = 0; i < mouseTrackerFullSet.Count; i++) {
            if (mouseTrackerFullSet[i] == this) {
                transferSet.Add(this);
            }
            else if (mouseTrackerFullSet[i].SB.IsChildOf(SB)) {
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

    public void AddMouseUpListener(UnityAction eventCB) {
        mouseUpEvent.AddListener(eventCB);
    }
    public void AddMouseEnterListener(UnityAction eventCB) {
        mouseEnterEvent.AddListener(eventCB);
    }
    public void AddMouseExitListener(UnityAction eventCB) {
        mouseExitEvent.AddListener(eventCB);
    }

    private void ResetEnteredSet() {
        mouseTrackerEnteredSet.Clear();
        for (int i = 0; i < mouseTrackerFullSet.Count; i++) {
            if (mouseTrackerFullSet[i].containsMousePoint)
                mouseTrackerEnteredSet.Add(mouseTrackerFullSet[i]);
        }
    }

    private void UseMouseMoveDelegate() {
        MouseMoveCalls = null;
        for (int i = 0; i < mouseMoveScripts.Length; i++)
            MouseMoveCalls += mouseMoveScripts[i].MouseMoveCB;

        if (MouseMoveCalls != null)
            MouseMoveCalls();
    }
    private void UseMouseEnterDelegate() {
        MouseEnterCalls = null;
        for (int i = 0; i < mouseEnterScripts.Length; i++)
            MouseEnterCalls += mouseEnterScripts[i].MouseEnterCB;

        if(MouseEnterCalls != null)
            MouseEnterCalls();
    }
    private void UseMouseExitDelegate() {
        MouseExitCalls = null;
        for (int i = 0; i < mouseExitScripts.Length; i++)
            MouseExitCalls += mouseExitScripts[i].MouseExitCB;

        if (MouseExitCalls != null)
            MouseExitCalls();
    }
    private void UseMouseDownDelegate() {
        MouseDownCalls = null;
        for (int i = 0; i < mouseDownScripts.Length; i++)
            MouseDownCalls += mouseDownScripts[i].MouseDownCB;

        if (MouseDownCalls != null)
            MouseDownCalls();
    }
    private void UseMouseUpDelegate() {
        MouseUpCalls = null;
        for (int i = 0; i < mouseUpScripts.Length; i++)
            MouseUpCalls += mouseUpScripts[i].MouseUpCB;

        if (MouseUpCalls != null)
            MouseUpCalls();
    }

    public void MouseMoveCB(Vector2 mousePos) {
        // Double conditions switching containsMousePoint boolean to catch enter and exit. Within those, mouseMove is launched, only within refShape of course.
        if (!containsMousePoint) {
            if (refShape.ContainsPoint(mousePos)) {
                UseMouseMoveDelegate();
                mouseMoveEvent.Invoke();
                UseMouseEnterDelegate();                
                mouseEnterEvent.Invoke();
                containsMousePoint = true;
                ResetEnteredSet(); // TODO: Find a more efficient way to add "this" to entered list at this point
            }
        }
        else {
            if (refShape.ContainsPoint(mousePos)) {
                UseMouseMoveDelegate();
                mouseMoveEvent.Invoke();
            }
            else {
                UseMouseExitDelegate();
                mouseExitEvent.Invoke();
                containsMousePoint = false;
                mouseTrackerEnteredSet.Remove(this);
            }
        }
    }

    public void MouseDownCB(Vector2 mousePos) {
        if (refShape.ContainsPoint(mousePos)) {
            if (linkMouseUpToDown) {
                mouseTrackerLinkedSet.Add(this);
                mouseDownWithinBounds = true;
            }

            UseMouseDownDelegate();
            mouseDownEvent.Invoke();
        }
    }

    public void MouseUpCB(Vector2 mousePos) {
        if (linkMouseUpToDown) {
            // even if option is checked, the mouse being clicked and dragged into these bounds should still do mouse up,
            // even without the down connection
            if (mouseDownWithinBounds) {
                mouseDownWithinBounds = false;

                UseMouseUpDelegate();
                mouseUpEvent.Invoke();
                return;
            }
        }
        mouseDownWithinBounds = false;

        if (refShape.ContainsPoint(mousePos)) {
            UseMouseUpDelegate();
            mouseUpEvent.Invoke();
        }
    }
}