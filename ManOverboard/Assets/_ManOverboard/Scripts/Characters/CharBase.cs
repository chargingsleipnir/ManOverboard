using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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

    protected delegate void ActionCBs();
    protected ActionCBs ActionStep;
    protected ActionCBs ActionComplete;

    protected ItemBase itemHeld;
    protected CharBase activeChar;
    protected List<ItemBase> itemsWorn;
    public bool IsWearingLifeJacket { get; set; }

    public int ItemWeight { get; set; }
    public override int Weight {
        get { return weight + ItemWeight; }
    }

    public Consts.CharState CharState { get; set; }
    protected Consts.Skills activeSkill;
    protected bool canAct = false;
    private bool actHold;

    public int strength;
    public int speed;

    protected float activityCounter;
    protected float activityInterval;

    [SerializeField]
    protected ProgressBar timerBar;

    [SerializeField]
    protected Transform trans_ItemUseHand;

    [SerializeField]
    protected GameObject cancelBtnObj;
    public bool IsCancelBtnActive {
        get { return cancelBtnObj.activeSelf; }
        set { cancelBtnObj.SetActive(value); }
    }

    [SerializeField]
    protected CommandPanel commandPanel;
    public bool IsCommandPanelOpen {
        get { return commandPanel.gameObject.activeSelf; }
        set {
            commandPanel.gameObject.SetActive(value);
            if (value == true)
                CharState = Consts.CharState.InMenu;
            else if (CharState == Consts.CharState.InMenu)
                CharState = Consts.CharState.Default;
        }
    }

    protected override void Awake() {
        base.Awake();

        ActionStep = Action_Step;
        ActionComplete = Action_Complete;

        activeSkill = Consts.Skills.None;

        RefShape2DMouseTracker cancelBtnTracker = cancelBtnObj.GetComponent<RefShape2DMouseTracker>();

        cancelBtnTracker.AddMouseUpListener(CancelAction);
        cancelBtnTracker.AddMouseEnterListener(MouseUpToDownLinksFalse);
        cancelBtnTracker.AddMouseExitListener(MouseUpToDownLinksTrue);
    }

    protected override void Start () {
        strength = 0;
        speed = 0;
        Reset();
    }

    protected override void Reset() {
        base.Reset();

        CharState = Consts.CharState.Default;
        itemsWorn = new List<ItemBase>();

        actHold = false;

        ItemWeight = 0;
        timerBar.Fill = 0;
        activityCounter = 0.0f;
        activityInterval = 0.0f;

        SortCompLayerChange(Consts.DrawLayers.BoatLevel1Contents);
    }

    protected override void Update() {
        if (CheckImmExit())
            return;

        if (!actHold && CharState == Consts.CharState.InAction) {
            activityCounter -= Time.deltaTime * speed;
            float counterPct = 1.0f - (activityCounter / activityInterval);
            timerBar.Fill = counterPct;
            //ActionStep(); <-- Simply nothing using it yet
            if (activityCounter <= 0) {
                activityCounter = activityInterval;
                ActionComplete();
            }
        }
    }

    public override void OnClick() {
        if (CharState == Consts.CharState.Default) {
            OpenCommandPanel();
        }
        else if (CharState == Consts.CharState.InAction) {
            actHold = true;
            IsCancelBtnActive = true;
        }
    }

    protected override bool CheckImmExit() {
        return base.CheckImmExit() || CharState == Consts.CharState.Saved || CharState == Consts.CharState.Dazed;
    }

    public void WearItem(ItemBase item) {
        itemsWorn.Add(item);
        ItemWeight += item.Weight;
        item.CharHeldBy = this;
    }
    public bool IsWearingItem(ItemBase item) {
        return itemsWorn.Contains(item);
    }
    public void HoldItem(ItemBase item) {
        itemHeld = item;
        ItemWeight += item.Weight;
        item.InUse = true;

        item.EnableMouseTracking(false);
        item.transform.position = trans_ItemUseHand.position;
        item.transform.parent = transform;
    }
    // BOOKMARK
    public virtual void LoseItem(ItemBase item) {
        if (itemsWorn.Remove(item)) {
            ItemWeight -= item.Weight;
        }
        else if (item == itemHeld) {
            ItemWeight -= item.Weight;
            itemHeld = null;
        }
        item.InUse = false;
    }

    public override void Toss(Vector2 vel) {
        EndAction();
        base.Toss(vel);

        if (itemHeld != null) {
            itemHeld.Toss(vel);
            lvlMngr.RemoveItem(itemHeld);
        }
    }

    // TODO: Check if given commands are available, and disable buttons if not.
    // In the case of crewman, the button for scooping water should be disabled if no scooping items are available.

    public virtual void SetActionBtns() { }
    public virtual void CheckActions() { }

    // Essentially just placeholders to never have to check for null
    private void Action_Step() { }
    private void Action_Complete() { }

    public void OpenCommandPanel() {
        if (!canAct)
            return;

        CheckActions();
        MouseUpToDownLinksTrue();

        IsCommandPanelOpen = true;

        lvlMngr.FadeLevel();
    }
    protected void PrepAction(Consts.Skills activeSkill) {
        IsCommandPanelOpen = false;
        ReturnToBoat();
        this.activeSkill = activeSkill;
    }
    public bool CheckAvailToAct() {
        return CharState != Consts.CharState.InAction && CharState != Consts.CharState.Saved;
    }

    public void ReturnToGameState() {
        IsCancelBtnActive = false;
        IsCommandPanelOpen = false;
        actHold = false;
    }
    public void ReturnToNeutral() {
        ReturnToGameState();
        CancelAction();
        ChangeMouseUpToDownLinks(true);
    }

    protected void TakeAction() {
        // Remove removable items (tools, small objects) from scene lists
        lvlMngr.ConfirmSelections(this);
        timerBar.IsActive = true;
        CharState = Consts.CharState.InAction;
    }
    public virtual void CancelAction() {
        if (CharState != Consts.CharState.InAction)
            return;

        MouseUpToDownLinksTrue();
        DropItemHeld();
        ReturnToBoat();
        EndAction();
        lvlMngr.UnfadeLevel();
    }
    // BOOKMARK
    public virtual void DropItemHeld() {
        if (itemHeld == null)
            return;

        ItemWeight -= itemHeld.Weight;
        itemHeld.Deselect();

        // Place item at feet of character just to their left.
        // TODO: Alter this to account for not every item having a refShape component? Shouldn't really come up though.
        float posX = RefShape.XMin - (itemHeld.RefShape.Width * 0.5f) - Consts.ITEM_DROP_X_BUFF;
        float posY = RefShape.YMin + (itemHeld.RefShape.Height * 0.5f);
        itemHeld.transform.position = new Vector3(posX, posY, itemHeld.transform.position.z);

        // Turn this character's parent (the boat) into the new parent of the item, as it's just sitting on the floor of the boat now.
        lvlMngr.SetBoatAsParent(itemHeld);

        itemHeld.CharHeldBy = null;
        itemHeld = null;
    }

    public virtual void EndAction() {
        activityCounter = activityInterval = 0;
        timerBar.IsActive = false;
        IsCancelBtnActive = false;
        

        if (activeChar != null) {
            activeChar.SetStateDazed(false);
            activeChar.ReturnToNeutral();            
            activeChar = null;
        }

        if (CharState != Consts.CharState.Saved) {
            CharState = Consts.CharState.Default;
            actHold = false;
        }

        activeSkill = Consts.Skills.None;
    }

    public void SetStateDazed(bool isDazed) {
        if (isDazed) {
            CharState = Consts.CharState.Dazed;
            ChangeColour(null, null, null, 0.5f);
        }
        else {
            CharState = Consts.CharState.Default;
            ChangeColour(null, null, null, 1.0f);
        }
    }
    public void SetStateSaved() {
        CancelAction();
        CharState = Consts.CharState.Saved;
    }
}
