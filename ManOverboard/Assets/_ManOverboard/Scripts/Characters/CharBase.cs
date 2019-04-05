using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using ZeroProgress.Common;
using ZeroProgress.Common.Collections;


/*
Need a better way to see items/characters:
Translucent masking for items behind boat wall (bucket, life jacket, characters legs?) 

OR

Instead of masking, make is more general, and design the entire boat to be translucent anyway.
 */

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class CharBase : SpriteTossable, IMouseDownDetector, IMouseUpDetector {

    
    protected Action TaskStep;
    protected Action TaskComplete;

    public override bool Paused {
        get { return paused; }
        set {
            paused = value;
            if (paused) AnimSpeed(0);
            else AnimSpeed(1);
        }
    }

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
    protected int moveDir;

    protected float taskCounter;
    protected float taskInterval;

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

    Enemy enemy;

    protected Animator animator;
    protected string prevAnim;

    protected override void Awake() {
        base.Awake();

        animator = GetComponent<Animator>();
        prevAnim = "Idle";

        
        TaskStep = Action_Step;
        TaskComplete = Action_Complete;

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
        moveDir = 1;

        Reset();
    }

    protected override void Reset() {
        base.Reset();

        CharState = Consts.CharState.Default;
        itemsWorn = new List<ItemBase>();

        actHold = false;

        ItemWeight = 0;
        timerBar.Fill = 0;
        taskCounter = 0.0f;
        taskInterval = 0.0f;

        SortCompLayerChange(Consts.DrawLayers.BoatLevel1Mid);
    }

    protected override void Update() {
        if (CheckImmExit())
            return;

        if (!actHold) {
            if (CharState == Consts.CharState.Walking) {
                if (mouseDown_Unmoved)
                    return;

                // Walking update code specific to selection process
                float moveStepDist = (speed * Consts.MOVE_SPEED_REDUC) * Time.deltaTime;
                float posDelta = Math.Abs(transform.position.x - steps[0].sprite.transform.position.x);

                // In case the contact range could be overstepped (smaller than the step size)
                if (moveStepDist > steps[0].minSelReachDist) {
                    // If a full step can still be taken, take it.
                    if (moveStepDist < posDelta + steps[0].minSelReachDist)
                        Utility.RepositionX(transform, transform.position.x + (moveStepDist * moveDir));
                    // Otherwise go straight to the target
                    else
                        Utility.RepositionX(transform, steps[0].sprite.transform.position.x);
                }
                else
                    Utility.RepositionX(transform, transform.position.x + (moveStepDist * moveDir));

                if (CheckSpriteContact())
                    OnSpriteContact();
            }
            else if (CharState == Consts.CharState.InAction) {
                taskCounter -= Time.deltaTime * speed;
                float counterPct = 1.0f - (taskCounter / taskInterval);
                timerBar.Fill = counterPct;
                TaskStep();
                if (taskCounter <= 0) {
                    taskCounter = taskInterval;
                    TaskComplete();
                }
            }
        }
        else if (Airborne) {
            if (transform.position.y <= lvlMngr.WaterSurfaceYPos - Consts.OFFSCREEN_CATCH_BUFF) {
                WaterContact();
                Utility.RepositionY(transform, lvlMngr.WaterSurfaceYPos - Consts.OFFSCREEN_CATCH_BUFF);
                StopVel();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (CheckImmExit())
            return;

        if (collider.gameObject.layer == (int)Consts.UnityLayers.Water) {
            WaterContact();
            AnimSetBool("InWater", true);
        }
        else if (collider.gameObject.layer == (int)Consts.UnityLayers.Envir) {
            ClingableSurface cs = collider.GetComponent<ClingableSurface>();
            if (cs.Cling(this)) {
                // TODO: Some of this will not be true in all cases, such as "SetStateSaved();" - this applies to cave stalactites, but in other situations might not.
                SortCompLayerChange(Consts.DrawLayers.BehindBoat, null);
                LandedAndSaved();
                AnimSetBool("Hanging", true);
            }
        }
        else if (collider.gameObject.layer == (int)Consts.UnityLayers.Enemy) {
            // This is presuming enemy has/is clingable surface
            ClingableSurface cs = collider.GetComponent<ClingableSurface>();
            // TODO: Clinging allows for timed/sustained attack (5 seconds, 1 hit per second, for example), while not cliniging results in single impact damage
            if (cs.Cling(this)) {
                Landed();
                Attack(collider.GetComponent<Enemy>());
                AnimSetBool("Hanging", true);
                AnimTrigger("Attack");
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

            // TODO: This is just for now, as the only floatation device is the ring buoy, which still leaves the character visibly in the water, making this animation work.
            AnimSetBool("InWater", true);

            LandedAndSaved();
        }
    }

    public override void OnClick() {
        AnimSpeed(0);

        Held = false;
        if (CharState == Consts.CharState.Default) {
            OpenCommandPanel();
        }
        else if (CharState == Consts.CharState.InAction || CharState == Consts.CharState.Walking) {
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

    protected void AnimSpeed(float speed) {
        if (animator != null)
            animator.speed = speed;
    }
    protected void AnimTrigger(string animation) {
        if (animator != null) {
            prevAnim = animation;
            animator.SetTrigger(animation);
        }
    }
    protected void AnimSetBool(string anim, bool value) {
        if (animator != null)
            animator.SetBool(anim, value);
    }
    protected void SetFramePct(float pct) {
        if(animator != null) {
            animator.SetFloat("FramePct", pct);
        }
    }
    protected void StepTimerBarFill() {
        SetFramePct(timerBar.Fill);
    }

    public void Grab() {
        if (!Held) {
            Held = true;
            mouseDown_Unmoved = false;
            actHold = true;
            // Using this specifically so as NOT to record this animation, to easily go back to the previous one.
            if (animator != null)
                animator.SetTrigger("Grabbed");
        }
    }
    public void Release() {
        AnimTrigger(prevAnim);
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

        lvlMngr.ConfirmItemSelection(this, item);

        item.InUse = true;
        item.CharHeldBy = this;
        item.EnableMouseTracking(false);
        item.MoveToCharHand(trans_ItemUseHand);
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
        base.Toss(vel);
        EndAction();        
    }

    private void Attack(Enemy e) {
        enemy = e;
        if(e is SeaSerpent) {
            taskCounter = taskInterval = Consts.ATTACK_RATE;
            TaskStep = StepTimerBarFill;
            TaskComplete = DamageEnemy;
            timerBar.IsActive = true;
            CharState = Consts.CharState.InAction;
            AnimSpeed(1);
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
    public bool CheckAvailToAct() {
        return CharState != Consts.CharState.InAction && !CheckNonFuncStates();
    }

    public void ReturnToGameState() {
        IsCancelBtnActive = false;
        IsCommandPanelOpen = false;
        actHold = false;
        Held = false;

        AnimSpeed(1);
    }
    public void ReturnToNeutral() {
        ReturnToGameState();
        CancelAction();
        ChangeMouseUpToDownLinks(true);
    }
    public virtual void CancelAction() {
        if (!(CharState == Consts.CharState.InAction || CharState == Consts.CharState.Walking))
            return;

        MouseUpToDownLinksTrue();
        ReturnToBoat();
        DropItemHeld();        
        EndAction();
        lvlMngr.UnfadeLevel();
        AnimSpeed(1);
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
        taskCounter = taskInterval = 0;
        timerBar.IsActive = false;
        IsCancelBtnActive = false;
        

        if (activeChar != null) {
            activeChar.SetStateDazed(false);
            activeChar.ReturnToNeutral();            
            activeChar = null;
        }

        if (CharState != Consts.CharState.Saved) {
            CharState = Consts.CharState.Default;

            if(!Airborne)
                AnimTrigger("Idle");

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
        CharState = Consts.CharState.Saved;

        // Slight;y different functionality if in water
        if(gameObject.layer != (int)Consts.UnityLayers.TossedObj) {
            ReturnToBoat();
        }

        AnimTrigger("Saved");

        DropItemHeld();        
        EndAction();
        AnimSpeed(1);
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
            AnimTrigger("Died");
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

    // =====================  ACTION/TASK SEQUENCE  ===================== //

    private class ActionStep {
        public Consts.HighlightGroupType selectable;
        public SpriteBase sprite;
        public bool walkTo;
        public float minSelReachDist;
        public Action<SpriteBase> OnSelContact;
    }

    List<ActionStep> steps = new List<ActionStep>();
    List<ActionStep> splitStepsList = new List<ActionStep>();
    int stepIdx = 0;

    bool actPathSplit;

    protected void ActionQueueInit(Consts.Skills skill, bool splitPath = false) {
        steps.Clear();
        splitStepsList.Clear();
        stepIdx = 0;

        actPathSplit = splitPath;

        IsCommandPanelOpen = false;
        ReturnToBoat();
        this.activeSkill = skill;
    }
    // Using "walkTo" bool for now, for so long as no other pathing is implemented. True to walk, false to perform action instantly
    protected void ActionQueueAdd(Consts.HighlightGroupType selectable, bool walkTo, float minSelReachDist, Action<SpriteBase> OnSelectionContact) {
        // Record parameters - selectable types need to be selected, replaced with sprites.

        ActionStep step = new ActionStep {
            selectable = selectable,
            walkTo = walkTo,
            minSelReachDist = minSelReachDist,
            OnSelContact = OnSelectionContact
        };

        if (actPathSplit)
            splitStepsList.Add(step);
        else
            steps.Add(step);
    }

    // activity counter & interval calculation, task step function, task complete function
    protected void ActionQueueRun(float taskInterval, Action TaskStep, Action TaskComplete) {
        // Task data is set, but won't be used until all selections are made
        taskCounter = this.taskInterval = taskInterval;
        this.TaskStep = (TaskStep == null) ? Action_Step : TaskStep;
        this.TaskComplete = TaskComplete;

        if (actPathSplit)
            CommenceGather();
        else
            // Start recursive function to get all selectable items
            lvlMngr.HighlightToSelect(steps[stepIdx].selectable, RecordSelectedSprite);
    }
    protected void ActionQueueRun(Action<SpriteBase> SelectionCB) {
        if (!actPathSplit)
            return;

        foreach(ActionStep step in splitStepsList)
            lvlMngr.HighlightToSelect(step.selectable, SelectionCB);
    }

    protected void ActionQueueAccSplitIdx(int index, SpriteBase sprite) {
        if (!actPathSplit)
            return;

        steps.Add(splitStepsList[index]);
        steps[stepIdx].sprite = sprite;
        stepIdx++;

        splitStepsList.Clear();
    }
    protected void ActionQueueModTaskCounter(float taskInterval) {
        taskCounter = this.taskInterval = taskInterval;
    }
    protected void ActPathSplitOff() {
        actPathSplit = false;
    }

    private void CommenceGather() {
        stepIdx = 0;
        lvlMngr.ResetEnvir();
        GatherSelected();
    }
    private void RecordSelectedSprite(SpriteBase sprite) {
        steps[stepIdx].sprite = sprite;

        if (stepIdx == steps.Count - 1) {
            CommenceGather();
        }
        else {
            stepIdx++;
            lvlMngr.HighlightToSelect(steps[stepIdx].selectable, RecordSelectedSprite);
        }            
    }
    private void GatherSelected() {
        if(steps[0].walkTo) {
            // If walking to selected sprite, check to see we're not already touching it. If not, start walking to it.
            if (!CheckSpriteContact()) {
                // Walk animation deisgned to go right, so flip left if target sprite is to the left
                if (steps[0].sprite.transform.position.x < transform.position.x) {
                    Utility.Scale(transform, -1, null, null);
                    moveDir = -1;
                }
                else {
                    Utility.Scale(transform, 1, null, null);
                    moveDir = 1;
                }
                AnimSpeed(1);
                AnimTrigger("Walk");
                CharState = Consts.CharState.Walking;
                return;
            }
        }
        OnSpriteContact();
    }
    private bool CheckSpriteContact() {
        float absDist = Math.Abs(transform.position.x - steps[0].sprite.transform.position.x);
        return absDist <= steps[0].minSelReachDist;
    }
    private void OnSpriteContact() {
        AnimSpeed(0);
        // Just less confusing after having flipped to walk left.
        Utility.Scale(transform, 1, 1, 1);
        CharState = Consts.CharState.Default;
        steps[0].OnSelContact(steps[0].sprite);
        steps.RemoveAt(0);
        if (steps.Count > 0) {
            GatherSelected();
        }
        else {
            // Perform task
            timerBar.IsActive = true;
            CharState = Consts.CharState.InAction;
            AnimSpeed(1);
        }
    }
}
