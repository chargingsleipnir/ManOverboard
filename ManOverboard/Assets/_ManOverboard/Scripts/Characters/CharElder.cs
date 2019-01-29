using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharElder : CharChild {

    protected bool canDonLifeJacketChild = false;
    protected bool canScoop = false;
    int waterWeight = 0;

    [SerializeField]
    protected JobBase job;    

    protected override void Start() {
        strength = 50;
        speed = 50;
        Reset();
    }

    protected override void Reset() {
        base.Reset();
        if(job != null)
            job.Init(this);
    }

    public override void SetActionBtns() {
        commandPanel.InactiveAwake();

        canDonLifeJacketSelf = lvlMngr.CheckCanDonLifeJacketAdults() && IsWearingLifeJacket == false;
        canDonLifeJacketChild = lvlMngr.CheckCanDonLifeJacketChildren();
        canScoop = lvlMngr.CheckCanScoop();

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

        // TODO: Requires bool for job specific options
        if(job != null)
            job.SetActionBtns();

        commandPanel.SetBtns();
    }

    public override void CheckActions() {
        canDonLifeJacketChild = lvlMngr.CheckCanDonLifeJacketChildren();
        canDonLifeJacketSelf = lvlMngr.CheckCanDonLifeJacketAdults() && IsWearingLifeJacket == false;
        canScoop = lvlMngr.CheckCanScoop();

        commandPanel.EnableBtn(Consts.Skills.DonLifeJacket, canDonLifeJacketChild || canDonLifeJacketSelf);
        commandPanel.EnableBtn(Consts.Skills.ScoopWater, canScoop);
    }

    // Donning life jacket ===============================================================

    protected override void PrepDonLifeJacket() {
        PrepAction(Consts.Skills.DonLifeJacket);
        if(canDonLifeJacketChild)
            lvlMngr.HighlightToSelect(Consts.HighlightGroupType.LifeJacketChild, OnSelectionLifeJacket);
        if (canDonLifeJacketSelf)
            lvlMngr.HighlightToSelect(Consts.HighlightGroupType.LifeJacketAdult, OnSelectionLifeJacket);
    }
    protected override void OnSelectionLifeJacket(SpriteBase sprite) {
        LifeJacket jacket = sprite as LifeJacket;

        // Adult jacket, can don self immediately
        if (jacket.size == Consts.FitSizes.adult) {
            base.OnSelectionLifeJacket(sprite);
        }
        // Child jacket, need to wait for child to be selected
        else {
            itemHeld = sprite as LifeJacket;
            lvlMngr.HighlightToSelect(Consts.HighlightGroupType.Children, OnSelectionChild);
        }
    }
    private void OnSelectionChild(SpriteBase sprite) {
        // Add child for completion Callback
        activeChar = sprite as CharChild;
        activeChar.CharState = Consts.CharState.Dazed;

        itemHeld.EnableMouseTracking(false);
        itemHeld.transform.position = trans_ItemUseHand.position;
        itemHeld.transform.parent = trans_ItemUseHand.parent;

        activityCounter = activityInterval = Consts.DON_RATE;
        ActionComplete = CompleteDonLifeJacketChild;
        TakeAction();
    }
    private void CompleteDonLifeJacketChild() {
        // TODO: Just set in center of self for now, will need proper location around center of torso later
        itemHeld.transform.position = activeChar.transform.position;
        itemHeld.transform.parent = activeChar.transform;

        // Remove item so it can no longer be acted upon, and transfer weight to child
        itemsWorn.Remove(itemHeld);
        itemWeight -= itemHeld.Weight;

        // TODO: Fix this garbage
        activeChar.itemWeight += itemHeld.Weight;
        activeChar.IsWearingLifeJacket = true;
        activeChar.CharState = Consts.CharState.Default;

        //(selectObjQueue[0] as ItemBase).RetPosLocal = (selectObjQueue[0] as ItemBase).transform.localPosition;

        EndAction();
    }

    // Scooping Water ===============================================================

    public virtual void PrepScoop() {
        PrepAction(Consts.Skills.ScoopWater);
        lvlMngr.HighlightToSelect(Consts.HighlightGroupType.Scooping, OnSelectionScoop);
    }
    private void OnSelectionScoop(SpriteBase sprite) {
        itemHeld = sprite as ItemCanScoop;
        sprite.EnableMouseTracking(false);

        // Logic for scooping wth item
        sprite.transform.position = trans_ItemUseHand.position;
        sprite.transform.parent = trans_ItemUseHand.parent;

        ItemCanScoop scoop = sprite as ItemCanScoop;
        waterWeight = scoop.capacity;
        float heldWeight = scoop.Weight + waterWeight;
        float scoopRate = (heldWeight / strength) * heldWeight;
        if (scoopRate < Consts.MIN_SCOOP_RATE)
            scoopRate = Consts.MIN_SCOOP_RATE;

        activityCounter = activityInterval = scoopRate;
        ActionComplete = CompleteSingleScoop;
        TakeAction();
    }
    private void CompleteSingleScoop() {
        lvlMngr.RemoveWater(waterWeight);
    }
}
