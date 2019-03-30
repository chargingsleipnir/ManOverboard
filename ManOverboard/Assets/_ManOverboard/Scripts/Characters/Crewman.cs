﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ZeroProgress.Common.Collections;

public class Crewman : CharAdult {

    [SerializeField]
    protected ItemBase sailorCap;

    [SerializeField]
    protected Transform capParent;

    Vector3 capPosLocal;
    bool capDonned;

    private Hole holeToRepair;

    protected bool canRepair = false;

    protected override void Awake() {
        capPosLocal = sailorCap.transform.localPosition;
        capDonned = true;
        base.Awake();
    }

    protected override void Start() {
        strength += 25;
        speed += 25;

        Reset();

        WearItem(sailorCap);
    }

    public override void SetActionBtns() {
        commandPanel.InactiveAwake();

        canDonLifeJacketSelf = lvlMngr.CheckCanDonLifeJacketAdults() && IsWearingLifeJacket == false;
        canDonLifeJacketChild = lvlMngr.CheckCanDonLifeJacketChildren();
        canScoop = lvlMngr.CheckCanScoop();
        canRepair = lvlMngr.CheckCanRepair();

        if (canDonLifeJacketSelf) {
            canAct = true;
            commandPanel.PrepBtn(Consts.Skills.DonLifeJacket, PrepDonLifeJacket);
        }
        if (canDonLifeJacketChild) {
            canAct = true;
            commandPanel.PrepBtn(Consts.Skills.DonLifeJacket, PrepDonLifeJacket);
        }
        if (canScoop) {
            canAct = true;
            commandPanel.PrepBtn(Consts.Skills.ScoopWater, PrepScoop);
        }
        if(canRepair) {
            canAct = true;
            commandPanel.PrepBtn(Consts.Skills.RepairPinhole, PrepRepair);
        }

        commandPanel.SetBtns();
    }

    public override void CheckActions() {
        base.CheckActions();
        canRepair = lvlMngr.CheckCanRepair();
        commandPanel.EnableBtn(Consts.Skills.RepairPinhole, canRepair);
    }

    public override void Toss(Vector2 vel) {
        base.Toss(vel);

        if (capDonned) {
            sailorCap.Toss(Utility.AddNoiseDeg(vel, Consts.TOSS_NOISE_MIN, Consts.TOSS_NOISE_MAX));
            lvlMngr.RemoveItem(sailorCap);
        }
    }

    public override void LoseItem(ItemBase item) {
        // Only way to specify this item with precision right now
        if (item.name.Contains("SailorCap")) {
            capDonned = false;
        }

        base.LoseItem(item);
    }

    public override void CancelAction() {
        if(holeToRepair != null) {
            holeToRepair.InRepair = false;
            holeToRepair = null;
        }
        base.CancelAction();
    }

    public override void DropItemHeld() {
        if (ItemHeld == null)
            return;

        if (ItemHeld.name.Contains("SailorCap")) {
            if (!capDonned) {
                sailorCap = ItemHeld;
                sailorCap.SortCompResetToBase(capParent);
                sailorCap.transform.localPosition = capPosLocal;
                sailorCap.transform.rotation = Quaternion.identity;
                sailorCap.CharHeldBy = this;
                itemsWorn.Add(sailorCap);
                capDonned = true;
                sailorCap.EnableMouseTracking(true);
                lvlMngr.OnDeselection(sailorCap);
                sailorCap.InUse = false;
                ItemHeld = null;
                return;
            }
        }

        base.DropItemHeld();
    }

    // JOB SPECIFIC ACTIONS (just examples for now)

    protected override void OnSelectionScoop(SpriteBase sprite) {
        if ((sprite as ItemBase) == sailorCap)
            capDonned = false;

        base.OnSelectionScoop(sprite);
    }

    public void PrepRepair() {
        PrepAction(Consts.Skills.RepairPinhole);
        lvlMngr.HighlightToSelect(Consts.HighlightGroupType.RepairKits, OnSelectionRepairKit);
    }
    private void OnSelectionRepairKit(SpriteBase sprite) {
        lvlMngr.HighlightToSelect(Consts.HighlightGroupType.PinHoles, OnSelectionPinhole);
    }
    private void OnSelectionPinhole(SpriteBase sprite) {
        holeToRepair = sprite as Hole;

        holeToRepair.InRepair = true;
        activityCounter = activityInterval = Consts.REPAIR_RATE;
        ActionComplete = CompleteRepair;
        TakeAction();
    }
    private void CompleteRepair() {
        (ItemHeld as RepairKit).uses -= 1;
        // TODO: Does the weight simply disappear? Add it to the boat by reducing it's buoyancy?
        // But repairing a hole would increase the buoyancy anyway, so reduce this weight and call it even?
        ItemHeld.Weight -= 2;

        lvlMngr.RepairBoat(holeToRepair);
        holeToRepair = null;

        EndAction();
        DropItemHeld();
        
    }

    public void LowerAnchor() {

    }
    public void ReleaseAnchor() {

    }
    public void RaiseSail() {

    }
}
