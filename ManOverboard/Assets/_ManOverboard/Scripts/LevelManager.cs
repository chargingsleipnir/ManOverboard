using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ZeroProgress.Common;
using ZeroProgress.Common.Collections;
using Game2DWaterKit;

public class LevelManager : MonoBehaviour {

    [SerializeField]
    private GameCtrl gameCtrl;

    Consts.LevelState levelState;
    private bool prepTossCalc;

    [SerializeField]
    private GameObject waterObj;
    public float WaterSurfaceYPos { get; private set; }

    [SerializeField]
    private GameObject rearMenuFieldPrefab;
    private GameObject rearMenuFieldObj;

    private List<Consts.HighlightGroupType> currGroupsLit;

    private bool levelActive;
    private bool levelPaused;
    private Coroutine shipSinkCoroutine;

    public Boat boat;

    private SpriteTossableSet spriteTossableSet;

    private List<CharBase> charsInPlay;
    private List<CharChild> children;
    private int charSetStartCount;
    private int charsSaved;
    private int charsKilled;
    private int numChildren = 0;
    private int numElders = 0;

    private ItemBaseSet items;
    private List<ItemCanScoop> itemsCanScoop;
    private List<LifeJacket> lifeJacketsAdult;
    private List<LifeJacket> lifeJacketsChild;
    private List<RepairKit> repairKits;

    private EnemySet enemies;

    public delegate void SelectionCallback(SpriteBase spriteSelected);
    SelectionCallback SelectionCB;

    // Presuming only one item can be selected at a time for now.
    private ItemBase selectedItem;

    private SpriteTossable heldSpriteTossable;
    private CharBase heldChar;

    private GameObject itemObj;

    private ScriptableInt holdWeight;

    public GameObject spriteMouseRespPrefab;
    private SpriteMouseRespHdlr spriteMouseRespScpt;

    // Mouse tracking
    private ScriptableVector2 mousePos;
    private Vector2 mouseLastPos = Vector2.zero;
    private Vector2 mouseDelta = Vector2.zero;

    // UI update event
    [SerializeField]
    private GameEvent uiUpdate;

    // Level over event
    private StringParamEvent levelMsg;

    private Vector3 grabPosLocal;    

    private void Awake() {
        // ! gameCtrl.Init is just here while building levels, so we don't need to go through PreGame during testing.
        // Once game is ready to ship, delete it.
        items = Resources.Load<ItemBaseSet>("ScriptableObjects/SpriteSets/ItemBaseSet");
        enemies = Resources.Load<EnemySet>("ScriptableObjects/SpriteSets/EnemySet");
        spriteTossableSet = Resources.Load<SpriteTossableSet>("ScriptableObjects/SpriteSets/SpriteTossableSet");

        holdWeight = Resources.Load<ScriptableInt>("ScriptableObjects/weightHeldObj");
        mousePos = Resources.Load<ScriptableVector2>("ScriptableObjects/v2_mouseWorldPos");

        levelMsg = Resources.Load<StringParamEvent>("ScriptableObjects/Events/WithParam/Str_AnyMsg");

        WaterSurfaceYPos = (waterObj.transform.position.y + (waterObj.GetComponent<Game2DWater>().WaterSize.y * 0.5f));

        gameCtrl.Init();
    }

    private void Start () {
        shipSinkCoroutine = null;

        boat.LvlMngr = this;
        boat.Start();

        charSetStartCount = 0;
        charsSaved = 0;
        charsKilled = 0;

        charsInPlay = new List<CharBase>();
        children = new List<CharChild>();
        for (int i = 0; i < spriteTossableSet.Count; i++) {
            spriteTossableSet[i].LvlMngr = this;
            if (spriteTossableSet[i] is CharBase) {
                charsInPlay.Add(spriteTossableSet[i] as CharBase);
                if (spriteTossableSet[i] is CharAdult)
                    numElders++;
                else if (spriteTossableSet[i] is CharChild) {
                    numChildren++;
                    children.Add(spriteTossableSet[i] as CharChild);
                }
            }
        }

        for (int i = 0; i < enemies.Count; i++)
            enemies[i].LvlMngr = this;

        charSetStartCount = charsInPlay.Count;

        holdWeight.CurrentValue = 0;
        uiUpdate.RaiseEvent();

        spriteMouseRespScpt = (Instantiate(spriteMouseRespPrefab) as GameObject).GetComponent<SpriteMouseRespHdlr>();
        spriteMouseRespScpt.LvlMngr = this;
        spriteMouseRespScpt.Init();

        rearMenuFieldObj = Instantiate(rearMenuFieldPrefab) as GameObject;
        RearMenuField menuFieldScpt = rearMenuFieldObj.GetComponent<RearMenuField>();
        menuFieldScpt.LvlMngr = this;

        rearMenuFieldObj.SetActive(false);

        currGroupsLit = new List<Consts.HighlightGroupType>();
        selectedItem = null;
        // Establish every possible action in the level, and do any appropriate setup for it (like adding highlight components to selectable objects).
        itemsCanScoop = new List<ItemCanScoop>();
        lifeJacketsAdult = new List<LifeJacket>();
        lifeJacketsChild = new List<LifeJacket>();
        repairKits = new List<RepairKit>();
        foreach (ItemBase item in items) {
            item.AddHighlightComponent();
            ReturnItem(item);
        }

        foreach (CharBase character in charsInPlay)
            character.SetActionBtns();

        // Check if children will be highlightable/selectable
        if (lifeJacketsChild.Count > 0 && numChildren > 0 && numElders > 0) {
            foreach (CharChild child in children) {
                child.AddHighlightComponent();
            }
        }

        levelActive = true;
        levelPaused = false;
        prepTossCalc = false;

        if (shipSinkCoroutine != null)
            StopCoroutine(shipSinkCoroutine);
        shipSinkCoroutine = StartCoroutine(SinkShipInterval());
    }

