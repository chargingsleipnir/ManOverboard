using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common.Collections;

public class Crewman : CharActionable {

    DelSetItemType delSet;
    DelStartCoroutine delStart;
    DelStopCoroutine delStop;

    private Coroutine activeCo;

    [SerializeField]
    protected ItemBase sailorHat;

    private ItemBase activeItem;

    protected override void Awake() {
        base.Awake();
        strength = 125;
    }
    protected override void Start() {
        base.Start();
        heldItems.Add(sailorHat);
    }

    private void ClickThroughHat(bool clickThrough) {
        RefShape2DMouseTracker[] trackers = sailorHat.GetComponents<RefShape2DMouseTracker>();
        for (int i = 0; i < trackers.Length; i++)
            trackers[i].clickThrough = clickThrough;
    }

    public void PrepScoop() {
        ClickThroughHat(false);

        IsCommandPanelOpen = false;
        delSet(Consts.ItemType.Scooping);
        state = Consts.CharState.Scooping;
    }

    public override void UseItem(ItemBase item) {
        activeItem = item;
        if (!heldItems.Contains(item))
            heldItems.Add(item);

        if (state == Consts.CharState.Scooping) {
            // Logic for scooping wth item
            item.transform.position = trans_ItemUseHand.position;
            item.transform.parent = trans_ItemUseHand.parent;
            ClickThroughHat(true);

            if (item is ItemCanScoop) {
                int capWeight = (item as ItemCanScoop).capacity;
                float heldWeight = item.Weight + capWeight;
                float scoopRate = (heldWeight / strength) * heldWeight;
                if (scoopRate < Consts.MIN_SCOOP_RATE)
                    scoopRate = Consts.MIN_SCOOP_RATE;

                activeCo = delStart(capWeight, scoopRate);
            }
            else {
                Debug.Log("Crewman.cs, UseItem, ItemBase obj is not of proper action type");
            }
        }
    }


    // TODO: Really flush this out
    // Item need to go somewhere, keeping it's relative position to it's parent intact - not necessarily it's original position (bucket), just a proper y level near the player.

    public override void CancelAction() {
        if (state == Consts.CharState.Scooping) {
            activeItem.GoToOrigPlacement();
            delStop(activeCo);
        }

        base.CancelAction();
    }

    public override void SetCallbacks(DelSetItemType setTypeCB, DelStartCoroutine startCoCB, DelStopCoroutine stopCoCB) {
        delSet = setTypeCB;
        delStart = startCoCB;
        delStop = stopCoCB;
    }
}
