using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharElder : CharChild {

    protected bool canDonLifeJacketChild = false;
    protected bool canScoop = false;

    protected int capWeight;

    protected override void Start() {
        strength = 50;
        speed = 50;
        Reset();
    }

    public override void CheckCanAct(bool childrenDonLifeJacket, bool adultDonLifeJacket, bool scoopWater) {

        // TODO: Need to consider that a single button should apply to both, jacketting a child or self
        if (childrenDonLifeJacket) {
            canAct = true;
            canDonLifeJacketChild = true;
            commandPanel.PrepDonLifeJacketBtn(PrepDonLifeJacketChild);
        }
        if(adultDonLifeJacket) {
            canAct = true;
            canDonLifeJacketSelf = true;
            commandPanel.PrepDonLifeJacketBtn(PrepDonLifeJacketSelf);
        }
        if(scoopWater) {
            canAct = true;
            canScoop = true;

            commandPanel.PrepScoopBtn(PrepScoop);

            // TODO: The actions that can be taken will build up from nothing (buttons not existing), however, as abilities are lost (items used, chars tossed),
            // The buttons will not disappear, but rather be greyed out.
            //scoopBtnSprite.ChangeColour(null, null, null, 1.0f);
            //scoopBtnSprite.ChangeColour(null, null, null, Consts.BTN_DISABLE_FADE);
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

        if (item is ItemCanScoop) {
            charState = Consts.CharState.InAction;

            // Logic for scooping wth item
            item.transform.position = trans_ItemUseHand.position;
            item.transform.parent = trans_ItemUseHand.parent;

            capWeight = (item as ItemCanScoop).capacity;
            float heldWeight = item.Weight + capWeight;
            float scoopRate = (heldWeight / strength) * heldWeight;
            if (scoopRate < Consts.MIN_SCOOP_RATE)
                scoopRate = Consts.MIN_SCOOP_RATE;

            activityCounter = activityInterval = scoopRate;
        }
    }

    protected void PrepDonLifeJacketChild() {
        IsCommandPanelOpen = false;
        DelSetItemType(Consts.ItemType.LifeJacket);
    }

    public virtual void PrepScoop() {
        if (!canScoop)
            return;

        IsCommandPanelOpen = false;
        DelSetItemType(Consts.ItemType.Scooping);
    }
}
