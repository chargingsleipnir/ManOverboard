using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ZeroProgress.Common.Collections;

public class Crewman : CharAdult {

    [SerializeField]
    protected ItemBase sailorHat;

    protected override void Awake() {
        base.Awake();
        
        capWeight = 0;
        sailorHat.RetPosLocal = sailorHat.transform.localPosition;
    }
    protected override void Start() {
        strength = 125;
        speed = 125;
        Reset();

        sailorHat.CharHeldBy = this;
        heldItems.Add(sailorHat);
        heldItemWeight += sailorHat.Weight;

        canScoop = true;
    }

    //private void ClickThroughHat(bool clickThrough) {
    //    RefShape2DMouseTracker[] trackers = sailorHat.GetComponents<RefShape2DMouseTracker>();
    //    for (int i = 0; i < trackers.Length; i++)
    //        trackers[i].clickThrough = clickThrough;
    //}

    public override void PrepScoop() {
        if (!canScoop)
            return;

        IsCommandPanelOpen = false;
        DelSetItemType(Consts.HighlightGroupType.Scooping);
    }

    public override void CancelAction() {
        if (activeItem == sailorHat) {
            sailorHat.Deselect();
            activeItem = null;
        }

        base.CancelAction();
    }
}
