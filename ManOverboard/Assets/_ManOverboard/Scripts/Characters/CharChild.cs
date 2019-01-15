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

    public override void CheckCanAct(bool childrenDonLifeJacket, bool adultDonLifeJacket, bool canScoop) {
        if(childrenDonLifeJacket) {
            canAct = true;
            canDonLifeJacketSelf = true;

            commandPanel.PrepBtn(Consts.Skills.DonLifeJacketSelf, PrepDonLifeJacketSelf);
        }

        commandPanel.SetBtns();
    }

    public override void UseItem(ItemBase item) {
        activeItem = item;
        activeItem.EnableMouseTracking(false);

        timerBar.IsActive = true;

        if (!heldItems.Contains(item)) {
            heldItems.Add(item);
            heldItemWeight += item.Weight;
        }

        if (item is LifeJacket) {
            charState = Consts.CharState.InAction;

            // TODO: Just set in center of self for now, will need proper location around center of torso later
            item.transform.position = transform.position;
            item.transform.parent = transform;

            // TODO: Of course this isn't meaningful right now, need to formulate exactly how this will work.
            float donRate = speed;
            activityCounter = activityInterval = donRate;
        }
    }

    protected void PrepDonLifeJacketSelf() {
        IsCommandPanelOpen = false;
        lvlMngr.HighlightToSelect(Consts.HighlightGroupType.LifeJacket);
    }
}
