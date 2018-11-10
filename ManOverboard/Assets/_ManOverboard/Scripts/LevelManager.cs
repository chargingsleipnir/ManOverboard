using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;
using ZeroProgress.Common.Collections;
using UnityEditor;

public class LevelManager : MonoBehaviour {

    [SerializeField]
    private GameCtrl gameCtrl;

    Consts.LevelState levelState;

    // TODO: Adjust sink rate based on number of holes
    [SerializeField]
    private GameObject waterObj;
    [SerializeField]
    private GameObject rearMenuFieldPrefab;
    private GameObject rearMenuFieldObj;

    private bool levelActive;
    private bool levelPaused;
    private Coroutine shipSinkCoroutine;

    public Boat boat;

    private SpriteTossableSet spriteTossableSet;

    private List<CharBase> characterSet;
    private int charSetStartCount;
    private int numChildren = 0;
    private int numElders = 0;

    private ItemBaseSet items;
    private List<ItemCanScoop> itemsCanScoop;
    private List<LifeJacket> lifeJacketsAdult;
    private List<LifeJacket> lifeJacketsChild;

    private GameObject heldObj;
    private SpriteTossable heldSpriteTossable;
    private CharBase heldChar;

    private GameObject itemObj;

    public IntReference holdWeight;

    public GameObject charContAreaPrefab;
    private CharContArea charContAreaScpt;

    // Mouse tracking
    [SerializeField]
    private Vector2Reference mousePos;
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
        gameCtrl.Init();

