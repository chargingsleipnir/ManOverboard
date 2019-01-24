using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharElder : CharChild {

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

    public override void SetActionBtns() {
        commandPanel.InactiveAwake();

        // TODO: Need to consider that a single button should apply to both, jacketting a child or self
        if (lvlMngr.CheckCanDonLifeJacketChildren(true) || lvlMngr.CheckCanDonLifeJacketAdults(false)) {
            canAct = true;
            canDonLifeJacket = true;
            commandPanel.PrepBtn(Consts.Skills.DonLifeJacket, PrepDonLifeJacket);
        }
        if(lvlMngr.CheckCanScoop()) {
            canAct = true;
            canScoop = true;

            commandPanel.PrepBtn(Consts.Skills.ScoopWater, PrepScoop);
        }

        // TODO: Requires bool for job specific options
        if(job != null)
            job.SetActionBtns();

        commandPanel.SetBtns();
    }
    public override void CheckActions() {
        if (canDonLifeJacket)
            commandPanel.EnableBtn(Consts.Skills.DonLifeJacket, lvlMngr.CheckCanDonLifeJacketChildren(true) || lvlMngr.CheckCanDonLifeJacketAdults(false));

        if (canScoop)
            commandPanel.EnableBtn(Consts.Skills.ScoopWater, lvlMngr.CheckCanScoop());
    }

    // TODO: This whole function needs to be split up, separating the objects being prepared, from the actions to be taken with them.
    public override void GetSelection(SpriteBase sprite) {
        if (activeSkill == Consts.Skills.ScoopWater) {
            ItemCanScoop scoop = sprite as ItemCanScoop;
            activeItem = scoop;
            activeItem.EnableMouseTracking(false);

            ItemBase item = sprite as ItemBase;
            if (!heldItems.Contains(item)) {
                heldItems.Add(item);
                heldItemWeight += item.Weight;
            }

            lvlMngr.ResetEnvir();

            // Logic for scooping wth item
            scoop.transform.position = trans_ItemUseHand.position;
            scoop.transform.parent = trans_ItemUseHand.parent;

            waterWeight = scoop.capacity;
            float heldWeight = scoop.Weight + waterWeight;
            float scoopRate = (heldWeight / strength) * heldWeight;
            if (scoopRate < Consts.MIN_SCOOP_RATE)
                scoopRate = Consts.MIN_SCOOP_RATE;

            activityCounter = activityInterval = scoopRate;
            ActionComplete = CompleteSingleScoop;

            timerBar.IsActive = true;
            charState = Consts.CharState.InAction;
        }
        else if (activeSkill == Consts.Skills.DonLifeJacket) {
            if(sprite is LifeJacket) {
                LifeJacket jacket = sprite as LifeJacket;

                // Adult jacket, can don self immediately
                if (jacket.size == Consts.FitSizes.adult) {
                    activeItem = jacket;
                    activeItem.EnableMouseTracking(false);

                    ItemBase item = sprite as ItemBase;
                    if (!heldItems.Contains(item)) {
                        heldItems.Add(item);
                        heldItemWeight += item.Weight;
                    }

                    lvlMngr.ResetEnvir();

                    // Place life jacket in hand and start donning timer
                    jacket.transform.position = trans_ItemUseHand.position;
                    jacket.transform.parent = trans_ItemUseHand.parent;

                    activityCounter = activityInterval = Consts.DON_RATE;
                    ActionComplete = CompleteDonLifeJacket;

                    timerBar.IsActive = true;
                    charState = Consts.CharState.InAction;
                }
                // Child jacket, need to wait for child to be selected
                else {
                    actObjQueue.Add(sprite);
                    lvlMngr.HighlightToSelect(Consts.HighlightGroupType.Children);
                }
            }
            if(sprite is CharChild) {
                // TODO: These lines are delayed so that the "activeItem" isn't set before potential action is fully underway. Is this really a necessary precaution?
                // Just make sure it can be unset if the action is not fully set into motion, without any hiccups.
                // !! Queue being cleared in Charbase EndAction()
                activeItem = actObjQueue[0] as ItemBase;
                activeItem.EnableMouseTracking(false);

                lvlMngr.ResetEnvir();

                activeItem.transform.position = trans_ItemUseHand.position;
                activeItem.transform.parent = trans_ItemUseHand.parent;

                actObjQueue.Add(sprite);

                activityCounter = activityInterval = Consts.DON_RATE;
                ActionComplete = CompleteDonLifeJacketChild;

                timerBar.IsActive = true;
                charState = Consts.CharState.InAction;

                // TODO:
                // - Adult needs to move(animate) to child
                // - Timer bar needs to activate
                // - Child needs to be unclickable
                // - Need to account for what will happen to everything if player clicks to cancel after at least one selection has been made
            }
        }
    }

    protected override void PrepDonLifeJacket() {
        PrepAction(Consts.Skills.DonLifeJacket);
        lvlMngr.HighlightToSelect(Consts.HighlightGroupType.LifeJacket);
    }
    private void CompleteDonLifeJacketChild() {
        // TODO: Just set in center of self for now, will need proper location around center of torso later
        activeItem.transform.position = actObjQueue[1].transform.position;
        activeItem.transform.parent = actObjQueue[1].transform;
        EndAction();
    }

    public virtual void PrepScoop() {
        PrepAction(Consts.Skills.ScoopWater);
        lvlMngr.HighlightToSelect(Consts.HighlightGroupType.Scooping);
    }
    private void CompleteSingleScoop() {
        lvlMngr.RemoveWater(waterWeight);
    }
}
