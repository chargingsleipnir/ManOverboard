﻿using UnityEngine;
using UnityEditor;
using ZeroProgress.Common;

public class SpriteTossable : SpriteBase, IMouseDownDetector, IMouseUpDetector {

    public bool Held { get; protected set; }
    public bool Airborne { get; protected set; }
    protected bool paused;
    public virtual bool Paused {
        get { return paused; }
        set { paused = value; }
    }

    protected LevelManager lvlMngr;
    public LevelManager LvlMngr { set { lvlMngr = value; } }

    private SpriteTossableSet set;
    protected ScriptableVector2 mousePos;

    protected Rigidbody2D rb;
    protected BoxCollider2D bc;

    protected bool mouseDown_Unmoved;

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
        Airborne = false;
        mouseDown_Unmoved = false;
    }

    public virtual void MouseDownCB() {
        if (CheckImmExit() || Airborne || selectable)
            return;

        mouseDown_Unmoved = true;

        lvlMngr.OnSpriteMouseDown(gameObject);
    }
    public virtual void OnClick() {}

    public void MouseUpCB() {
        mouseDown_Unmoved = false;

        if (CheckImmExit() || Airborne)
            return;

        if (selectable)
            lvlMngr.OnSelection(this);
    }

    // Airborne not included here because derived class "Update" methods need to check for it.
    protected virtual bool CheckImmExit() {
        return Paused;
    }

    public void MoveRigidbody() {
        rb.MovePosition(mousePos.CurrentValue);
    }


    // TODO: Flesh this out - remove from everything it could be wasting calculations with
    public virtual void Toss(Vector2 vel) {
        set.Remove(this);

        // Move in front of all other non-water objects
        SortCompLayerChange(Consts.DrawLayers.BehindWater, null);
        rb.isKinematic = false;
        rb.velocity = vel;
        bc.enabled = true;
        gameObject.layer = (int)Consts.UnityLayers.TossedObj;
        Airborne = true;

        RefShape2DMouseTracker[] trackers = GetComponents<RefShape2DMouseTracker>();
        for (int i = 0; i < trackers.Length; i++)
            trackers[i].enabled = false;
    }

    protected void StopVel() {
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
    }

    public override void HighlightToSelect() {
        base.HighlightToSelect();
        Paused = false;
    }

    public void ReturnToBoat() {
        SortCompResetToBase();
    }
}
