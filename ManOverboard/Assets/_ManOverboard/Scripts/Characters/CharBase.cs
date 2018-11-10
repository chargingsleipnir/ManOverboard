﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ZeroProgress.Common;
using ZeroProgress.Common.Collections;


/* TODO: Get rid of CharActionable - all characters will be potentially actionable at some point in some way. I'm better off to check what the scene is like
 * (what items are available, etc) and adjust each characters settings for that level at that point (like whether or not the action button will appear)
 * 
 * Have children be able to don their own life jackets at about 30 seconds, or click on any adult and change the amount of time. 5 secs for crewman, 10 secs for passenger, etc.
 * The click to have an adult do it should come from the adult themselves:
 * Click adult,
 * Click life jacket button, all life jackets highlight.
 * If adult jacket is clicked, it's donned. 
 * Maybe Clicking the life jacket button again will allow your own to be clicked to remove. Maybe a second hold & release button needs to be available for quicker doffing
 * If child jacket is clicked, kids are highlighted to put in on.
 * 
 * Show a meter at one side that’s like a close-up of the water level reaching the next hole or boat-level-ledge, as this is very hard to see as is, 
 * and once the water goes over, that level is flooded/lost, whatever exactly that means right now.
 * 
 * Need a better way to see items/characters:
Translucent masking for items behind boat wall (bucket, life jacket, characters legs?) 

OR

Instead of masking, make is more general, and design the entire boat to be translucent anyway.

 */


