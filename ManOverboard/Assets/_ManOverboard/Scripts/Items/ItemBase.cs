using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ZeroProgress.Common;

public class ItemBase : SpriteTossable {

    public CharBase CharHeldBy { get; set; }

    public bool InUse { get; set; }

    private ItemBaseSet items;

    // Change on items that need a certain rotation, like buckets, should be facing down when held.
    public Vector2 holdDirTopOfHand = Vector2.up;

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

    public void MoveToCharHand(Transform charHandTrans) {
        transform.position = charHandTrans.position;
        transform.parent = charHandTrans.transform;

        // Rotate so item "up" matches hand "up" (Set as if pointing out the top of a closed fist)
        Vector3 dirToMatch = charHandTrans.rotation * Vector3.up;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, dirToMatch);
        // From here, rotate based so holdDirTopOfHand vector points this way.
        Vector3 dirToOrient = transform.rotation * new Vector3(holdDirTopOfHand.x, holdDirTopOfHand.y);
        transform.rotation = Quaternion.LookRotation(Vector3.forward, dirToOrient);
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
        // Set to old parent first
        SortCompResetToBase();

        // Some items scaled through animation, which then stops them from being clickable. Need to reset to avoid this.
        Utility.Scale(transform, 1, 1, 1);

        EnableMouseTracking(true);
        lvlMngr.OnDeselection(this);
        InUse = false;
    }
}