    private void Update() {
        if (levelActive == false || prepTossCalc == false)
            return;

        heldSpriteTossable.MoveRigidbody();
        mouseDelta = mousePos.CurrentValue - mouseLastPos;
        mouseLastPos = mousePos.CurrentValue;
    }

    private void OnDestroy() {
        DrawLayerMngr.ClearSpriteRef();
        spriteTossableSet.Clear();
        items.Clear();
    }

    public bool CheckCanDonLifeJacketChildren() {
        int numChildrenNoJacket = 0;
        foreach (CharChild child in children)
            if (child.IsWearingLifeJacket == false && child.CheckAvailToAct())
                numChildrenNoJacket++;

        if (numChildrenNoJacket == 0)
            return false;

        return lifeJacketsChild.Count > 0 && numChildrenNoJacket > 0;
    }
    public bool CheckCanDonLifeJacketAdults() {
        return lifeJacketsAdult.Count > 0;
    }
    public bool CheckCanScoop() {
        return itemsCanScoop.Count > 0;
    }
    public bool CheckCanRepair() {
        int usefullRepairKits = 0;
        int holesNotInRepair = 0;

        foreach (RepairKit kit in repairKits)
            if(!kit.InUse && kit.uses > 0)
                usefullRepairKits++;

        foreach (Hole hole in boat.Pinholes)
            if (!hole.InRepair)
                holesNotInRepair++;

        return usefullRepairKits > 0 && holesNotInRepair > 0;
    }

    public void OnSpriteMouseDown(GameObject spriteObj) {
        if (!levelActive)
            return;

        heldSpriteTossable = spriteObj.GetComponent<SpriteTossable>();
        grabPosLocal = heldSpriteTossable.transform.localPosition; // maintain copy of character's position where grabbed, relative to the boat

        if (heldSpriteTossable is CharBase) {
            heldChar = spriteObj.GetComponent<CharBase>();
        }
        else
            heldChar = null;

        // Bring character to focus, in front of everything.
        heldSpriteTossable.SortCompLayerChange(Consts.DrawLayers.FrontOfLevel3, null);

        holdWeight.CurrentValue = heldSpriteTossable.Weight;
        uiUpdate.RaiseEvent();

        spriteMouseRespScpt.SetActive(true);
    }
    public void HeldSpriteClick() {
        heldSpriteTossable.OnClick();
        FadeLevel();
    }

    public void HeldSpriteRelease() {
        prepTossCalc = false;
        UnfadeLevel();
        ResetHeld();
    }
    public void HeldSpriteInsideTossArea() {
        prepTossCalc = true;

        if (heldChar != null)
            heldChar.IsHeld();
    }
    public void HeldSpriteToss() {
        spriteMouseRespScpt.SetActive(false);

        // TODO: Top out the toss speed to something not TOO unreasonable
        //Vector2 tossDir = mouseDelta.normalized;
        float tossSpeed = mouseDelta.magnitude / Time.deltaTime;


        if (heldSpriteTossable is ItemBase) {
            RemoveItem(heldSpriteTossable as ItemBase);
            heldSpriteTossable.Toss(mouseDelta * tossSpeed);
        }
        else {
            CharBase c = heldSpriteTossable as CharBase;

            // TODO: Character animation still set to Struggling? That's fine for now, but perhaps make something else for when airborne.

            if (c.ItemHeld != null)
                c.ItemHeld.Toss(Utility.AddNoiseDeg(mouseDelta, Consts.TOSS_NOISE_MIN, Consts.TOSS_NOISE_MAX) * tossSpeed);

            c.Toss(mouseDelta * tossSpeed);
        }         

        // Change weight in boat
        boat.RemoveLoad(holdWeight.CurrentValue);

        prepTossCalc = false;
        heldChar = null;
        heldSpriteTossable = null;        

        holdWeight.CurrentValue = 0;
        uiUpdate.RaiseEvent();

        UnPauseLevel();
    }

