using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharElder : CharChild {

    protected bool canDonLifeJacketChild = false;
    protected bool canScoop = false;

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

    protected void PrepDonLifeJacketChild() {

    }

    public virtual void PrepScoop() {
        if (canScoop)
            return;

        IsCommandPanelOpen = false;
        DelSetItemType(Consts.ItemType.Scooping);
    }
}
