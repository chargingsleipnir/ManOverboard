using UnityEngine;
using UnityEditor;
using ZeroProgress.Common;

public class SpriteTossable : SpriteBase, IMouseDownDetector, IMouseUpDetector {

    private bool tossed;

    protected LevelManager lvlMngr;
    public LevelManager LvlMngr { set { lvlMngr = value; } }

    private SpriteTossableSet set;
    protected ScriptableVector2 mousePos;

    protected Rigidbody2D rb;
    protected BoxCollider2D bc;

    public bool Paused { get; set; }

    [SerializeField]
    protected int weight;
    public virtual int Weight {
        get { return weight; }
        set { weight = value; }
    }

    protected override void Awake() {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();

        mousePos = Resources.Load<ScriptableVector2>("ScriptableObjects/v2_mouseWorldPos");
        set = Resources.Load<SpriteTossableSet>("ScriptableObjects/SpriteSets/SpriteTossableSet");
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
        tossed = false;
    }

    public virtual void MouseDownCB() {
        if (CheckImmExit())
            return;

        lvlMngr.OnSpriteMouseDown(gameObject);
    }
    public virtual void OnClick() {}

    public void MouseUpCB() {
        if (CheckImmExit())
            return;

        if (selectable)
            lvlMngr.OnSelection(this);
    }

    protected virtual bool CheckImmExit() {
        return tossed || Paused;
    }

    public void MoveRigidbody() {
        rb.MovePosition(mousePos.CurrentValue);
    }


    // TODO: Flesh this out - remove from everything it could be wasting calculations with
    public virtual void Toss(Vector2 vel) {
        set.Remove(this);

        tossed = true;

        // Move in front of all other non-water objects
        SortCompLayerChange(Consts.DrawLayers.BehindWater, null);
        rb.isKinematic = false;
        rb.velocity = vel;
        bc.enabled = true;
        gameObject.layer = 9;

        RefShape2DMouseTracker[] trackers = GetComponents<RefShape2DMouseTracker>();
        for (int i = 0; i < trackers.Length; i++)
            trackers[i].enabled = false;
    }

    public override void HighlightToSelect() {
        base.HighlightToSelect();
        Paused = false;
    }

    public void ReturnToBoat() {
        SortCompResetToBase();
    }
}
