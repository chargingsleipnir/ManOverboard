using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharChild : CharBase {

    protected bool canDonLifeJacket = false;

    protected override void Start() {
        strength = 25;
        speed = 25;
        Reset();
    }

    public override void SetActionBtns() {
        commandPanel.InactiveAwake();

        if (lvlMngr.CheckCanDonLifeJacketChildren(false)) {
            canAct = true;
            canDonLifeJacket = true;

            commandPanel.PrepBtn(Consts.Skills.DonLifeJacket, PrepDonLifeJacket);
        }

        commandPanel.SetBtns();
    }
    public override void CheckActions() {
        if (canDonLifeJacket)
            commandPanel.EnableBtn(Consts.Skills.DonLifeJacket, lvlMngr.CheckCanDonLifeJacketChildren(false));
    }

    protected virtual void OnSelectionLifeJacket(SpriteBase sprite) {
        selectObjQueue.Add(sprite);
        sprite.EnableMouseTracking(false);        

        // Place life jacket in hand and start donning timer
        sprite.transform.position = trans_ItemUseHand.position;
        sprite.transform.parent = trans_ItemUseHand.parent;

        activityCounter = activityInterval = Consts.DON_RATE;
        ActionComplete = CompleteDonLifeJacket;
        TakeAction();
    }

    protected virtual void PrepDonLifeJacket() {
        PrepAction(Consts.Skills.DonLifeJacket);
        lvlMngr.HighlightToSelect(Consts.HighlightGroupType.LifeJacket, OnSelectionLifeJacket);
    }
    protected void CompleteDonLifeJacket() {
        // TODO: Just set in center of self for now, will need proper location around center of torso later
        selectObjQueue[0].transform.position = transform.position;
        selectObjQueue[0].transform.parent = transform;

        // Remove item so it can no longer be acted upon, but do not deduct weight, as it's now permanently afixxed to the character
        itemsHeld.Remove(selectObjQueue[0] as ItemBase);

        //(selectObjQueue[0] as ItemBase).RetPosLocal = (selectObjQueue[0] as ItemBase).transform.localPosition;
        EndAction();
    }
}
