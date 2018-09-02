using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;
using ZeroProgress.Common.Collections;

public class LevelManager : MonoBehaviour {

    public GameCtrl gameCtrl;

    // TODO: Adjust sink rate based on number of holes
    const float SINK_STEP_SECS = 0.25f;
    [SerializeField]
    private GameObject waterObj;

    private bool levelActive = true;
    private Coroutine currCoroutine;

    public Boat boat;

    public ComponentSet characterSet;
    private int charSetStartCount;

    public IntReference loadWeight;

    private GameObject heldCharObj;
    private CharBase heldCharScpt;
    Rect actionBtnRect;

    public IntReference holdWeight;
    private bool prepToss = false;

    public GameObject charContAreaPrefab;
    private CharContArea charContAreaScpt;

    public StringReference levelCompMsg;

    // UI items
    [SerializeField]
    private Canvas uiCanvas;
    [SerializeField]
    private GameEvent threeStarWinEvent;
    [SerializeField]
    private GameEvent twoStarWinEvent;
    [SerializeField]
    private GameEvent oneStarWinEvent;
    [SerializeField]
    private GameEvent levelLostEvent;

    // Mouse tracking
    private Vector3 grabPos;
    private Vector2 mouseLastPos = Vector2.zero;
    private Vector2 mouseCurrPos = Vector2.zero;
    private Vector2 mouseDelta = Vector2.zero;

    private void Awake() {
        currCoroutine = null;

        boat.AddNumLeaksCallback(NumLeaks);

        holdWeight.Value = 0;

        charSetStartCount = characterSet.Count;
        foreach (CharBase character in characterSet) {
            character.AddCharGrabCallback(CharHold);
            character.AddCharReleaseCallback(CharRelease);
        }

        //characterSet.OnItemRemoved += CharReleased;
    }

    private void Start () {
        Utility.RepositionZ(waterObj.transform, (float)Consts.ZLayers.Water);

        levelCompMsg.Value = "";
        levelActive = true;
        heldCharObj = null;
        heldCharScpt = null;
        actionBtnRect = new Rect(0, 0, 0, 0);

        if (currCoroutine != null)
            StopCoroutine(currCoroutine);
        currCoroutine = StartCoroutine(SinkShipInterval());
    }

    private void Update() {
        if (heldCharObj == null)
            return;

        if (charContAreaScpt == null)
            return;

        mouseLastPos = mouseCurrPos;

        Vector3 mouseCalcPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
        mouseCurrPos.Set(mouseCalcPos.x, mouseCalcPos.y);

        charContAreaScpt.CheckMouseOverlap(mouseCurrPos);

        if (prepToss) {
            heldCharScpt.MoveRigidbody(mouseCurrPos);
            mouseDelta = mouseCurrPos - mouseLastPos;
        }
    }

    private void MouseEnterCharContArea() {
        prepToss = false;
        heldCharObj.transform.position = grabPos;
        if (heldCharScpt is CharActionable)
            heldCharScpt.SetActionBtnActive(true);
    }

    private void MouseExitCharContArea() {
        prepToss = true;
        if (heldCharScpt is CharActionable)
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

    private void CharHold(Vector3 pos, Quaternion rot, Vector2 size, int weight, GameObject charObj) {
        if (!levelActive)
            return;

        heldCharObj = charObj;
        heldCharScpt = charObj.GetComponent<CharBase>();

        // maintain copy of character's position where grabbed
        grabPos = pos;

        // Display action button for this character, if available
        // Incorporate size/posiiton of action button in character containment area
        
        if (heldCharScpt is CharActionable) {
            heldCharScpt.SetActionBtnActive(true);
            actionBtnRect = heldCharScpt.GetActionBtnRect(true);
        }
        else {
            actionBtnRect.Set(0, 0, 0, 0);
        }

        // Create area where character can be returned if player doesn't want to toss it.
        GameObject charContArea = Instantiate(charContAreaPrefab, new Vector3(pos.x, pos.y - (actionBtnRect.height * 0.5f), (float)Consts.ZLayers.Front + 0.1f), rot) as GameObject;
        charContArea.transform.localScale = new Vector3(Utility.GreaterOf(size.x, actionBtnRect.width), size.y + actionBtnRect.height, 1);
        charContAreaScpt = charContArea.GetComponent<CharContArea>();
        charContAreaScpt.SetMouseCBs(MouseEnterCharContArea, MouseExitCharContArea);

        holdWeight.Value = weight;

        PauseLevel();
    }


    private void CharRelease() {

        if (!levelActive)
            return;

        // TODO: Flesh this out - new menu needs to appear, listing commands available to this character.

        // As this will be a touch game, maybe move "action" button to the top, and commands to the side? If not all around,
        // if any at all. (Maybe go straight to having interactable items highlight. Do buttons for now. Some chars will have commands not involving items.)

        // What's the exact process here...
        // Level still paused while options up.
        // Character can be selected and dragged again, essentially reverting to previous control state
        // If player clicks anything other than character or action options, close options, resume normal state of play


        if (heldCharScpt is CharActionable) {
            if (actionBtnRect.Contains(mouseCurrPos)) {
                heldCharScpt.SetCommandBtnsActive(true);
            }
            heldCharScpt.SetActionBtnActive(false);
        }

        Destroy(charContAreaScpt.gameObject);
        charContAreaScpt = null;

        if (prepToss) {
            Transform t = heldCharObj.transform;
            t.parent = null;
            // Move in front of all other non-water objects
            Utility.RepositionZ(t, (float)Consts.ZLayers.BehindWater);
            heldCharObj.layer = 9; // tossed objects

            // TODO: Top out the toss speed to something not TOO unreasonable
            float tossSpeed = mouseDelta.magnitude / Time.deltaTime;
            heldCharObj.GetComponent<CharBase>().Toss(mouseDelta * tossSpeed);

            // Change weight in boat
            boat.LightenLoad(holdWeight.Value);
        }
        else {
            heldCharObj.GetComponent<CharBase>().ReturnToBoat();
        }

        holdWeight.Value = 0;
        heldCharObj = null;

        UnPauseLevel();
    }


    private void NumLeaks(int numLeaks) {
        // TODO: Have the number of leaks on the gui somewhere?
        if(numLeaks == 0) {

            // level over
            int charLoss = charSetStartCount - characterSet.Count;

            if (charLoss <= gameCtrl.GetLevelMaxCharLoss(3)) {
                threeStarWinEvent.RaiseEvent();
            }
            else if (charLoss <= gameCtrl.GetLevelMaxCharLoss(2)) {
                twoStarWinEvent.RaiseEvent();
            }
            else if (charLoss <= gameCtrl.GetLevelMaxCharLoss(1)) {
                oneStarWinEvent.RaiseEvent();
            }
            else {
                levelLostEvent.RaiseEvent();
            }
        }
    }

    private void EndLevel(string msg) {
        levelCompMsg.Value = msg;
        levelActive = false;

        foreach (CharBase character in characterSet) {
            character.Saved = true;
        }
    }

    IEnumerator SinkShipInterval() {
        while (levelActive) {
            boat.SinkInterval();
            yield return new WaitForSeconds(SINK_STEP_SECS);
        }
    }
}
