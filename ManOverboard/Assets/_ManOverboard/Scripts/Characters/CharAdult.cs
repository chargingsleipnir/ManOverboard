using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharAdult : CharChild {

    protected bool canDonLifeJacketChild = false;
    protected bool canScoop = false;
    int waterWeight = 0;

    // Always being overridden without base reference. Use Reset() for base referencing
    protected override void Start() {
        Reset();
    }

    public override void SetActionBtns() {
        commandPanel.InactiveAwake();

        canDonLifeJacketSelf = lvlMngr.CheckCanDonLifeJacketAdults() && IsWearingLifeJacket == false;
        canDonLifeJacketChild = lvlMngr.CheckCanDonLifeJacketChildren();
        canScoop = lvlMngr.CheckCanScoop();

        if (canDonLifeJacketSelf) {
            canAct = true;
            commandPanel.PrepBtn(Consts.Skills.DonLifeJacket, DonLifeJacket);
        }
        if (canDonLifeJacketChild) {
            canAct = true;
            commandPanel.PrepBtn(Consts.Skills.DonLifeJacket, DonLifeJacket);
        }
        if (canScoop) {
            canAct = true;
            commandPanel.PrepBtn(Consts.Skills.ScoopWater, Scoop);
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

    protected override void DonLifeJacket() {
        ActionQueueInit(Consts.Skills.DonLifeJacket, true);

        if (canDonLifeJacketChild)
            ActionQueueAdd(Consts.HighlightGroupType.LifeJacketChild, true, Consts.MIN_SEL_REACH_DIST, (SpriteBase sprite) => { HoldItem(sprite as ItemBase); });
        if (canDonLifeJacketSelf)
            ActionQueueAdd(Consts.HighlightGroupType.LifeJacketAdult, true, Consts.MIN_SEL_REACH_DIST, OnContactLifeJacketSelf);

        ActionQueueRun((SpriteBase jacketSprite) => {
            LifeJacket jacket = jacketSprite as LifeJacket;

            if (jacket.size == Consts.FitSizes.child) {
                ActionQueueAccSplitIdx(0, jacketSprite);

                // Can shut path splitting off at this point and map out the rest linearly
                ActPathSplitOff(); 
                ActionQueueAdd(Consts.HighlightGroupType.Children, true, Consts.MIN_SEL_REACH_DIST, (SpriteBase childSprite) => {
                    // Add child for completion Callback
                    activeChar = childSprite as CharChild;
                    activeChar.SetStateDazed(true);

                    AnimTrigger("DonLifeJacketChild");
                });
                ActionQueueRun(Consts.DON_RATE, StepTimerBarFill, () => {
                    (activeChar as CharChild).PlaceJacketTransform(ItemHeld.transform);


                    ItemWeight -= ItemHeld.Weight;
                    activeChar.WearItem(ItemHeld);
                    activeChar.IsWearingLifeJacket = true;
                    activeChar.CharState = Consts.CharState.Default;
                    activeChar.SetStateDazed(false);

                    ItemHeld = null;

                    EndAction();
                    //(selectObjQueue[0] as ItemBase).RetPosLocal = (selectObjQueue[0] as ItemBase).transform.localPosition;
                });
            }
            else {
                // Index will differ based on which actions were available to be added above.
                ActionQueueAccSplitIdx(canDonLifeJacketChild ? 1 : 0, jacketSprite);
                ActionQueueRun(Consts.DON_RATE, StepTimerBarFill, OnDonLifeJacketComplete);
            }
        });
    }

    // Scooping Water ===============================================================

    public void Scoop() {
        ActionQueueInit(Consts.Skills.ScoopWater);
        ActionQueueAdd(Consts.HighlightGroupType.Scooping, true, Consts.MIN_SEL_REACH_DIST, OnSelectionScoop);
        // First param is overridden just above -> ActionQueueModTaskCounter
        ActionQueueRun(Consts.SCOOP_RATE, StepTimerBarFill, () => {
            lvlMngr.RemoveWater(waterWeight);
        });
    }
    // Not anonymous, so as to be overridden in Crewman.cs
    protected virtual void OnSelectionScoop(SpriteBase sprite) {
        HoldItem(sprite as ItemBase);

        ItemCanScoop scoop = sprite as ItemCanScoop;
        waterWeight = scoop.capacity;
        float heldWeight = scoop.Weight + waterWeight;
        ActionQueueModTaskCounter(Consts.SCOOP_RATE - (strength - heldWeight));

        AnimTrigger("Scoop");
    }
}
