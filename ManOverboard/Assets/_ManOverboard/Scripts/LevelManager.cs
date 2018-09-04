using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;
using ZeroProgress.Common.Collections;

public class LevelManager : MonoBehaviour {

    private enum CtrlState {
        None,
        CharHeldToReplace,
        CharHeldToToss,        
        CmdMenuOpen,
        ObjectSelection
    }
    CtrlState ctrlState;

    public GameCtrl gameCtrl;

    // TODO: Adjust sink rate based on number of holes
    const float SINK_STEP_SECS = 0.25f;
    [SerializeField]
    private GameObject waterObj;

    private bool levelActive = true;
    private Coroutine currCoroutine;

    public Boat boat;

    [SerializeField]
    private ComponentSet characterSet;
    private int charSetStartCount;

    [SerializeField]
    private GameObjectSet itemsScooping;

    public IntReference loadWeight;

    private GameObject heldCharObj;
    private CharBase heldCharScpt;

    public IntReference holdWeight;

    public GameObject charContAreaPrefab;
    private CharContArea charContAreaScpt;

    // Mouse tracking
    [SerializeField]
    private Vector2Reference mousePos;

    // UI update event
    [SerializeField]
    private GameEvent uiUpdate;

    // Level over event
    [SerializeField]
    private StringParamEvent levelWinLossDisp;

    // Mouse tracking
    private Vector3 grabPos;
    private Vector2 mouseLastPos = Vector2.zero;
    private Vector2 mouseCurrPos = Vector2.zero;
    private Vector2 mouseDelta = Vector2.zero;

    private void Awake() {
        currCoroutine = null;

        boat.AddNumLeaksCallback(NumLeaks);

        holdWeight.Value = 0;
        uiUpdate.RaiseEvent();

        charSetStartCount = characterSet.Count;
    }

    private void Start () {
        ctrlState = CtrlState.None;

        Utility.RepositionZ(waterObj.transform, (float)Consts.ZLayers.Water);

        levelActive = true;

        if (currCoroutine != null)
            StopCoroutine(currCoroutine);
        currCoroutine = StartCoroutine(SinkShipInterval());
    }

    private void Update() {
        if (ctrlState == CtrlState.None)
            return;

        mouseLastPos = mouseCurrPos;
        Vector3 mouseCalcPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
        mouseCurrPos.Set(mouseCalcPos.x, mouseCalcPos.y);
        mousePos.Value = mouseCurrPos;

        if (ctrlState == CtrlState.CharHeldToReplace) {
            charContAreaScpt.CheckMouseOverlap();
        }
        else if (ctrlState == CtrlState.CharHeldToToss) {
            heldCharScpt.MoveRigidbody();
            charContAreaScpt.CheckMouseOverlap();
            mouseDelta = mouseCurrPos - mouseLastPos;
        }
        else if (ctrlState == CtrlState.CmdMenuOpen) {
            if (Input.GetMouseButtonDown(0)) {
                // Player clicked off into nothing, go back to neutral state.
                if (!heldCharScpt.SpriteHovered() && !heldCharScpt.CmdPanelHovered()) {
                    ReturnToNeutral();
                }
            }
        }
        else if(ctrlState == CtrlState.ObjectSelection) {
            // TODO: Similar to cmdmenuopen - if anything but the appropriate items are clicked, return to neutral.
        }
    }

    private void MouseEnterCharContArea() {
        ctrlState = CtrlState.CharHeldToReplace;
        heldCharObj.transform.position = grabPos;
        heldCharScpt.SetActionBtnActive(true);
    }

    private void MouseExitCharContArea() {
        ctrlState = CtrlState.CharHeldToToss;
        heldCharScpt.SetActionBtnActive(false);
    }

    private void CheckLevelEndResult() {
        gameCtrl.GetCurrLevel();
    }

    private void PauseLevel() {
        boat.Sinking = false;
        // Pause animations of all characters except the one being held.
    }
    private void UnPauseLevel() {
        if (!levelActive)
            return;

        boat.Sinking = true;
    }

    public void OnCharMouseDown(GameObject charObj) {
        if (!levelActive)
            return;

        // If a different character's menu is currently open, close it.
        if (ctrlState == CtrlState.CmdMenuOpen && charObj != heldCharObj)
            ReturnToNeutral();

        heldCharObj = charObj;
        heldCharScpt = charObj.GetComponent<CharBase>();

        // Bring character to focus, in front of everything.
        Utility.RepositionZ(charObj.transform, (float)Consts.ZLayers.Front);

        // maintain copy of character's position where grabbed
        grabPos = charObj.transform.position;

        // Create area where character can be returned if player doesn't want to toss it.
        GameObject charContArea = Instantiate(charContAreaPrefab) as GameObject;
        heldCharScpt.ApplyTransformToContArea(charContArea);

        charContAreaScpt = charContArea.GetComponent<CharContArea>();
        charContAreaScpt.SetMouseCBs(MouseEnterCharContArea, MouseExitCharContArea);

        holdWeight.Value = heldCharScpt.weight;
        uiUpdate.RaiseEvent();

        PauseLevel();
    }

    public void OnCharMouseUp() {
        if (!levelActive)
            return;

        Destroy(charContAreaScpt.gameObject);
        charContAreaScpt = null;

        if (heldCharScpt.GetMenuOpen()) {
            ctrlState = CtrlState.CmdMenuOpen;
            return;
        }

        if (ctrlState == CtrlState.CharHeldToToss) {
            Transform t = heldCharObj.transform;
            t.parent = null;
            // Move in front of all other non-water objects
            Utility.RepositionZ(t, (float)Consts.ZLayers.BehindWater);
            heldCharObj.layer = 9; // tossed objects

            // TODO: Top out the toss speed to something not TOO unreasonable
            float tossSpeed = mouseDelta.magnitude / Time.deltaTime;
            heldCharScpt.Toss(mouseDelta * tossSpeed);

            // Change weight in boat
            boat.LightenLoad(holdWeight.Value);
        }
        else if (ctrlState == CtrlState.CharHeldToReplace) {
            heldCharScpt.ReturnToBoat();
        }

        ctrlState = CtrlState.None;

        holdWeight.Value = 0;
        uiUpdate.RaiseEvent();

        UnPauseLevel();
    }

    // TODO: need to find a better way to dynamically select item types. Passing raw ints in the inspector is insufficient
    public void ToStateObjectSelection(int itemType) {
        ctrlState = CtrlState.ObjectSelection;

        // Highlight appropriate items
        // Other state change effects
        if(itemType == (int)Consts.ItemType.Scooping) {
            foreach (GameObject item in itemsScooping) {
                Debug.Log("item.name: " + item.name);
            }
        }
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
            ctrlState = CtrlState.None;
            foreach (CharBase character in characterSet) {
                character.Saved = true;
            }
        }
    }

    IEnumerator SinkShipInterval() {
        while (levelActive) {
            boat.SinkInterval();
            yield return new WaitForSeconds(SINK_STEP_SECS);
        }
    }


    // Internal/helper methods ==========================================

    private void ReturnToNeutral() {
        if (heldCharScpt) {
            heldCharScpt.SetActionBtnActive(false);
            heldCharScpt.SetCommandBtnsActive(false);
            heldCharScpt.ReturnToBoat();
        }

        holdWeight.Value = 0;
        uiUpdate.RaiseEvent();
        ctrlState = CtrlState.None;
        UnPauseLevel();
    }
}
