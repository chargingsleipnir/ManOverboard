using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ZeroProgress.Common.Collections;

public class Crewman : CharActionable {

    // TODO: Crewmen can scoop with each other's hats, which is fine, but a little buggy. Clean it up.
    // TODO: Figure out how exactly children donning life jackets is to be implemented. Click on them to do it, than an adult to put it on? Click on the adult to do it?
    // Or click on any person and change the amount of time. 5 secs for crewman, 10 secs for passenger adult, 30 seconds for child (With a probability of being on wrong?).
    // Might need better/more abstract command system, and perhaps to consider every Character as "Actionable" in some way.

    [SerializeField]
    protected ItemBase sailorHat;
    Vector3 hatPos;

    private ItemBaseSet items;
    [SerializeField]
    protected SpriteBase scoopBtnSprite;

    private bool blockScooping;

    protected override void Awake() {
        base.Awake();
        strength = 125;
        hatPos = sailorHat.transform.localPosition;
        items = AssetDatabase.LoadAssetAtPath<ItemBaseSet>("Assets/_ManOverboard/Variables/Sets/ItemBaseSet.asset");
    }
    protected override void Start() {
        base.Start();
        sailorHat.CharHeldBy = this;
        heldItems.Add(sailorHat);
        heldItemWeight += sailorHat.Weight;

        blockScooping = false;
    }

    private void ClickThroughHat(bool clickThrough) {
        RefShape2DMouseTracker[] trackers = sailorHat.GetComponents<RefShape2DMouseTracker>();
        for (int i = 0; i < trackers.Length; i++)
            trackers[i].clickThrough = clickThrough;
    }

    private void ReturnHatToHead() {
        sailorHat.SortCompFullReset();
        sailorHat.transform.localPosition = hatPos;
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

            int capWeight = (item as ItemCanScoop).capacity;
            float heldWeight = item.Weight + capWeight;
            float scoopRate = (heldWeight / strength) * heldWeight;
            if (scoopRate < Consts.MIN_SCOOP_RATE)
                scoopRate = Consts.MIN_SCOOP_RATE;

            activeCo = DelStartWaterRemove(capWeight, scoopRate);
        }
    }

    public override void CancelAction() {
        // TODO: Modify to reflect clearly that there may or may not be an active item
        if (state == Consts.CharState.Scooping) {
            if (activeItem == sailorHat)
                ReturnHatToHead();
            else {
                activeItem.Deselect();
                // Place item at feet of character just to their left.
                // TODO: Alter this to account for not every item having a refShape component? Shouldn't really come up though.
                float posX = RefShape.XMin - (activeItem.RefShape.Width * 0.5f) - Consts.ITEM_DROP_X_BUFF;
                float posY = RefShape.YMin + (activeItem.RefShape.Height * 0.5f);
                activeItem.transform.position = new Vector3(posX, posY, activeItem.transform.position.z);

                if (heldItems.Contains(activeItem)) {
                    heldItems.Remove(activeItem);
                    heldItemWeight -= activeItem.Weight;
                }
            }

            activeItem.EnableMouseTracking(true);
            DelStopWaterRemove(activeCo);
        }

        base.CancelAction();
    }
}
