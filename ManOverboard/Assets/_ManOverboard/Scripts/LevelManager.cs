using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ZeroProgress.Common;
using ZeroProgress.Common.Collections;

public class LevelManager : MonoBehaviour {

    [SerializeField]
    private GameCtrl gameCtrl;

    Consts.LevelState levelState;

    [SerializeField]
    private GameObject waterObj;
    [SerializeField]
    private GameObject rearMenuFieldPrefab;
    private GameObject rearMenuFieldObj;

    private List<Consts.HighlightGroupType> currGroupsLit;

    private bool levelActive;
    private bool levelPaused;
    private Coroutine shipSinkCoroutine;

    public Boat boat;

    private SpriteTossableSet spriteTossableSet;

    private List<CharBase> characterSet;
    private List<CharChild> children;
    private int charSetStartCount;
    private int numChildren = 0;
    private int numElders = 0;

    private ItemBaseSet items;
    private List<ItemCanScoop> itemsCanScoop;
    private List<LifeJacket> lifeJacketsAdult;
    private List<LifeJacket> lifeJacketsChild;

    public delegate void SelectionCallback(SpriteBase spriteSelected);
    SelectionCallback SelectionCB;

    private List<ItemBase> itemsToRemove;

    private GameObject heldObj;
    private SpriteTossable heldSpriteTossable;
    private CharBase heldChar;

    private GameObject itemObj;

    private ScriptableInt holdWeight;

    public GameObject charContAreaPrefab;
    private CharContArea charContAreaScpt;

    // Mouse tracking
    private ScriptableVector2 mousePos;
    private Vector2 mouseLastPos = Vector2.zero;
    private Vector2 mouseDelta = Vector2.zero;

    // UI update event
    [SerializeField]
    private GameEvent uiUpdate;

    // Level over event
    [SerializeField]
    private StringParamEvent levelWinLossDisp;

    private Vector3 grabPos;    

    private void Awake() {
        // ! gameCtrl.Init is just here while building levels, so we don't need to go through PreGame during testing.
        // Once game is ready to ship, delete it.
        items = Resources.Load<ItemBaseSet>("ScriptableObjects/SpriteSets/ItemBaseSet");
        spriteTossableSet = Resources.Load<SpriteTossableSet>("ScriptableObjects/SpriteSets/SpriteTossableSet");

        holdWeight = Resources.Load<ScriptableInt>("ScriptableObjects/weightHeldObj");
        mousePos = Resources.Load<ScriptableVector2>("ScriptableObjects/v2_mouseWorldPos");

        gameCtrl.Init();
    }

