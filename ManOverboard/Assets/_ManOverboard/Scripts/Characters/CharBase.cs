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

    protected List<SpriteBase> actObjQueue;

    protected Consts.CharState charState;
    protected Consts.Skills activeSkill;
    protected bool canAct = false;

    protected int strength;
    protected int speed;

    public int Strength { get; set; }
    public int Speed { get; set; }

    protected ItemBaseSet levelItems;
    protected ItemBase activeItem;
    protected List<ItemBase> heldItems;

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
            if (value)
                CancelAction();
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
    protected CommandPanel commandPanel;
    public bool IsCommandPanelOpen {
        get { return commandPanel.gameObject.activeSelf; }
        set {
            commandPanel.gameObject.SetActive(value);
            if (value == true)
                charState = Consts.CharState.InMenu;
            else if (charState == Consts.CharState.InMenu)
                charState = Consts.CharState.Default;
        }
    }

    protected override void Awake() {
        base.Awake();

        ActionStep = Action_Step;
        ActionComplete = Action_Complete;

        activeSkill = Consts.Skills.None;
        actObjQueue = new List<SpriteBase>();

        levelItems = Resources.Load<ItemBaseSet>("ScriptableObjects/SpriteSets/ItemBaseSet");
        actBtnSR = actionBtnObj.GetComponent<SpriteRenderer>();

        RefShape2DMouseTracker actionBtnTracker = actionBtnObj.GetComponent<RefShape2DMouseTracker>();

        // These are set here, the way they are, to help prevent from having to do this in the inspector for every individual character prefab variant (setting these publically for the base character prefab does not do the job properly)
        actionBtnTracker.AddMouseUpListener(OpenCommandPanel);
        actionBtnTracker.AddMouseEnterListener(MouseUpToDownLinksFalse);
        actionBtnTracker.AddMouseExitListener(MouseUpToDownLinksTrue);

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

        charState = Consts.CharState.Default;
        heldItems = new List<ItemBase>();

        heldItemWeight = 0;
        timerBar.Fill = 0;
        activityCounter = 0.0f;
        activityInterval = 0.0f;

        SortCompLayerChange(Consts.DrawLayers.BoatLevel1Contents);
    }

    protected override void Update() {
        if (Paused)
            return;

        if (tossableState == Consts.SpriteTossableState.Default) {
            if (charState == Consts.CharState.InAction) {
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
    }

    public override void MouseDownCB() {
        if (CheckImmClickExit())
            return;

        if (selectable)
            return;

        if (charState == Consts.CharState.Default) {
            IsActionBtnActive = true;
            tossableState = Consts.SpriteTossableState.Held;
            lvlMngr.OnSpriteMouseDown(gameObject);
        }
        else if (charState == Consts.CharState.InAction) {
            IsCancelBtnActive = true;
            timerBar.IsActive = false;
            tossableState = Consts.SpriteTossableState.Held;
            lvlMngr.OnSpriteMouseDown(gameObject);
        }
        // If in menu
        // If paused
        // If selectable?
    }
    public override void MouseUpCB() {
        if (CheckImmClickExit())
            return;

        if (selectable) {
            lvlMngr.OnSelection(this);
            return;
        }

        if (tossableState == Consts.SpriteTossableState.Held) {
            if (charState == Consts.CharState.Default) {
                IsActionBtnActive = false;
                tossableState = Consts.SpriteTossableState.Default;
                lvlMngr.OnSpriteMouseUp();
            }
            else if (charState == Consts.CharState.InAction) {
                IsCancelBtnActive = false;
                timerBar.IsActive = true;
                tossableState = Consts.SpriteTossableState.Default;
                lvlMngr.OnSpriteMouseUp();
            }
        }
        // If in menu
        // If paused
        // If selectable?
    }
    protected override bool CheckImmClickExit() {
        return base.CheckImmClickExit() || charState == Consts.CharState.Saved;
    }

    public void HoldItem(ItemBase item, bool inHand) {
        heldItems.Add(item);
        heldItemWeight += item.Weight;

        if(inHand) {
            // TODO: Place item location in character's hand, in front of character
        }
    }
    public void LoseItem(ItemBase item) {
        if (heldItems.Remove(item))
            heldItemWeight -= item.Weight;
    }

    public override void Toss(Vector2 vel) {
        EndAction();
        base.Toss(vel);
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

    public virtual void GetSelection(SpriteBase sprite) { }

    public override void OverheadButtonActive(bool isActive) {
        if (charState == Consts.CharState.Default)
            IsActionBtnActive = isActive;
        else
            IsCancelBtnActive = isActive;
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

        tossableState = Consts.SpriteTossableState.Default;
        IsCommandPanelOpen = true;
        IsActionBtnActive = false;

        lvlMngr.FadeLevel();
    }
    protected void PrepAction(Consts.Skills activeSkill) {
        IsCommandPanelOpen = false;
        ReturnToBoat();
        this.activeSkill = activeSkill;
    }

    public void ReturnToNeutral() {
        IsActionBtnActive = false;
        IsCancelBtnActive = false;
        IsCommandPanelOpen = false;
        CancelAction();
        ChangeMouseUpToDownLinks(true);
    }

    public virtual void CancelAction() {
        if (activeItem != null) {

            // Runs callback in level manager to put item back into list
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

        ReturnToBoat();
        EndAction();
        lvlMngr.UnfadeLevel();
    }

    public virtual void EndAction() {
        activityCounter = activityInterval = 0;
        timerBar.IsActive = false;
        IsCancelBtnActive = false;
        actObjQueue.Clear();
        activeItem = null; // The "active" item is only that which is used during an action which is still cancellable
        charState = Consts.CharState.Default;
        activeSkill = Consts.Skills.None;
    }
}