    public void CharSaved(CharBase character) {
        charsSaved++;
        RemoveCharacter(character);
        CheckLevelEnd();
    }
    public void CharKilled(CharBase character) {
        charsKilled++;
        RemoveCharacter(character);
        CheckLevelEnd();
    }
    public void RemoveCharacter(CharBase character) {
        charsInPlay.Remove(character);
        if (character is CharChild) {
            children.Remove(character as CharChild);
            numChildren--;
        }
        else
            numElders--;        
    }

    public void EnemyKilled(Enemy e) {
        enemies.Remove(e);
        if (enemies.Count <= 0)
            CheckLevelEnd();
    }

    public void CheckLevelEnd() {

        // TODO: Incorporate enemy count in meaningful way.
        if (enemies.Count > 0)
            return;

        bool charsDone = charsSaved + charsKilled == charSetStartCount;
        bool holesDone = boat.HolesSubmCount == 0;

        if (charsDone == false && holesDone == false)
            return;

        if(holesDone) {
            bool charInAir = false;
            for(int i = charsInPlay.Count - 1; i > -1; i--) {
                // Still not sure if they'll live or die - wait to determine
                if (charsInPlay[i].Airborne)
                    charInAir = true;
                else {
                    charsInPlay[i].SetStateSaved();
                    charsSaved++;
                    RemoveCharacter(charsInPlay[i]);
                }
            }

            if (charInAir)
                return;                
        }

        LevelEnd(charsKilled);
    }

    private void LevelEnd(int charLoss) {
        if (charLoss <= gameCtrl.GetLevelMaxCharLoss(3)) {
            levelMsg.RaiseEvent("You a winner! 3 star play!");
        }
        else if (charLoss <= gameCtrl.GetLevelMaxCharLoss(2)) {
            levelMsg.RaiseEvent("You a winner! 2 star play!");
        }
        else if (charLoss <= gameCtrl.GetLevelMaxCharLoss(1)) {
            levelMsg.RaiseEvent("You a winner! 1 star play!");
        }
        else {
            levelMsg.RaiseEvent("Too many people died!");
        }

        levelActive = false;
    }

    public void HighlightToSelect(Consts.HighlightGroupType groupType, SelectionCallback CB) {
        currGroupsLit.Add(groupType);

        SelectionCB = CB;

        if (groupType == Consts.HighlightGroupType.Scooping) {
            foreach (ItemCanScoop itemCS in itemsCanScoop)
                itemCS.HighlightToSelect();
        }
        else if (groupType == Consts.HighlightGroupType.LifeJacketChild) {
            foreach (LifeJacket itemLJ in lifeJacketsChild)
                itemLJ.HighlightToSelect();
        }
        if (groupType == Consts.HighlightGroupType.LifeJacketAdult) {
            foreach (LifeJacket itemLJ in lifeJacketsAdult)
                itemLJ.HighlightToSelect();
        }
        else if (groupType == Consts.HighlightGroupType.Children) {
            foreach (CharChild child in children)
                child.HighlightToSelect();
        }
        else if (groupType == Consts.HighlightGroupType.RepairKits) {
            foreach (RepairKit kit in repairKits)
                kit.HighlightToSelect();
        }
        else if (groupType == Consts.HighlightGroupType.PinHoles) {
            foreach (Hole hole in boat.Pinholes)
                hole.HighlightToSelect();
        }
    }
    // BOOKMARK
    public void UnHighlight() {
        foreach (ItemCanScoop itemCS in itemsCanScoop)
            itemCS.UnHighlight();
        foreach (LifeJacket itemLJ in lifeJacketsChild)
            itemLJ.UnHighlight();
        foreach (LifeJacket itemLJ in lifeJacketsAdult)
            itemLJ.UnHighlight();
        foreach (CharChild child in children)
            child.UnHighlight();
        foreach (RepairKit kit in repairKits)
            kit.UnHighlight();
        foreach (Hole hole in boat.Pinholes)
            hole.UnHighlight();

        currGroupsLit.Clear();
    }
    public void UnHighlight(Consts.HighlightGroupType groupType) {
        if (groupType == Consts.HighlightGroupType.Scooping) {
            foreach (ItemCanScoop itemCS in itemsCanScoop)
                itemCS.UnHighlight();
        }
        else if (groupType == Consts.HighlightGroupType.LifeJacketChild) {
            foreach (LifeJacket itemLJ in lifeJacketsChild)
                itemLJ.UnHighlight();
        }
        if (groupType == Consts.HighlightGroupType.LifeJacketAdult) {
            foreach (LifeJacket itemLJ in lifeJacketsAdult)
                itemLJ.UnHighlight();
        }
        else if (groupType == Consts.HighlightGroupType.Children) {
            foreach (CharChild child in children)
                child.UnHighlight();
        }
        else if (groupType == Consts.HighlightGroupType.RepairKits) {
            foreach (RepairKit kit in repairKits)
                kit.UnHighlight();
        }
        else if (groupType == Consts.HighlightGroupType.PinHoles) {
            foreach (Hole hole in boat.Pinholes)
                hole.UnHighlight();
        }

        currGroupsLit.Remove(groupType);
    }

