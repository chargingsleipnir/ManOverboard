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
        commandPanel.InactiveAwake();

        if (canDonLifeJacketSelf = lvlMngr.CheckCanDonLifeJacketChildren()) {
            canAct = true;
            commandPanel.PrepBtn(Consts.Skills.DonLifeJacket, PrepDonLifeJacket);
        }

        commandPanel.SetBtns();
    }
    public override void CheckActions() {
        commandPanel.EnableBtn(Consts.Skills.DonLifeJacket, canDonLifeJacketSelf = lvlMngr.CheckCanDonLifeJacketChildren());
    }

    // Donning life jacket ===============================================================

    protected virtual void PrepDonLifeJacket() {
        PrepAction(Consts.Skills.DonLifeJacket);
        lvlMngr.HighlightToSelect(Consts.HighlightGroupType.LifeJacketChild, OnSelectionLifeJacket);
    }
    protected virtual void OnSelectionLifeJacket(SpriteBase sprite) {
        itemHeld = sprite as LifeJacket;
        sprite.EnableMouseTracking(false);

        // Place life jacket in hand and start donning timer
        sprite.transform.position = trans_ItemUseHand.position;
        sprite.transform.parent = trans_ItemUseHand.parent;

        activityCounter = activityInterval = Consts.DON_RATE;
        ActionComplete = CompleteDonLifeJacket;
        TakeAction();
    }
    protected void CompleteDonLifeJacket() {
        // TODO: Just set in center of self for now, will need proper location around center of torso later
        itemHeld.transform.position = transform.position;
        itemHeld.transform.parent = transform;

        // Life jacket now permanently afixxed to the character
        itemsWorn.Add(itemHeld);
        IsWearingLifeJacket = true;

        //(selectObjQueue[0] as ItemBase).RetPosLocal = (selectObjQueue[0] as ItemBase).transform.localPosition;
        EndAction();
    }
}
