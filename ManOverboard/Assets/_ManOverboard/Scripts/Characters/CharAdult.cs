using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharAdult : CharChild {

    protected bool canDonLifeJacketChild = false;
    protected bool canScoop = false;
    int waterWeight = 0; 

    protected override void Start() {
        Reset();
    }

    protected override void Reset() {
        base.Reset();
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
            lvlMngr.HighlightToSelect(Consts.HighlightGroupType.Children, OnSelectionChild);
        }
    }
    private void OnSelectionChild(SpriteBase sprite) {
        // Add child for completion Callback
        activeChar = sprite as CharChild;
        activeChar.SetStateDazed(true);

        activityCounter = activityInterval = Consts.DON_RATE;
        ActionComplete = CompleteDonLifeJacketChild;
        TakeAction();
    }
    private void CompleteDonLifeJacketChild() {
        // TODO: Just set in center of self for now, will need proper location around center of torso later
        itemHeld.transform.position = activeChar.transform.position;
        itemHeld.transform.parent = activeChar.transform;

        ItemWeight -= itemHeld.Weight;
        activeChar.WearItem(itemHeld);
        activeChar.IsWearingLifeJacket = true;
        activeChar.CharState = Consts.CharState.Default;
        activeChar.SetStateDazed(false);

        itemHeld = null;

        //(selectObjQueue[0] as ItemBase).RetPosLocal = (selectObjQueue[0] as ItemBase).transform.localPosition;

        EndAction();
    }

    // Scooping Water ===============================================================

    public virtual void PrepScoop() {
        PrepAction(Consts.Skills.ScoopWater);
        lvlMngr.HighlightToSelect(Consts.HighlightGroupType.Scooping, OnSelectionScoop);
    }
    protected virtual void OnSelectionScoop(SpriteBase sprite) {
        sprite.EnableMouseTracking(false);

        // Logic for scooping wth item
        sprite.transform.position = trans_ItemUseHand.position;
        sprite.transform.parent = trans_ItemUseHand.parent;

        ItemCanScoop scoop = sprite as ItemCanScoop;
        waterWeight = scoop.capacity;
        float heldWeight = scoop.Weight + waterWeight;

        activityCounter = activityInterval = Consts.SCOOP_RATE - (strength - heldWeight);
        ActionComplete = CompleteSingleScoop;
        TakeAction();
    }
    private void CompleteSingleScoop() {
        lvlMngr.RemoveWater(waterWeight);
    }
}
