using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharChild : CharBase {

    protected override void Start() {
        strength = 25;
        speed = 25;
        Reset();
    }

    public override void CheckCanAct(bool childrenDonLifeJacket, bool adultDonLifeJacket, bool canScoop) {
        if(childrenDonLifeJacket) {
            canAct = true;

            // TODO: Add functions to buttons
            commandPanel.SetDonLifeJacketBtn();
        }
    }
}
