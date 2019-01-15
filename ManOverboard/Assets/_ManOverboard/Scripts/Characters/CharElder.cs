using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharElder : CharChild {

    protected bool canDonLifeJacketChild = false;
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

    protected override void Update() {
        if (Paused)
            return;

        if (tossableState == Consts.SpriteTossableState.Default) {
            if (charState == Consts.CharState.InAction) {
                activityCounter -= Time.deltaTime;
                float counterPct = 1.0f - (activityCounter / activityInterval);
                timerBar.Fill = counterPct;
                if (activityCounter <= 0) {
                    activityCounter = activityInterval;
                    if (canScoop)
                        lvlMngr.RemoveWater(waterWeight);
                    else if(canDonLifeJacketChild) {
                        // TODO: Code/Action to make it happen

                        // Once the jacket is donned, action is over
                        charState = Consts.CharState.Default;
                    }
                }
            }
        }
    }

    public override void CheckCanAct(bool childrenDonLifeJacket, bool adultDonLifeJacket, bool scoopWater) {

        // TODO: Need to consider that a single button should apply to both, jacketting a child or self
        if (childrenDonLifeJacket) {
            canAct = true;
            canDonLifeJacketChild = true;
            commandPanel.PrepBtn(Consts.Skills.DonLifeJacketOther, PrepDonLifeJacketChild);
        }
        if(adultDonLifeJacket) {
            canAct = true;
            canDonLifeJacketSelf = true;
            commandPanel.PrepBtn(Consts.Skills.DonLifeJacketSelf, PrepDonLifeJacketSelf);
        }
        if(scoopWater) {
            canAct = true;
            canScoop = true;

            commandPanel.PrepBtn(Consts.Skills.ScoopWater, PrepScoop);

            // TODO: The actions that can be taken will build up from nothing (buttons not existing), however, as abilities are lost (items used, chars tossed),
            // The buttons will not disappear, but rather be greyed out.
            //scoopBtnSprite.ChangeColour(null, null, null, 1.0f);
            //scoopBtnSprite.ChangeColour(null, null, null, Consts.BTN_DISABLE_FADE);
        }

        // TODO: Requires bool for job specific options
        if(job != null)
            job.CheckCanAct();

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

            waterWeight = (item as ItemCanScoop).capacity;
            float heldWeight = item.Weight + waterWeight;
            float scoopRate = (heldWeight / strength) * heldWeight;
            if (scoopRate < Consts.MIN_SCOOP_RATE)
                scoopRate = Consts.MIN_SCOOP_RATE;

            activityCounter = activityInterval = scoopRate;
        }
        else if(item is LifeJacket) {
            if((item as LifeJacket).size == Consts.FitSizes.child) {

            }
            else {

            }
        }
    }

    protected void PrepDonLifeJacketChild() {
        IsCommandPanelOpen = false;
        lvlMngr.HighlightToSelect(Consts.HighlightGroupType.LifeJacket);
    }

    public virtual void PrepScoop() {
        if (!canScoop)
            return;

        IsCommandPanelOpen = false;
        lvlMngr.HighlightToSelect(Consts.HighlightGroupType.Scooping);
    }
}
