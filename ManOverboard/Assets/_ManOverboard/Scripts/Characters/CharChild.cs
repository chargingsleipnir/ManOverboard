using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharChild : CharBase {

    protected bool canDonLifeJacketSelf = false;

    protected override void Start() {
        strength = 25;
        speed = 25;
        Reset();
    }

    public override void SetActionBtns() {
        base.SetActionBtns();

        if (lvlMngr.CheckCanDonLifeJacketChildren(false)) {
            canAct = true;
            canDonLifeJacketSelf = true;

            commandPanel.PrepBtn(Consts.Skills.DonLifeJacketSelf, PrepDonLifeJacketSelf);
        }

        commandPanel.SetBtns();
    }
    public override void CheckCanAct() {
        if(lvlMngr.CheckCanDonLifeJacketChildren(false)) {
            commandPanel.EnableBtn(Consts.Skills.DonLifeJacketSelf);
            canDonLifeJacketSelf = true;
        }
        else {
            commandPanel.DisableBtn(Consts.Skills.DonLifeJacketSelf);
            canDonLifeJacketSelf = false;
        }
    }

    // Initiated from CharBase Update(), when activityCounter reaches 0
    protected override void Action_CounterZero() {
        if (canDonLifeJacketSelf) {
            // TODO: Just set in center of self for now, will need proper location around center of torso later
            activeItem.transform.position = transform.position;
            activeItem.transform.parent = transform;
        }
        EndAction();
    }

    public override void UseItem(ItemBase item) {
        activeItem = item;
        activeItem.EnableMouseTracking(false);

        timerBar.IsActive = true;

        if (!heldItems.Contains(item)) {
            heldItems.Add(item);
            heldItemWeight += item.Weight;
        }

        if (item is LifeJacket) {
            charState = Consts.CharState.InAction;

            // Place life jacket in hand and start donning timer
            item.transform.position = trans_ItemUseHand.position;
            item.transform.parent = trans_ItemUseHand.parent;

            // TODO: Of course this isn't meaningful right now, need to formulate exactly how this will work.
            activityCounter = activityInterval = Consts.DON_RATE;
        }
    }

    protected void PrepDonLifeJacketSelf() {
        IsCommandPanelOpen = false;
        ReturnToBoat();
        lvlMngr.HighlightToSelect(Consts.HighlightGroupType.LifeJacket);
    }
}
