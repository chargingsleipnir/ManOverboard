using UnityEngine;
using UnityEditor;
using ZeroProgress.Common;

public class SpriteTossable : SpriteBase, IMouseDownDetector, IMouseUpDetector {

    public delegate void DelPassGO(GameObject obj);
    protected DelPassGO OnMouseDownCB;

    public delegate void DelVoid();
    protected DelVoid OnMouseUpCB;

    private SpriteTossableSet set;
    protected ScriptableVector2 mousePos;

    protected bool held;
    protected bool tossed;

    protected Rigidbody2D rb;
    protected BoxCollider2D bc;

    [SerializeField]
    protected int weight;
    public virtual int Weight {
        get { return weight; }
    }

    protected override void Awake() {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();

        set = AssetDatabase.LoadAssetAtPath<SpriteTossableSet>("Assets/_ManOverboard/Variables/Sets/SpriteTossableSet.asset");
        mousePos = AssetDatabase.LoadAssetAtPath<ScriptableVector2>("Assets/_ManOverboard/Variables/v2_mouseWorldPos.asset");

        set.Add(this);
    }

    protected override void Start() {
        Reset();
    }

    protected override void Reset() {
        base.Reset();

        // Rigidbody is essentially inactive until being flung/thrown by user.
        // (Could use posiiton constraints instead)
        rb.isKinematic = true;
        bc.enabled = false;

        held = false;
        tossed = false;
    }

    public void ChangeMouseUpToDownLinks(bool linkEvents) {
        RefShape2DMouseTracker[] trackers = GetComponents<RefShape2DMouseTracker>();
        for (int i = 0; i < trackers.Length; i++)
            trackers[i].LinkMouseUpToDown = linkEvents;
    }

    public virtual void MouseDownCB() {
        if (tossed)
            return;

        held = true;
        OnMouseDownCB(gameObject);
    }
    public virtual void MouseUpCB() {
        if (tossed || held == false)
            return;

        held = false;
        OnMouseUpCB();
    }

    public void SetMouseRespCallbacks(DelPassGO OnMouseDownCB, DelVoid OnMouseUpCB) {
        this.OnMouseDownCB = OnMouseDownCB;
        this.OnMouseUpCB = OnMouseUpCB;
    }

    public virtual void ApplyTransformToContArea(GameObject contAreaObj, bool prioritizeRefShape) {
        contAreaObj.transform.position = new Vector3(transform.position.x, transform.position.y, (float)Consts.ZLayers.FrontOfWater);

        if(prioritizeRefShape) {
            RefShape shape = GetComponent<RefShape>();
            if (shape != null) {
                contAreaObj.transform.localScale = new Vector3(shape.Width + Consts.CONT_AREA_BUFFER, shape.Height + Consts.CONT_AREA_BUFFER, 1);
                return;
            }
        }
        contAreaObj.transform.localScale = new Vector3(srRef.comp.sprite.bounds.size.x + Consts.CONT_AREA_BUFFER, srRef.comp.sprite.bounds.size.y + Consts.CONT_AREA_BUFFER, 1);
    }

    public void MoveRigidbody() {
        rb.MovePosition(mousePos.CurrentValue);
    }


    // TODO: Flesh this out - remove from everything it could be wasting calculations with
    public virtual void Toss(Vector2 vel) {
        set.Remove(this);

        // Move in front of all other non-water objects
        SortCompLayerChange(Consts.DrawLayers.BehindWater, null);
        tossed = true;
        rb.isKinematic = false;
        rb.velocity = vel;
        bc.enabled = true;

        RefShape2DMouseTracker[] trackers = GetComponents<RefShape2DMouseTracker>();
        for (int i = 0; i < trackers.Length; i++)
            trackers[i].enabled = false;
    }

    public void ReturnToBoat() {
        SortCompFullReset();
    }

    // Virtual function here for child use only -----------------------------------------------

    public virtual void OverheadButtonActive(bool isActive) { }
}
