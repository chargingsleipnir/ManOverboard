using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharElder : CharChild {

    [SerializeField]
    protected SpriteBase scoopBtnSprite;

    protected bool blockScooping = true;

    protected override void Start() {
        strength = 50;
        speed = 50;
        Reset();
    }

    public override void CheckCanAct(bool childrenDonLifeJacket, bool adultDonLifeJacket, bool canScoop) {
        if (childrenDonLifeJacket) {
            canAct = true;
        }
        if(adultDonLifeJacket) {
            canAct = true;
        }
        if(canScoop) {
            canAct = true;
            blockScooping = false;

            // TODO: The actions that can be taken will build up from nothing (buttons not existing), however, as abilities are lost (items used, chars tossed),
            // The buttons will not disappear, but rather be greyed out.
            //scoopBtnSprite.ChangeColour(null, null, null, 1.0f);
            //scoopBtnSprite.ChangeColour(null, null, null, Consts.BTN_DISABLE_FADE);
        }
    }

    public virtual void PrepScoop() {
        if (blockScooping)
            return;

        IsCommandPanelOpen = false;
        DelSetItemType(Consts.ItemType.Scooping);
    }

}