    private void Start () {
        shipSinkCoroutine = null;

        boat.AddNumLeaksCallback(NumLeaks);

        characterSet = new List<CharBase>();
        children = new List<CharChild>();
        for (int i = 0; i < spriteTossableSet.Count; i++) {
            spriteTossableSet[i].LvlMngr = this;
            if (spriteTossableSet[i] is CharBase) {
                characterSet.Add(spriteTossableSet[i] as CharBase);
                if (spriteTossableSet[i] is CharAdult)
                    numElders++;
                else if (spriteTossableSet[i] is CharChild) {
                    numChildren++;
                    children.Add(spriteTossableSet[i] as CharChild);
                }
            }
        }

        charSetStartCount = characterSet.Count;

        levelState = Consts.LevelState.Default;
        holdWeight.CurrentValue = 0;
        uiUpdate.RaiseEvent();

        GameObject charContArea = Instantiate(charContAreaPrefab) as GameObject;
        charContAreaScpt = charContArea.GetComponent<CharContArea>();
        charContAreaScpt.SetMouseCBs(MouseEnterCharContArea, MouseExitCharContArea);
        charContAreaScpt.gameObject.SetActive(false);

        rearMenuFieldObj = Instantiate(rearMenuFieldPrefab) as GameObject;
        RearMenuField menuFieldScpt = rearMenuFieldObj.GetComponent<RearMenuField>();
        menuFieldScpt.Mngr = this;

        rearMenuFieldObj.SetActive(false);

        currGroupsLit = new List<Consts.HighlightGroupType>();
        itemsToRemove = new List<ItemBase>();
        // Establish every possible action in the level, and do any appropriate setup for it (like adding highlight components to selectable objects).
        itemsCanScoop = new List<ItemCanScoop>();
        lifeJacketsAdult = new List<LifeJacket>();
        lifeJacketsChild = new List<LifeJacket>();
        foreach (ItemBase item in items) {
            item.AddHighlightComponent();
            ReturnItem(item);
        }

        foreach (CharBase character in characterSet)
            character.SetActionBtns();

        // Check if children will be highlightable/selectable
        if (lifeJacketsChild.Count > 0 && numChildren > 0 && numElders > 0) {
            foreach (CharChild child in children) {
                child.AddHighlightComponent();
            }
        }

        levelActive = true;
        levelPaused = false;

        if (shipSinkCoroutine != null)
            StopCoroutine(shipSinkCoroutine);
        shipSinkCoroutine = StartCoroutine(SinkShipInterval());
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

    private void Update() {
        if (levelState == Consts.LevelState.CharHeldToToss) {
            heldSpriteTossable.MoveRigidbody();
            mouseDelta = mousePos.CurrentValue - mouseLastPos;
            mouseLastPos = mousePos.CurrentValue;
        }        
    }

    private void OnDestroy() {
        DrawLayerMngr.ClearSpriteRef();
        spriteTossableSet.Clear();
        items.Clear();
    }

    private void MouseEnterCharContArea() {
        levelState = Consts.LevelState.Default;
        heldObj.transform.position = grabPos;
        heldSpriteTossable.OverheadButtonActive(true);
    }

    private void MouseExitCharContArea() {
        levelState = Consts.LevelState.CharHeldToToss;
        heldSpriteTossable.OverheadButtonActive(false);
    }

    private void CheckLevelEndResult() {
        gameCtrl.GetCurrLevel();
    }

    public void OnSpriteMouseDown(GameObject spriteObj) {
        if (!levelActive)
            return;

        heldObj = spriteObj;
        heldSpriteTossable = spriteObj.GetComponent<SpriteTossable>();
        if(heldSpriteTossable is CharBase)
            heldChar = spriteObj.GetComponent<CharBase>();
        else
            heldChar = null;

        // Bring character to focus, in front of everything.
        heldSpriteTossable.SortCompLayerChange(Consts.DrawLayers.FrontOfLevel3, null);

        // maintain copy of character's position where grabbed
        grabPos = spriteObj.transform.position;

        charContAreaScpt.gameObject.SetActive(true);
        heldSpriteTossable.ApplyTransformToContArea(charContAreaScpt.gameObject, true);
        charContAreaScpt.MouseEnterCB();

        holdWeight.CurrentValue = heldSpriteTossable.Weight;
        uiUpdate.RaiseEvent();

        PauseLevel();
    }

    public void OnSpriteMouseUp() {
        if (!levelActive)
            return;

        charContAreaScpt.gameObject.SetActive(false);

        if (levelState == Consts.LevelState.CharHeldToToss) {

            if (heldSpriteTossable is ItemBase)
                RemoveItem(heldSpriteTossable as ItemBase);
            else
                RemoveCharacter(heldSpriteTossable as CharBase);     

            // TODO: Top out the toss speed to something not TOO unreasonable
            float tossSpeed = mouseDelta.magnitude / Time.deltaTime;

            // TODO: Consider that when a character is holding 1 or more items, they should maybe be unparented and given varying trajectories upon being tossed.
            // Lifejacket donned would be exeption to this, being worn, not held.
            // Takes care of set removal
            heldSpriteTossable.Toss(mouseDelta * tossSpeed);

            heldObj.layer = 9; // tossed objects

            // Change weight in boat
            boat.RemoveLoad(holdWeight.CurrentValue);

            heldChar = null;
            heldSpriteTossable = null;
        }
        else
            heldSpriteTossable.ReturnToBoat();

        levelState = Consts.LevelState.Default;

        holdWeight.CurrentValue = 0;
        uiUpdate.RaiseEvent();

        UnPauseLevel();
    }

    

    // TODO: need to find a better way to dynamically select item types. Passing raw ints in the inspector is insufficient
    public void ToState(Consts.LevelState state) {
        levelState = state;
    }

    private void NumLeaks(int numLeaks) {
        // TODO: Have the number of leaks on the gui somewhere?
        if(numLeaks == 0) {
            // level over
            int charLoss = charSetStartCount - characterSet.Count;

            if (charLoss <= gameCtrl.GetLevelMaxCharLoss(3)) {
                levelWinLossDisp.RaiseEvent("You a winner! 3 star play!");
            }
            else if (charLoss <= gameCtrl.GetLevelMaxCharLoss(2)) {
                levelWinLossDisp.RaiseEvent("You a winner! 2 star play!");
            }
            else if (charLoss <= gameCtrl.GetLevelMaxCharLoss(1)) {
                levelWinLossDisp.RaiseEvent("You a winner! 1 star play!");
            }
            else {
                levelWinLossDisp.RaiseEvent("Too many people died!");
            }

            levelActive = false;
            levelState = Consts.LevelState.Default;
            foreach (CharBase character in characterSet) {
                character.Saved = true;
            }
        }
    }

    public void HighlightToSelect(Consts.HighlightGroupType groupType, SelectionCallback CB) {
        levelState = Consts.LevelState.ObjectSelection;
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
    }
    // BOOKMARK
    public void UnHighlight() {
        levelState = Consts.LevelState.Default;

        foreach (ItemCanScoop itemCS in itemsCanScoop)
            itemCS.UnHighlight();
        foreach (LifeJacket itemLJ in lifeJacketsChild)
            itemLJ.UnHighlight();
        foreach (LifeJacket itemLJ in lifeJacketsAdult)
            itemLJ.UnHighlight();
        foreach (CharChild child in children)
            child.UnHighlight();

        currGroupsLit.Clear();
    }
    public void UnHighlight(Consts.HighlightGroupType groupType) {
        levelState = Consts.LevelState.Default;

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

        currGroupsLit.Remove(groupType);
    }

    // BOOKMARK
    public void ConfirmSelections() {
        // Remove items from lists
        foreach (ItemBase item in itemsToRemove) {
            item.RemoveFromChar();
            RemoveItem(item);
        }

        ResetEnvir();
    }
    public void OnSelection(SpriteBase sprite) {
        for (int i = currGroupsLit.Count - 1; i >= 0; i--)
            UnHighlight(currGroupsLit[i]);

        // Remove item from general level listing, as it is now "occupied", or in the "possession" of the character.
        // This will have to be modified if it becomes that 2 or more characters can act on one item at a time.
        if (sprite is ItemBase)
            itemsToRemove.Add(sprite as ItemBase);

        SelectionCB(sprite);
    }
    public void OnDeselection(SpriteBase sprite) {
        if (sprite is ItemBase)
            ReturnItem(sprite as ItemBase);
    }
    private void RemoveCharacter(CharBase character) {
        characterSet.Remove(character);
        if (character is CharChild) {
            children.Remove(character as CharChild);
            numChildren--;
        }
        else {
            numElders--;
        }
    }
    private void RemoveItem(ItemBase item) {
        if (item is LifeJacket) {
            lifeJacketsChild.Remove(item as LifeJacket);
            lifeJacketsAdult.Remove(item as LifeJacket);
        }
        else if(item is ItemCanScoop) {
            itemsCanScoop.Remove(item as ItemCanScoop);
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
        if (item is ItemCanScoop)
            itemsCanScoop.Add(item as ItemCanScoop);
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

    // BOOKMARK
    private void PauseLevel() {
        levelPaused = true;

        foreach (CharBase character in characterSet) {
            if (character != heldChar)
                character.Paused = true;
        }
    }
    private void UnPauseLevel() {
        if (!levelActive)
            return;

        levelPaused = false;

        foreach (CharBase character in characterSet) {
            if (character != heldChar)
                character.Paused = false;
        }
    }
    public void FadeLevel() {
        charContAreaScpt.gameObject.SetActive(false);
        rearMenuFieldObj.SetActive(true);
        PauseLevel();
    }
    public void UnfadeLevel() {
        charContAreaScpt.gameObject.SetActive(false);
        rearMenuFieldObj.SetActive(false);
        UnPauseLevel();
        holdWeight.CurrentValue = 0;
        uiUpdate.RaiseEvent();
        heldObj = null;
    }
    public void ResetEnvir() {
        UnfadeLevel();
        for(int i = currGroupsLit.Count - 1; i >= 0; i--)
            UnHighlight(currGroupsLit[i]);
        itemsToRemove.Clear();
        levelState = Consts.LevelState.Default;
    }
    // Called by RearMenuField when field is clicked, so as to cancel current selection process.
    public void ResetAll() {
        if (heldChar != null)
            heldChar.ReturnToNeutral();

        if(heldSpriteTossable != null)
            heldSpriteTossable.ReturnToBoat();

        ResetEnvir();
    }
}