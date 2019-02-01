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

    public override void LoseItem(ItemBase item) {

        // Only way to specify this item with precision right now
        if (item.name.Contains("SailorCap")) {
            capDonned = false;
        }

        base.LoseItem(item);
    }

    protected override void OnSelectionScoop(SpriteBase sprite) {
        if ((sprite as ItemBase) == sailorCap) {
            // Deducting the weight only because the base function > TakeAction() will add it again.
            //ItemWeight -= sailorCap.Weight;
            capDonned = false;
        }

        base.OnSelectionScoop(sprite);
    }

    public override void DropItemHeld() {
        if (itemHeld == null)
            return;

        if (itemHeld.name.Contains("SailorCap")) {
            if (!capDonned) {
                itemHeld.SortCompLayerReset(transform);
                itemHeld.transform.localPosition = capPosLocal;
                itemsWorn.Add(itemHeld);
                itemHeld.CharHeldBy = this;
                capDonned = true;
                itemHeld.EnableMouseTracking(true);
                lvlMngr.OnDeselection(itemHeld);
                itemHeld.InUse = false;
                itemHeld = null;
            }
        }
        else
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
