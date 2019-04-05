using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharChild : CharBase {

    protected bool canDonLifeJacketSelf = false;

    [SerializeField]
    protected Transform jacketParent;

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

    public void PlaceJacketTransform(Transform jacketTransform) {
        jacketTransform.position = jacketParent.position;
        jacketTransform.parent = jacketParent;
        // Seems to be changing the local rotation to NOT be zero, which I really do not understand. Rotation after animation seems fine/straight for now anyway
        //jacketTransform.rotation = Quaternion.identity;
    }
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
        PlaceJacketTransform(ItemHeld.transform);

        // Life jacket now permanently afixxed to the character
        itemsWorn.Add(ItemHeld);
        ItemHeld.CharHeldBy = this;
        ItemHeld = null;
        IsWearingLifeJacket = true;
        EndAction();
    }
}
