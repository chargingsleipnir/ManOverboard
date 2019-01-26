using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharElder : CharChild {

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

        // TODO: Need to consider that a single button should apply to both, jacketting a child or self
        if (lvlMngr.CheckCanDonLifeJacketChildren(true) || lvlMngr.CheckCanDonLifeJacketAdults(false)) {
            canAct = true;
            canDonLifeJacket = true;
            commandPanel.PrepBtn(Consts.Skills.DonLifeJacket, PrepDonLifeJacket);
        }
        if(lvlMngr.CheckCanScoop()) {
            canAct = true;
            canScoop = true;

            commandPanel.PrepBtn(Consts.Skills.ScoopWater, PrepScoop);
        }

        // TODO: Requires bool for job specific options
        if(job != null)
            job.SetActionBtns();

        commandPanel.SetBtns();
    }
    public override void CheckActions() {
        if (canDonLifeJacket)
            commandPanel.EnableBtn(Consts.Skills.DonLifeJacket, lvlMngr.CheckCanDonLifeJacketChildren(true) || lvlMngr.CheckCanDonLifeJacketAdults(false));

        if (canScoop)
            commandPanel.EnableBtn(Consts.Skills.ScoopWater, lvlMngr.CheckCanScoop());
    }

    private void OnSelectionScoop(SpriteBase sprite) {
        selectObjQueue.Add(sprite);
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

    protected override void OnSelectionLifeJacket(SpriteBase sprite) {
        LifeJacket jacket = sprite as LifeJacket;

        // Adult jacket, can don self immediately
        if (jacket.size == Consts.FitSizes.adult) {
            base.OnSelectionLifeJacket(sprite);
        }
        // Child jacket, need to wait for child to be selected
        else {
            selectObjQueue.Add(sprite);
            lvlMngr.HighlightToSelect(Consts.HighlightGroupType.Children, OnSelectionChild);
        }
    }
    private void OnSelectionChild(SpriteBase sprite) {
        // Add child for completion Callback
        selectObjQueue.Add(sprite);

        // selectObjQueue[0] refers to child life jacket at this point
        selectObjQueue[0].EnableMouseTracking(false);

        selectObjQueue[0].transform.position = trans_ItemUseHand.position;
        selectObjQueue[0].transform.parent = trans_ItemUseHand.parent;

        activityCounter = activityInterval = Consts.DON_RATE;
        ActionComplete = CompleteDonLifeJacketChild;
        TakeAction();
    }

    protected override void PrepDonLifeJacket() {
        PrepAction(Consts.Skills.DonLifeJacket);
        lvlMngr.HighlightToSelect(Consts.HighlightGroupType.LifeJacket, OnSelectionLifeJacket);
    }
    private void CompleteDonLifeJacketChild() {
        // TODO: Just set in center of self for now, will need proper location around center of torso later
        selectObjQueue[0].transform.position = selectObjQueue[1].transform.position;
        selectObjQueue[0].transform.parent = selectObjQueue[1].transform;

        // Remove item so it can no longer be acted upon, and transfer weight to child
        ItemBase jacket = selectObjQueue[0] as ItemBase;
        itemsHeld.Remove(jacket);
        heldItemWeight -= jacket.Weight;

        // TODO: Fix this garbage
        (selectObjQueue[1] as CharChild).heldItemWeight += jacket.Weight;
        
        //(selectObjQueue[0] as ItemBase).RetPosLocal = (selectObjQueue[0] as ItemBase).transform.localPosition;

        EndAction();
    }

    public virtual void PrepScoop() {
        PrepAction(Consts.Skills.ScoopWater);
        lvlMngr.HighlightToSelect(Consts.HighlightGroupType.Scooping, OnSelectionScoop);
    }
    private void CompleteSingleScoop() {
        lvlMngr.RemoveWater(waterWeight);
    }
}