[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class CharBase : SpriteTossable, IMouseDownDetector, IMouseUpDetector {

    protected Consts.CharState state;
    protected bool canAct;

    public delegate void DelPassItemType(Consts.ItemType type);
    public delegate void DelPassInt(int waterWeight);

    protected DelPassItemType DelSetItemType;
    protected DelPassInt DelRemoveWater;
    protected DelVoid FadeLevel;
    protected DelVoid UnFadeLevel;

    protected int strength;
    protected int speed;

    protected ItemBase activeItem;
    protected List<ItemBase> heldItems;
    protected ItemBaseSet levelItems;

    protected int heldItemWeight;
    public override int Weight {
        get { return weight + heldItemWeight; }
    }

    protected bool saved = false;
    public bool Saved {
        get {
            return saved;
        }
        set {
            saved = value;
            if (value) {
                CancelAction();
            }
        }
    }

    public bool Paused { get; set; }
    protected float activityCounter;
    protected float activityInterval;

    [SerializeField]
    protected ProgressBar timerBar;

    [SerializeField]
    protected Transform trans_ItemUseHand;

    [SerializeField]
    protected GameObject actionBtnObj;
    public bool IsActionBtnActive {
        get { return actionBtnObj.activeSelf; }
        set { actionBtnObj.SetActive(value); }
    }
    private SpriteRenderer actBtnSR;
    protected bool GUIActive;
    protected Rect actBtnRect;
    protected Rect cmdPanelRect;

    [SerializeField]
    protected GameObject cancelBtnObj;
    public bool IsCancelBtnActive {
        get { return cancelBtnObj.activeSelf; }
        set { cancelBtnObj.SetActive(value); }
    }

    [SerializeField]
    protected GameObject commandPanel;
    public bool IsCommandPanelOpen {
        get { return commandPanel.activeSelf; }
        set {
            commandPanel.SetActive(value);
            if (value == true)
                state = Consts.CharState.MenuOpen;
            else if (state == Consts.CharState.MenuOpen)
                state = Consts.CharState.Default;
        }
    }

    protected override void Awake() {
        base.Awake();
        
        actBtnSR = actionBtnObj.GetComponent<SpriteRenderer>();
        levelItems = AssetDatabase.LoadAssetAtPath<ItemBaseSet>("Assets/_ManOverboard/Variables/Sets/ItemBaseSet.asset");
    }

    protected override void Start () {
        strength = 0;
        speed = 0;
        Reset();
    }

    protected override void Reset() {
        base.Reset();

        state = Consts.CharState.Default;
        heldItems = new List<ItemBase>();

        heldItemWeight = 0;
        timerBar.Fill = 0;
        activityCounter = 0.0f;
        activityInterval = 0.0f;

        SortCompLayerChange(Consts.DrawLayers.BoatLevel1Contents);
    }

    public override void MouseDownCB() {
        if (IsCommandPanelOpen || saved || tossed)
            return;

        held = true;

        if (state == Consts.CharState.Default)
            IsActionBtnActive = true;
        else {
            IsCancelBtnActive = true;
            timerBar.IsActive = false;
        }

        OnMouseDownCB(gameObject);
    }
    public override void MouseUpCB() {
        if (IsCommandPanelOpen || saved || tossed || held == false)
            return;

        held = false;

        if (state == Consts.CharState.Default)
            IsActionBtnActive = false;
        else {
            IsCancelBtnActive = false;
            if (state == Consts.CharState.Scooping)
                timerBar.IsActive = true;
        }

        OnMouseUpCB();
    }

    public void LoseItem(ItemBase item) {
        if (heldItems.Remove(item))
            heldItemWeight -= item.Weight;
    }

    public override void ApplyTransformToContArea(GameObject contAreaObj, bool prioritizeRefShape) {
        if(!canAct) {
            base.ApplyTransformToContArea(contAreaObj, prioritizeRefShape);
            return;
        }

        actBtnRect = new Rect(
            actionBtnObj.transform.position.x,
            actionBtnObj.transform.position.y,
            actBtnSR.size.x * actionBtnObj.transform.lossyScale.x,
            actBtnSR.size.y * actionBtnObj.transform.lossyScale.y
        );

        float btnTopToCharTop = actBtnRect.height > 0 ? actBtnRect.yMax - (actBtnSR.size.y * 0.5f) - (transform.position.y + (srRef.comp.sprite.bounds.size.y * 0.5f)) : 0;
        contAreaObj.transform.position = new Vector3(transform.position.x, transform.position.y + (btnTopToCharTop * 0.5f), (float)Consts.ZLayers.FrontOfWater);

        if(prioritizeRefShape) {
            RefShape shape = GetComponent<RefShape>();
            if (shape != null) {
                contAreaObj.transform.localScale = new Vector3(Utility.GreaterOf(shape.Width, actBtnRect.width) + Consts.CONT_AREA_BUFFER, shape.Height + btnTopToCharTop + Consts.CONT_AREA_BUFFER, 1);
                return;
            }
        }
        contAreaObj.transform.localScale = new Vector3(Utility.GreaterOf(srRef.comp.sprite.bounds.size.x, actBtnRect.width) + Consts.CONT_AREA_BUFFER, srRef.comp.sprite.bounds.size.y + btnTopToCharTop + Consts.CONT_AREA_BUFFER, 1);
    }

    public virtual void UseItem(ItemBase item) { }
    public void SetCallbacks(DelPassItemType setTypeCB, DelPassInt RemoveWaterCB, DelVoid fadeLevelCB, DelVoid unfadeLevelCB) {
        DelSetItemType = setTypeCB;
        DelRemoveWater = RemoveWaterCB;
        FadeLevel = fadeLevelCB;
        UnFadeLevel = unfadeLevelCB;
    }

    public override void OverheadButtonActive(bool isActive) {
        if (state == Consts.CharState.Default)
            IsActionBtnActive = isActive;
        else
            IsCancelBtnActive = isActive;
    }

    // TODO: Check if given commands are available, and disable buttons if not.
    // In the case of crewman, the button for scooping water should be disabled if no scooping items are available.

    public virtual void CheckCanAct(bool childrenDonLifeJacket, bool adultDonLifeJacket, bool canScoop) {}

    public void OpenCommandPanel() {
        held = false;
        IsCommandPanelOpen = true;
        IsActionBtnActive = false;

        // TODO: Need to keep checking that char can act (because items are removed & characters are tossed)
        //CheckCanAct();

        FadeLevel();
    }

    public virtual void CancelAction() {
        if (activeItem != null) {
            activeItem.Deselect();

            if (activeItem.RetPosLocal == null) {
                // Place item at feet of character just to their left.
                // TODO: Alter this to account for not every item having a refShape component? Shouldn't really come up though.
                float posX = RefShape.XMin - (activeItem.RefShape.Width * 0.5f) - Consts.ITEM_DROP_X_BUFF;
                float posY = RefShape.YMin + (activeItem.RefShape.Height * 0.5f);
                activeItem.transform.position = new Vector3(posX, posY, activeItem.transform.position.z);
            }

            if (heldItems.Contains(activeItem)) {
                heldItems.Remove(activeItem);
                heldItemWeight -= activeItem.Weight;
            }
        }

        activityCounter = activityInterval = 0;
        timerBar.IsActive = false;
        IsCancelBtnActive = false;
        state = Consts.CharState.Default;
        UnFadeLevel();
    }
}
