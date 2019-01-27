using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ZeroProgress.Common.Collections;

public class Crewman : JobBase {

    [SerializeField]
    protected ItemBase sailorCap;

    protected void Awake() {
        sailorCap.RetPosLocal = sailorCap.transform.localPosition;
    }
    public override void Init(CharBase character) {
        character.Strength += 25;
        character.Speed += 25;

        sailorCap.CharHeldBy = character;
        character.WearItem(sailorCap);
        // Set cap position relative to character's head here?
    }

    public override void SetActionBtns() {

    }
    public override void CheckCanAct() {
        // Need to tell character (for their command panel) whether or not they can still perfom whatever action
        // First time this sets all buttons, leaving out those that cannot be perfomed
        // Afterwards, this greys out those that cannot be done.
    }

    //private void ClickThroughHat(bool clickThrough) {
    //    RefShape2DMouseTracker[] trackers = sailorHat.GetComponents<RefShape2DMouseTracker>();
    //    for (int i = 0; i < trackers.Length; i++)
    //        trackers[i].clickThrough = clickThrough;
    //}

    // JOB SPECIFIC ACTIONS (just examples for now)

    public void LowerAnchor() {

    }
    public void ReleaseAnchor() {

    }
    public void RaiseSail() {

    }
}
