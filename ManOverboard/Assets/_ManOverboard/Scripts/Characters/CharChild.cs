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

        canDonLifeJacketSelf = lvlMngr.CheckCanDonLifeJacketChildren() && IsWearingLifeJacket == false;

        if (canDonLifeJacketSelf) {
            canAct = true;
            commandPanel.PrepBtn(Consts.Skills.DonLifeJacket, PrepDonLifeJacket);
        }

        commandPanel.SetBtns();
    }
    public override void CheckActions() {
        canDonLifeJacketSelf = lvlMngr.CheckCanDonLifeJacketChildren() && IsWearingLifeJacket == false;
        commandPanel.EnableBtn(Consts.Skills.DonLifeJacket, canDonLifeJacketSelf);
    }

    // Donning life jacket ===============================================================

    protected virtual void PrepDonLifeJacket() {
        PrepAction(Consts.Skills.DonLifeJacket);
        lvlMngr.HighlightToSelect(Consts.HighlightGroupType.LifeJacketChild, OnSelectionLifeJacket);
    }
    protected virtual void OnSelectionLifeJacket(SpriteBase sprite) {
        activityCounter = activityInterval = Consts.DON_RATE;
        ActionComplete = CompleteDonLifeJacket;
        TakeAction();
    }
    protected void CompleteDonLifeJacket() {
        // TODO: Just set in center of self for now, will need proper location around center of torso later
        ItemHeld.transform.position = transform.position;
        ItemHeld.transform.parent = transform;

        // Life jacket now permanently afixxed to the character
        itemsWorn.Add(ItemHeld);
        IsWearingLifeJacket = true;

        //(selectObjQueue[0] as ItemBase).RetPosLocal = (selectObjQueue[0] as ItemBase).transform.localPosition;
        EndAction();
    }
}
