using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ZeroProgress.Common.Collections;

public class Crewman : CharActionable {

    [SerializeField]
    protected ItemBase sailorHat;

    private ItemBaseSet items;
    [SerializeField]
    protected SpriteBase scoopBtnSprite;

    private bool blockScooping;

    int capWeight;

    protected override void Awake() {
        base.Awake();
        strength = 125;
        capWeight = 0;
        sailorHat.RetPosLocal = sailorHat.transform.localPosition;
        items = AssetDatabase.LoadAssetAtPath<ItemBaseSet>("Assets/_ManOverboard/Variables/Sets/ItemBaseSet.asset");
    }
    protected override void Start() {
        base.Start();
        sailorHat.CharHeldBy = this;
        heldItems.Add(sailorHat);
        heldItemWeight += sailorHat.Weight;

        blockScooping = false;
    }

    protected override void Update() {
        if (Paused)
            return;

        if (!held) {
            if (state == Consts.CharState.Scooping) {
                activityCounter -= Time.deltaTime;
                float counterPct = 1.0f - (activityCounter / activityInterval);
                timerBar.Fill = counterPct;
                if (activityCounter <= 0) {
                    activityCounter = activityInterval;
                    DelRemoveWater(capWeight);
                }
            }
        }

        base.Update();
    }

    private void ClickThroughHat(bool clickThrough) {
        RefShape2DMouseTracker[] trackers = sailorHat.GetComponents<RefShape2DMouseTracker>();
        for (int i = 0; i < trackers.Length; i++)
            trackers[i].clickThrough = clickThrough;
    }

    public override void CheckCommandViability() {
        if (items.Count > 0) {
            blockScooping = false;
            scoopBtnSprite.ChangeColour(null, null, null, 1.0f);
        }
        else {
            blockScooping = true;
            scoopBtnSprite.ChangeColour(null, null, null, Consts.BTN_DISABLE_FADE);
        }

        // TODO: Follow up with any other command buttons.
    }

    public void PrepScoop() {
        if (blockScooping)
            return;

        ClickThroughHat(false);

        IsCommandPanelOpen = false;
        DelSetItemType(Consts.ItemType.Scooping);
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
            state = Consts.CharState.Scooping;

            // Logic for scooping wth item
            item.transform.position = trans_ItemUseHand.position;
            item.transform.parent = trans_ItemUseHand.parent;
            //ClickThroughHat(true);

            capWeight = (item as ItemCanScoop).capacity;
            float heldWeight = item.Weight + capWeight;
            float scoopRate = (heldWeight / strength) * heldWeight;
            if (scoopRate < Consts.MIN_SCOOP_RATE)
                scoopRate = Consts.MIN_SCOOP_RATE;

            activityCounter = activityInterval = scoopRate;
        }
    }

    public override void CancelAction() {
        if (activeItem == sailorHat) {
            sailorHat.Deselect();
            activeItem = null;
        }

        base.CancelAction();
    }
}
