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
    private List<Coroutine> removeWaterCoroutines;

    public Boat boat;

    private SpriteTossableSet spriteTossableSet;

    private List<CharBase> characterSet;
    private int charSetStartCount;

    private ItemCanScoopSet itemsCanScoop;

    private GameObject heldObj;
    private SpriteTossable heldSpriteTossable;
    private CharActionable heldCharActionable;

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
        itemsCanScoop = AssetDatabase.LoadAssetAtPath<ItemCanScoopSet>("Assets/_ManOverboard/Variables/Sets/ItemCanScoopSet.asset");
    }

    private void Start () {
        shipSinkCoroutine = null;
        removeWaterCoroutines = new List<Coroutine>();

        boat.AddNumLeaksCallback(NumLeaks);

        characterSet = new List<CharBase>();
        for (int i = 0; i < spriteTossableSet.Count; i++) {
            spriteTossableSet[i].SetMouseRespCallbacks(OnTossableSpriteMouseDown, OnTossableSpriteMouseUp);
            if (spriteTossableSet[i] is CharBase)
                characterSet.Add(spriteTossableSet[i] as CharBase);
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
        // Pause animations of all characters except the one being held.
    }
    private void UnPauseLevel() {
        if (!levelActive)
            return;

        levelPaused = false;
    }

    public void OnTossableSpriteMouseDown(GameObject spriteObj) {
        if (!levelActive)
            return;

        // If a different character's menu is currently open, close it.
        if (spriteObj != heldObj)
            ReturnToNeutral();

        heldObj = spriteObj;
        heldSpriteTossable = spriteObj.GetComponent<SpriteTossable>();
        if(heldSpriteTossable is CharActionable) {
            heldCharActionable = spriteObj.GetComponent<CharActionable>();
            heldCharActionable.SetCallbacks(HighlightToSelectCB, StartRemoveWaterCB, StopRemoveWaterCB, FadeLevelCB, UnfadeLevelCB);
        }
        else {
            heldCharActionable = null;
        }

        // Bring character to focus, in front of everything.
        heldSpriteTossable.SortCompLayerChange(Consts.DrawLayers.FrontOfLevel3, null);

        // maintain copy of character's position where grabbed
        grabPos = spriteObj.transform.position;

        charContAreaScpt.gameObject.SetActive(true);
        heldSpriteTossable.ApplyTransformToContArea(charContAreaScpt.gameObject);
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

            // TODO: Top out the toss speed to something not TOO unreasonable
            float tossSpeed = mouseDelta.magnitude / Time.deltaTime;
            heldSpriteTossable.Toss(mouseDelta * tossSpeed);

            heldObj.layer = 9; // tossed objects

            // Change weight in boat
            boat.RemoveLoad(holdWeight.Value);

            characterSet.Remove(heldCharActionable);
            heldSpriteTossable.RemoveFromSet(); // TODO: Flesh this out - remove from everything it could be wasting calculations with
            heldCharActionable = null;
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
        if (heldCharActionable != null)
            heldCharActionable.UseItem(item);
    }
    public void OnItemDeselectionCB(ItemBase item) {
        if(item is ItemCanScoop) {
            // TODO: Fill out as needed
        }
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
            foreach (ItemCanScoop item in itemsCanScoop)
                item.HighlightToClick();
        }
    }
    public Coroutine StartRemoveWaterCB(int waterWeight, float removalRate) {
        Coroutine retCo = StartCoroutine(RemoveWaterInterval(waterWeight, removalRate));
        removeWaterCoroutines.Add(retCo);
        return retCo;
    }
    public void StopRemoveWaterCB(Coroutine co) {
        StopCoroutine(co);
        removeWaterCoroutines.Remove(co);
    }

    IEnumerator SinkShipInterval() {
        while (levelActive) {
            if(!levelPaused)
                boat.AddWater();
            yield return new WaitForSeconds(Consts.SINK_STEP_SECS);
        }
    }

    IEnumerator RemoveWaterInterval(int waterWeight, float removalRate) {
        while (levelActive) {
            if (!levelPaused)
                boat.RemoveWater(waterWeight);
            yield return new WaitForSeconds(removalRate);
        }
    }

    public void ReturnToNeutral() {
        if (heldCharActionable != null) {
            heldCharActionable.IsActionBtnActive = false;
            heldCharActionable.IsCommandPanelOpen = false;
            heldCharActionable.CancelAction();
            heldCharActionable.ChangeMouseUpToDownLinks(true);
        }

        if(heldSpriteTossable != null)
            heldSpriteTossable.ReturnToBoat();

        rearMenuFieldObj.SetActive(false);
        holdWeight.Value = 0;
        uiUpdate.RaiseEvent();

        foreach (ItemCanScoop item in itemsCanScoop)
            item.UnHighlight();

        levelState = Consts.LevelState.Default;
        UnPauseLevel();
    }
}