    public void ConfirmSelections(CharBase charToAct) {
        if (selectedItem != null) {
            selectedItem.RemoveFromChar();
            RemoveItem(selectedItem);
            charToAct.HoldItem(selectedItem);
        }

        ResetEnvir();
    }
    public void OnSelection(SpriteBase sprite) {
        for (int i = currGroupsLit.Count - 1; i >= 0; i--)
            UnHighlight(currGroupsLit[i]);

        // Remove item from general level listing, as it is now "occupied", or in the "possession" of the character.
        // This will have to be modified if it becomes that 2 or more characters can act on one item at a time.
        if (sprite is ItemBase)
            selectedItem = sprite as ItemBase;

        SelectionCB(sprite);
    }
    public void OnDeselection(SpriteBase sprite) {
        if (sprite is ItemBase)
            ReturnItem(sprite as ItemBase);
    }
    public void RemoveItem(ItemBase item) {
        if (item is LifeJacket) {
            lifeJacketsChild.Remove(item as LifeJacket);
            lifeJacketsAdult.Remove(item as LifeJacket);
        }
        else if(item is ItemCanScoop) {
            itemsCanScoop.Remove(item as ItemCanScoop);
        }
        else if (item is RepairKit) {
            repairKits.Remove(item as RepairKit);
        }
    }
    private void ReturnItem(ItemBase item) {
        if (item is LifeJacket) {
            LifeJacket jacket = item as LifeJacket;
            if (jacket.size == Consts.FitSizes.child)
                lifeJacketsChild.Add(jacket);
            else
                lifeJacketsAdult.Add(jacket);
        }
        else if (item is ItemCanScoop)
            itemsCanScoop.Add(item as ItemCanScoop);
        else if (item is RepairKit)
            repairKits.Add(item as RepairKit);
    }
    
    public void RemoveWater(int waterWeight) {
        if (!levelActive)
            return;

        boat.RemoveWater(waterWeight);
    }

    IEnumerator SinkShipInterval() {
        while (levelActive) {
            if(!levelPaused)
                boat.AddWater();
            yield return new WaitForSeconds(Consts.SINK_STEP_SECS);
        }
    }

    public void SetBoatAsParent(SpriteBase sprite) {
        sprite.SortCompResetToBase(boat.transform);
    }

    public void RepairBoat(Hole hole) {
        boat.Repair(hole);
    }

    // BOOKMARK
    private void PauseLevel() {
        levelPaused = true;

        foreach (SpriteTossable st in spriteTossableSet) {
            st.Paused = true;
        }
    }
    private void UnPauseLevel() {
        if (!levelActive)
            return;

        levelPaused = false;

        foreach (SpriteTossable st in spriteTossableSet) {
            st.Paused = false;
        }
    }
    public void FadeLevel() {
        spriteMouseRespScpt.SetActive(false);
        rearMenuFieldObj.SetActive(true);
        PauseLevel();
    }
    public void UnfadeLevel() {
        spriteMouseRespScpt.SetActive(false);
        rearMenuFieldObj.SetActive(false);
        UnPauseLevel();
        holdWeight.CurrentValue = 0;
        uiUpdate.RaiseEvent();
    }
    public void ResetEnvir() {
        UnfadeLevel();
        for(int i = currGroupsLit.Count - 1; i >= 0; i--)
            UnHighlight(currGroupsLit[i]);

        selectedItem = null;
    }
    public void ResetHeld() {
        if (heldChar != null) {
            heldChar.ReturnToGameState();
        }

        if (heldSpriteTossable != null) {
            heldSpriteTossable.ReturnToBoat();
            heldSpriteTossable.transform.localPosition = grabPosLocal;
        }
    }

    // Called by RearMenuField when field is clicked, so as to cancel current selection process.
    public void ResetAll() {
        ResetHeld();
        ResetEnvir();
    }
}