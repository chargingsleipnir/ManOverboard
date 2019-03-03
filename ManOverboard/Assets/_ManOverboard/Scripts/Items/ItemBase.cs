using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ZeroProgress.Common;

public class ItemBase : SpriteTossable {

    public CharBase CharHeldBy { get; set; }

    public bool InUse { get; set; }

    private ItemBaseSet items;

    protected override void Awake() {
        base.Awake();
        items = Resources.Load<ItemBaseSet>("ScriptableObjects/SpriteSets/ItemBaseSet");
        items.Add(this);
    }

    protected override void Start() {
        base.Start();

        InUse = false;
    }

    protected override void Update() {
        if (CheckImmExit())
            return;

        if (Airborne) {
            if (transform.position.y <= lvlMngr.WaterSurfaceYPos - Consts.OFFSCREEN_CATCH_BUFF) {
                Utility.RepositionY(transform, lvlMngr.WaterSurfaceYPos - Consts.OFFSCREEN_CATCH_BUFF);
                StopVel();
            }
        }
    }

    public override void Toss(Vector2 vel) {
        items.Remove(this);

        RemoveFromChar();

        base.Toss(vel);
    }
    public void RemoveFromChar() {
        if (CharHeldBy != null)
            CharHeldBy.LoseItem(this);
    }
    public override void MouseDownCB() {
        if (selectable)
            return;

        base.MouseDownCB();
    }

    protected override bool CheckImmExit() {
        return base.CheckImmExit() || InUse;
    }

    public override void HighlightToSelect() {
        if (InUse)
            return;

        base.HighlightToSelect();
    }

    public void Deselect() {
        SortCompResetToBase();
        EnableMouseTracking(true);
        lvlMngr.OnDeselection(this);
        InUse = false;
    }
}