        spriteTossableSet = AssetDatabase.LoadAssetAtPath<SpriteTossableSet>("Assets/_ManOverboard/Variables/Sets/SpriteTossableSet.asset");
        items = AssetDatabase.LoadAssetAtPath<ItemBaseSet>("Assets/_ManOverboard/Variables/Sets/ItemBaseSet.asset");
    }

    private void Start () {
        shipSinkCoroutine = null;

        boat.AddNumLeaksCallback(NumLeaks);

        characterSet = new List<CharBase>();
        for (int i = 0; i < spriteTossableSet.Count; i++) {
            spriteTossableSet[i].SetMouseRespCallbacks(OnTossableSpriteMouseDown, OnTossableSpriteMouseUp);
            if (spriteTossableSet[i] is CharBase) {
                characterSet.Add(spriteTossableSet[i] as CharBase);
                if (spriteTossableSet[i] is CharElder)
                    numElders++;
                else if (spriteTossableSet[i] is CharChild)
                    numChildren++;
            }
            else if (spriteTossableSet[i] is ItemBase) {
                (spriteTossableSet[i] as ItemBase).SetItemSelectionCallback(OnItemSelectionCB, OnItemDeselectionCB);
            }
        }
        charSetStartCount = characterSet.Count;

        levelState = Consts.LevelState.Default;
        holdWeight.Value = 0;
        uiUpdate.RaiseEvent();

        GameObject charContArea = Instantiate(charContAreaPrefab) as GameObject;
        charContAreaScpt = charContArea.GetComponent<CharContArea>();
        charContAreaScpt.SetMouseCBs(MouseEnterCharContArea, MouseExitCharContArea);
        charContAreaScpt.gameObject.SetActive(false);

        rearMenuFieldObj = Instantiate(rearMenuFieldPrefab) as GameObject;
        RearMenuField menuFieldScpt = rearMenuFieldObj.GetComponent<RearMenuField>();
        menuFieldScpt.Mngr = this;

        rearMenuFieldObj.SetActive(false);

        itemsCanScoop = new List<ItemCanScoop>();
        lifeJacketsAdult = new List<LifeJacket>();
        lifeJacketsChild = new List<LifeJacket>();
        foreach (ItemBase item in items) {
            if (item is ItemCanScoop)
                itemsCanScoop.Add(item as ItemCanScoop);
            else if(item is LifeJacket) {
                LifeJacket jacket = item as LifeJacket;
                if (jacket.size == Consts.FitSizes.adult)
                    lifeJacketsAdult.Add(jacket);
                else
                    lifeJacketsChild.Add(jacket);
            }
        }

        foreach(CharBase character in characterSet) {
            character.CheckCanAct(
                lifeJacketsChild.Count > 0 && numChildren > 0,
                lifeJacketsAdult.Count > 0 && numElders > 0,
                itemsCanScoop.Count > 0 && numElders > 0
            );
        }

        levelActive = true;
        levelPaused = false;

        if (shipSinkCoroutine != null)
            StopCoroutine(shipSinkCoroutine);
        shipSinkCoroutine = StartCoroutine(SinkShipInterval());
    }

    private void Update() {
        if (levelState == Consts.LevelState.CharHeldToToss) {
            heldSpriteTossable.MoveRigidbody();
            mouseDelta = mousePos.Value - mouseLastPos;
            mouseLastPos = mousePos.Value;
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

    public void OnTossableSpriteMouseDown(GameObject spriteObj) {
        if (!levelActive)
            return;

        // If a different character's menu is currently open, close it.
        if (spriteObj != heldObj)
            ReturnToNeutral();

        heldObj = spriteObj;
        heldSpriteTossable = spriteObj.GetComponent<SpriteTossable>();
        if(heldSpriteTossable is CharBase) {
            heldChar = spriteObj.GetComponent<CharBase>();
            heldChar.SetCallbacks(HighlightToSelectCB, RemoveWaterCB, FadeLevelCB, UnfadeLevelCB);
        }
        else
            heldChar = null;

        // Bring character to focus, in front of everything.
        heldSpriteTossable.SortCompLayerChange(Consts.DrawLayers.FrontOfLevel3, null);

        // maintain copy of character's position where grabbed
        grabPos = spriteObj.transform.position;

        charContAreaScpt.gameObject.SetActive(true);
        heldSpriteTossable.ApplyTransformToContArea(charContAreaScpt.gameObject, true);
        charContAreaScpt.MouseEnterCB();

        holdWeight.Value = heldSpriteTossable.Weight;
        uiUpdate.RaiseEvent();

        PauseLevel();
    }

    public void OnTossableSpriteMouseUp() {
        if (!levelActive)
            return;

        charContAreaScpt.gameObject.SetActive(false);

        if (levelState == Consts.LevelState.CharHeldToToss) {

            characterSet.Remove(heldSpriteTossable as CharBase);        

            // TODO: Top out the toss speed to something not TOO unreasonable
            float tossSpeed = mouseDelta.magnitude / Time.deltaTime;

            // TODO: Consider that when a character is holding 1 or more items, they should maybe be unparented and given varying trajectories upon being tossed.
            // Takes care of set removal
            heldSpriteTossable.Toss(mouseDelta * tossSpeed);

            heldObj.layer = 9; // tossed objects

            // Change weight in boat
            boat.RemoveLoad(holdWeight.Value);

            heldChar = null;
            heldSpriteTossable = null;
        }
        else
            heldSpriteTossable.ReturnToBoat();

        levelState = Consts.LevelState.Default;

        holdWeight.Value = 0;
        uiUpdate.RaiseEvent();

        UnPauseLevel();
    }

    public void OnItemSelectionCB(ItemBase item) {
        ReturnToNeutral();
        if (heldChar != null)
            heldChar.UseItem(item);
    }
    public void OnItemDeselectionCB(ItemBase item) {
        // TODO: Fill out as needed
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

    public void FadeLevelCB() {
        charContAreaScpt.gameObject.SetActive(false);
        rearMenuFieldObj.SetActive(true);
        PauseLevel();        
    }
    public void UnfadeLevelCB() {
        charContAreaScpt.gameObject.SetActive(false);
        rearMenuFieldObj.SetActive(false);
        UnPauseLevel();
    }
    public void HighlightToSelectCB(Consts.ItemType itemType) {
        levelState = Consts.LevelState.ObjectSelection;

        if (itemType == Consts.ItemType.Scooping) {
            foreach (ItemCanScoop itemCS in itemsCanScoop)
                itemCS.HighlightToClick();
        }
    }
    public void RemoveWaterCB(int waterWeight) {
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

    public void ReturnToNeutral() {
        if (heldChar != null) {
            heldChar.IsActionBtnActive = false;
            heldChar.IsCancelBtnActive = false;
            heldChar.IsCommandPanelOpen = false;
            heldChar.ChangeMouseUpToDownLinks(true);
        }

        if(heldSpriteTossable != null)
            heldSpriteTossable.ReturnToBoat();

        rearMenuFieldObj.SetActive(false);
        holdWeight.Value = 0;
        uiUpdate.RaiseEvent();

        foreach (ItemBase item in items)
            item.UnHighlight();

        levelState = Consts.LevelState.Default;
        UnPauseLevel();
    }
}