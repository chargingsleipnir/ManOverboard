using UnityEngine;
using UnityEditor;
using ZeroProgress.Common;

public class SpriteTossable : SpriteBase, IMouseDownDetector, IMouseUpDetector {

    public delegate void DelPassGO(GameObject obj);
    DelPassGO OnMouseDownCB;

    public delegate void DelVoid();
    DelVoid OnMouseUpCB;

    private SpriteTossableSet set;
    [SerializeField]
    protected ScriptableVector2 mousePos;

    protected bool held;
    protected bool tossed;

    protected Rigidbody2D rb;
    protected BoxCollider2D bc;

    [SerializeField]
    protected int weight;
    public int Weight {
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
        base.Start();

        // Rigidbody is essentially inactive until being flung/thrown by user.
        // (Could use posiiton constraints instead)
        rb.isKinematic = true;
        bc.enabled = false;

        held = false;
        tossed = false;
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

    public virtual void ApplyTransformToContArea(GameObject contAreaObj) {
        contAreaObj.transform.position = new Vector3(transform.position.x, transform.position.y, (float)Consts.ZLayers.FrontOfWater);
        contAreaObj.transform.localScale = new Vector3(srRef.comp.size.x + Consts.CONT_AREA_BUFFER, srRef.comp.size.y + Consts.CONT_AREA_BUFFER, 1);
    }

    public void MoveRigidbody() {
        rb.MovePosition(mousePos.CurrentValue);
    }

    public void Toss(Vector2 vel) {
        // Move in front of all other non-water objects
        SortCompLayerChange(Consts.DrawLayers.BehindWater, null);
        tossed = true;
        rb.isKinematic = false;
        rb.velocity = vel;
        bc.enabled = true;
    }

    public void ReturnToBoat() {
        SortCompFullReset();
    }

    public virtual void RemoveFromSet() {
        set.Remove(this);
    }

    // Virtual function here for child use only -----------------------------------------------

    public virtual void OverheadButtonActive(bool isActive) { }
}
