using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;
using ZeroProgress.Common.Collections;

public class LevelManager : MonoBehaviour {

    Consts.LevelState levelState;

    public GameCtrl gameCtrl;

    // TODO: Adjust sink rate based on number of holes
    const float SINK_STEP_SECS = 0.25f;
    [SerializeField]
    private GameObject waterObj;
    [SerializeField]
    private GameObject rearMenuFieldPrefab;
    private GameObject rearMenuFieldObj;

    private bool levelActive = true;
    private Coroutine currCoroutine;

    public Boat boat;

    [SerializeField]
    private ComponentSet characterSet;
    private int charSetStartCount;

    [SerializeField]
    private ComponentSet itemsCanScoop;

    public IntReference loadWeight;

    private GameObject heldCharObj;
    private CharBase heldCharScpt;

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
        currCoroutine = null;

        boat.AddNumLeaksCallback(NumLeaks);
        charSetStartCount = characterSet.Count;
    }

    private void Start () {
        levelState = Consts.LevelState.Default;
        holdWeight.Value = 0;
        uiUpdate.RaiseEvent();

        Utility.RepositionZ(waterObj.transform, (float)Consts.ZLayers.Water);

        rearMenuFieldObj = Instantiate(rearMenuFieldPrefab) as GameObject;
        rearMenuFieldObj.GetComponent<RearMenuField>().Mngr = this;
        Utility.RepositionZ(rearMenuFieldObj.transform, (float)Consts.ZLayers.Water - 0.1f);
        rearMenuFieldObj.SetActive(false);

        levelActive = true;

        if (currCoroutine != null)
            StopCoroutine(currCoroutine);
        currCoroutine = StartCoroutine(SinkShipInterval());
    }

    private void Update() {
        if (levelState == Consts.LevelState.CharHeldToToss) {
            heldCharScpt.MoveRigidbody();
            mouseDelta = mousePos.Value - mouseLastPos;
            mouseLastPos = mousePos.Value;
        }        
    }

    private void MouseEnterCharContArea() {
        levelState = Consts.LevelState.CharHeldToReplace;
        heldCharObj.transform.position = grabPos;
        heldCharScpt.SetActionBtnActive(true);
    }

    private void MouseExitCharContArea() {
        levelState = Consts.LevelState.CharHeldToToss;
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
        if (levelState == Consts.LevelState.CmdMenuOpen && charObj != heldCharObj)
            ReturnToNeutral();

        heldCharObj = charObj;
        heldCharScpt = charObj.GetComponent<CharBase>();

        // Bring character to focus, in front of everything.
        Utility.RepositionZ(charObj.transform, (float)Consts.ZLayers.GrabbedObjHighlight);

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

        if (heldCharScpt.GetMenuOpen()) {
            levelState = Consts.LevelState.CmdMenuOpen;
            rearMenuFieldObj.SetActive(true);
            return;
        }

        if (levelState == Consts.LevelState.CharHeldToToss) {
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
        else if (levelState == Consts.LevelState.CharHeldToReplace) {
            heldCharScpt.ReturnToBoat();
        }

        levelState = Consts.LevelState.Default;

        holdWeight.Value = 0;
        uiUpdate.RaiseEvent();

        UnPauseLevel();
    }

    public void OnItemMouseUp(GameObject itemObj) {
        foreach (ItemBase item in itemsCanScoop) {
            item.UnHighlight();
        }
        heldCharScpt.UseItem(itemObj);
        levelState = Consts.LevelState.Default;
    }

    // TODO: need to find a better way to dynamically select item types. Passing raw ints in the inspector is insufficient
    public void ToState(Consts.LevelState state) {
        levelState = state;
    }
    public void HighlightToSelect(Consts.ItemType itemType) {
        levelState = Consts.LevelState.ObjectSelection;

        if (itemType == Consts.ItemType.Scooping) {
            foreach (ItemBase item in itemsCanScoop) {
                item.HighlightToClick();
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
            levelState = Consts.LevelState.Default;
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

    public void ReturnToNeutral() {
        Debug.Log("In ReturnToNeutral");
        if (heldCharScpt != null) {
            heldCharScpt.SetActionBtnActive(false);
            heldCharScpt.SetCommandBtnsActive(false);
            heldCharScpt.ReturnToBoat();
        }

        rearMenuFieldObj.SetActive(false);
        holdWeight.Value = 0;
        uiUpdate.RaiseEvent();
        levelState = Consts.LevelState.Default;
        UnPauseLevel();
    }
}
