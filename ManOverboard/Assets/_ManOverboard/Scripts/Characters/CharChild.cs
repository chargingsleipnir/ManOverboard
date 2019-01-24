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

    public override void GetSelection(SpriteBase sprite) {
        activeItem = sprite as ItemBase;
        activeItem.EnableMouseTracking(false);

        timerBar.IsActive = true;

        if (!heldItems.Contains(activeItem)) {
            heldItems.Add(activeItem);
            heldItemWeight += activeItem.Weight;
        }

        lvlMngr.ResetEnvir();

        // Presuming item is life jacket, as that's all the child can use right now.

        // Place life jacket in hand and start donning timer
        activeItem.transform.position = trans_ItemUseHand.position;
        activeItem.transform.parent = trans_ItemUseHand.parent;

        // TODO: Of course this isn't meaningful right now, need to formulate exactly how this will work.
        activityCounter = activityInterval = Consts.DON_RATE;
        ActionComplete = CompleteDonLifeJacket;

        charState = Consts.CharState.InAction;
    }

    protected virtual void PrepDonLifeJacket() {
        PrepAction(Consts.Skills.DonLifeJacket);
        lvlMngr.HighlightToSelect(Consts.HighlightGroupType.LifeJacket);
    }
    protected void CompleteDonLifeJacket() {
        // TODO: Just set in center of self for now, will need proper location around center of torso later
        activeItem.transform.position = transform.position;
        activeItem.transform.parent = transform;
        EndAction();
    }
}
