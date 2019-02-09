using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ZeroProgress.Common.Collections;

public class Crewman : CharAdult {

    [SerializeField]
    protected ItemBase sailorCap;

    Vector3 capPosLocal;
    bool capDonned;

    protected override void Awake() {
        capPosLocal = sailorCap.transform.localPosition;
        capDonned = true;
        base.Awake();
    }

    protected override void Start() {
        strength += 25;
        speed += 25;
        Reset();

        sailorCap.CharHeldBy = this;
        WearItem(sailorCap);
    }

    //public override void SetActionBtns() {

    //}

    //public override void CheckActions() {

    //}

    protected override void OnSelectionScoop(SpriteBase sprite) {
        if ((sprite as ItemBase) == sailorCap) {
            capDonned = false;
        }

        base.OnSelectionScoop(sprite);
    }

    public override void LoseItem(ItemBase item) {
        // Only way to specify this item with precision right now
        if (item.name.Contains("SailorCap")) {
            capDonned = false;
        }

        base.LoseItem(item);
    }

    public override void DropItemHeld() {
        if (itemHeld == null)
            return;

        if (itemHeld.name.Contains("SailorCap")) {
            if (!capDonned) {
                sailorCap = itemHeld;
                sailorCap.SortCompResetToBase(transform);
                sailorCap.transform.localPosition = capPosLocal;
                sailorCap.CharHeldBy = this;
                itemsWorn.Add(sailorCap);
                capDonned = true;
                sailorCap.EnableMouseTracking(true);
                lvlMngr.OnDeselection(sailorCap);
                sailorCap.InUse = false;
                itemHeld = null;
                return;
            }
        }

        base.DropItemHeld();
    }

    //private void ClickThroughHat(bool clickThrough) {
    //    RefShape2DMouseTracker[] trackers = sailorHat.GetComponents<RefShape2DMouseTracker>();
    //    for (int i = 0; i < trackers.Length; i++)
    //        trackers[i].clickThrough = clickThrough;
    //}

    // JOB SPECIFIC ACTIONS (just examples for now)

    public void LowerAnchor() {

    }
    public void ReleaseAnchor() {

    }
    public void RaiseSail() {

    }
}
