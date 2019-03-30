using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using ZeroProgress.Common;
using ZeroProgress.Common.Collections;
using DragonBones;
using Transform = UnityEngine.Transform;
using Animation = DragonBones.Animation;


/*
Need a better way to see items/characters:
Translucent masking for items behind boat wall (bucket, life jacket, characters legs?) 

OR

Instead of masking, make is more general, and design the entire boat to be translucent anyway.
 */

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class CharBase : SpriteTossable, IMouseDownDetector, IMouseUpDetector {

    protected delegate void ActionCBs();
    protected ActionCBs ActionStep;
    protected ActionCBs ActionComplete;

    public ItemBase ItemHeld { get; set; }
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
    protected GameObject armatureObj;
    protected UnityArmatureComponent armaComp;
    protected Animation anim;

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

    Enemy enemy;

    protected override void Awake() {
        base.Awake();


        // Cannot serialize armature or animation components directly - must acquire through gameobject.
        if (armatureObj != null) {
            armaComp = armatureObj.GetComponent<UnityArmatureComponent>();
            anim = armaComp.animation;
        }

        //try {
        //    anim = armatureObj.GetComponent<UnityArmatureComponent>().animation;
        //}
        //catch(UnassignedReferenceException ure) {
        //    Debug.Log("No armature component");
        //}

        ActionStep = Action_Step;
        ActionComplete = Action_Complete;

        activeSkill = Consts.Skills.None;

        RefShape2DMouseTracker cancelBtnTracker = cancelBtnObj.GetComponent<RefShape2DMouseTracker>();

        cancelBtnTracker.AddMouseUpListener(CancelAction);
        cancelBtnTracker.AddMouseEnterListener(MouseUpToDownLinksFalse);
        cancelBtnTracker.AddMouseExitListener(MouseUpToDownLinksTrue);
    }

    // Always being overridden without base reference. Use Reset() for base referencing
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

        SortCompLayerChange(Consts.DrawLayers.BoatLevel1Mid);

        PlayAnim("Idle");
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
        else if(Airborne) {
            if (transform.position.y <= lvlMngr.WaterSurfaceYPos - Consts.OFFSCREEN_CATCH_BUFF) {
                WaterContact();
                Utility.RepositionY(transform, lvlMngr.WaterSurfaceYPos - Consts.OFFSCREEN_CATCH_BUFF);
                StopVel();
            }
        }
    }

    public override void OnClick() {
        PlayAnim("Idle");
        Held = false;
        if (CharState == Consts.CharState.Default) {
            OpenCommandPanel();
        }
        else if (CharState == Consts.CharState.InAction) {
            actHold = true;
            IsCancelBtnActive = true;
        }
    }

    protected override bool CheckImmExit() {
        return base.CheckImmExit() || CheckNonFuncStates();
    }
    private bool CheckNonFuncStates() {
        return CharState == Consts.CharState.Saved || CharState == Consts.CharState.Dazed || CharState == Consts.CharState.Dead;
    }

    protected void PlayAnim(string animation) {
        if(anim != null) {
            anim.Play(animation);
        }
    }
    protected void GoToFrame(string animation, uint frame = 0) {
        if (anim != null) {
            anim.GotoAndStopByFrame(animation, frame);
        }
    }

    public void IsHeld(bool held) {
        if (!Held) {
            Held = true;
            PlayAnim("Struggle");
        }
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
        ItemHeld = item;
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
        else if (item == ItemHeld) {
            ItemWeight -= item.Weight;
            ItemHeld = null;
        }
        item.InUse = false;
    }

    public override void Toss(Vector2 vel) {
        EndAction();
        base.Toss(vel);
    }

    private void Attack(Enemy e) {
        enemy = e;
        if(e is SeaSerpent) {
            activityCounter = activityInterval = Consts.ATTACK_RATE;
            ActionComplete = DamageEnemy;
            TakeAction();
        }
    }

    private void DamageEnemy() {
        enemy.TakeDamage(strength);
    }

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
        return CharState != Consts.CharState.InAction && !CheckNonFuncStates();
    }

    public void ReturnToGameState() {
        IsCancelBtnActive = false;
        IsCommandPanelOpen = false;
        actHold = false;
        Held = false;
        PlayAnim("Idle");
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

    public virtual void DropItemHeld() {
        if (ItemHeld == null)
            return;

        ItemWeight -= ItemHeld.Weight;
        ItemHeld.Deselect();

        // Place item at feet of character just to their left.
        // TODO: Alter this to account for not every item having a refShape component? Shouldn't really come up though.
        float posX = RefShape.XMin - (ItemHeld.RefShape.Width * 0.5f) - Consts.ITEM_DROP_X_BUFF;
        float posY = RefShape.YMin + (ItemHeld.RefShape.Height * 0.5f);
        ItemHeld.transform.position = new Vector3(posX, posY, ItemHeld.transform.position.z);

        // Turn this character's parent (the boat) into the new parent of the item, as it's just sitting on the floor of the boat now.
        lvlMngr.SetBoatAsParent(ItemHeld);

        ItemHeld.CharHeldBy = null;
        ItemHeld = null;
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
        PlayAnim("Happy");
    }

    private void WaterContact() {
        if (IsWearingLifeJacket) {
            SetStateSaved();
            lvlMngr.CharSaved(this);
        }
        else {
            // TODO: This might need a small delay to see if within a moment the character comes into contact with a ring buoy
            CharState = Consts.CharState.Dead;
            lvlMngr.CharKilled(this);
            GoToFrame("Dead");
        }
        Airborne = false;
    }

    private void Landed() {
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        bc.enabled = false;
        Airborne = false;
    }
    private void LandedAndSaved() {
        Landed();
        SetStateSaved();
        lvlMngr.CharSaved(this);
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (CheckImmExit())
            return;

        if (collider.gameObject.layer == (int)Consts.UnityLayers.Water) {
            WaterContact();
        }
        else if (collider.gameObject.layer == (int)Consts.UnityLayers.Envir) {
            ClingableSurface cs = collider.GetComponent<ClingableSurface>();
            if(cs.Cling(this)) {
                // TODO: Some of this will not be true in all cases, such as "SetStateSaved();" - this applies to cave stalactites, but in other situations might not.
                SortCompLayerChange(Consts.DrawLayers.BehindBoat, null);
                LandedAndSaved();
            }
        }
        else if (collider.gameObject.layer == (int)Consts.UnityLayers.Enemy) {
            // This is presuming enemy has/is clingable surface
            ClingableSurface cs = collider.GetComponent<ClingableSurface>();
            // TODO: Clinging allows for timed/sustained attack (5 seconds, 1 hit per second, for example), while not cliniging results in single impact damage
            if (cs.Cling(this)) {
                Landed();
                Attack(collider.GetComponent<Enemy>());
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collider) {
        if (CheckImmExit())
            return;

        if (collider.gameObject.layer == (int)Consts.UnityLayers.FloatDev) {
            // TODO: Something better than this - temporary setup.
            transform.position = collider.transform.position;
            transform.parent = collider.transform;

            LandedAndSaved();
        }
    }
}
