using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharChild : CharBase {

    protected bool canDonLifeJacketSelf = false;

    // Always being overridden without base reference. Use Reset() for base referencing
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
            commandPanel.PrepBtn(Consts.Skills.DonLifeJacket, DonLifeJacket);
        }

        commandPanel.SetBtns();
    }
    public override void CheckActions() {
        canDonLifeJacketSelf = lvlMngr.CheckCanDonLifeJacketChildren() && IsWearingLifeJacket == false;
        commandPanel.EnableBtn(Consts.Skills.DonLifeJacket, canDonLifeJacketSelf);
    }

    // Donning life jacket ===============================================================

    protected virtual void DonLifeJacket() {
        ActionQueueInit(Consts.Skills.DonLifeJacket);
        ActionQueueAdd(Consts.HighlightGroupType.LifeJacketChild, true, Consts.MIN_SEL_REACH_DIST, OnContactLifeJacketSelf);
        ActionQueueRun(Consts.DON_RATE, StepTimerBarFill, OnDonLifeJacketComplete);
    }
    // Not anonymous, so as to be overridden in CharAdult.cs
    protected void OnContactLifeJacketSelf(SpriteBase sprite) {
        HoldItem(sprite as ItemBase);
        AnimTrigger("DonLifeJacketSelf");
    }
    protected void OnDonLifeJacketComplete() {
        // TODO: Just set in center of self for now, will need proper location around center of torso later
        ItemHeld.transform.position = transform.position;
        ItemHeld.transform.parent = transform;
        ItemHeld.transform.rotation = Quaternion.identity;

        // Life jacket now permanently afixxed to the character
        itemsWorn.Add(ItemHeld);
        IsWearingLifeJacket = true;
    }
}
