﻿using System.Collections;
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

    protected override void Update() {
        if (Paused)
            return;

        if (tossableState == Consts.SpriteTossableState.Default) {
            if (charState == Consts.CharState.InAction) {
                activityCounter -= Time.deltaTime;
                float counterPct = 1.0f - (activityCounter / activityInterval);
                timerBar.Fill = counterPct;
                if (activityCounter <= 0) {
                    activityCounter = activityInterval;
                    DelRemoveWater(capWeight);
                }
            }
        }

        base.Update();
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
        DelSetItemType(Consts.ItemType.Scooping);
    }

    public override void CancelAction() {
        if (activeItem == sailorHat) {
            sailorHat.Deselect();
            activeItem = null;
        }

        base.CancelAction();
    }
}
