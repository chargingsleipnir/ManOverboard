using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;
using ZeroProgress.Common.Collections;

public class LevelManager : MonoBehaviour {

    public GameCtrl gameCtrl;

    // TODO: Adjust sink rate based on number of holes
    const float SINK_STEP_SECS = 0.25f;

    private bool levelActive = true;
    private Coroutine currCoroutine;

    public Boat boat;

    public ComponentSet characterSet;
    private int charSetStartCount;

    public IntReference loadWeight;

    private GameObject heldChar;
    private Rigidbody2D heldCharRB;
    public IntReference holdWeight;
    private bool prepToss = false;

    public GameObject charContAreaPrefab;
    private CharContArea charContAreaScpt;

    public StringReference levelCompMsg;

    public Canvas uiCanvas;
    public GameObject actionBtn;

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
        foreach (Character character in characterSet) {
            character.AddCharGrabCallback(CharGrab);
            character.AddCharReleaseCallback(CharRelease);
        }

        //characterSet.OnItemRemoved += CharReleased;
    }

    private void Start () {
        levelCompMsg.Value = "";
        levelActive = true;
        heldChar = null;

        if (currCoroutine != null)
            StopCoroutine(currCoroutine);
        currCoroutine = StartCoroutine(SinkShipInterval());
    }

    private void Update() {
        if (heldChar == null)
            return;

        if (charContAreaScpt == null)
            return;

        mouseLastPos = mouseCurrPos;

        Vector3 mouseCalcPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
        mouseCurrPos.Set(mouseCalcPos.x, mouseCalcPos.y);

        // TODO: Need to be able to hover over button(s) as well, when mouse is in the containment area.
        // Simple, make area big enough to accomodate them.
        charContAreaScpt.CheckMouseOverlap(mouseCurrPos);

        if (prepToss) {
            heldCharRB.MovePosition(mouseCurrPos);
            mouseDelta = mouseCurrPos - mouseLastPos;
        }
    }

    private void MouseEnterCharContArea() {
        prepToss = false;
        heldChar.transform.position = grabPos;
        if (heldChar.tag == "HasActions")
            actionBtn.SetActive(true);
    }

    private void MouseExitCharContArea() {
        prepToss = true;
        if (actionBtn.activeSelf)
            actionBtn.SetActive(false);
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

    private void CharGrab(Vector3 pos, Quaternion rot, Vector2 size, int weight, GameObject charObj) {
        if (!levelActive)
            return;

        heldChar = charObj;
        heldCharRB = charObj.GetComponent<Rigidbody2D>();

        // maintain copy of character's position where grabbed
        grabPos = pos;

        // Create area where character can be returned of player doesn't want to toss it.
        GameObject charContArea = Instantiate(charContAreaPrefab, new Vector3(pos.x, pos.y, pos.z + 0.1f), rot) as GameObject;
        charContArea.transform.localScale = new Vector3(size.x, size.y, 1);
        charContAreaScpt = charContArea.GetComponent<CharContArea>();
        charContAreaScpt.SetMouseCBs(MouseEnterCharContArea, MouseExitCharContArea);

        holdWeight.Value = weight;

        PauseLevel();

        // Display buttons/options this character has available
        if(heldChar.tag == "HasActions") {
            actionBtn.SetActive(true);
            RectTransform rt = actionBtn.GetComponent<RectTransform>();
            rt.position = Utility.WorldToUISpace(uiCanvas, new Vector3(pos.x, pos.y - (size.y*0.5f), 0.0f));
            rt.Translate(0.0f, -(rt.rect.height * 0.5f), 0.0f);
        }
    }

    private void CharRelease() {
        if (actionBtn.activeSelf)
            actionBtn.SetActive(false);

        if (!levelActive)
            return;

        Destroy(charContAreaScpt.gameObject);
        charContAreaScpt = null;


        if (prepToss) {
            Transform t = heldChar.transform;
            t.parent = null;
            // Move in front of all other non-water objects
            t.position = new Vector3(t.position.x, t.position.y, -0.1f);
            heldChar.layer = 9; // tossed objects

            // TODO: Top out the toss speed to something not TOO unreasonable
            float tossSpeed = mouseDelta.magnitude / Time.deltaTime;
            heldChar.GetComponent<Character>().Toss(mouseDelta * tossSpeed);

            // Change weight in boat
            boat.LightenLoad(holdWeight.Value);
        }
        else {
            heldChar.transform.Translate(0, 0, 1.2f);
        }

        holdWeight.Value = 0;
        heldChar = null;

        UnPauseLevel();
    }

    void CharReleased(object sender, SetModifiedEventArgs<Character> e) {
        //Debug.Log("Passenger tossed!");
        //Debug.Log(sender);
        //Debug.Log(e);
    }

    private void NumLeaks(int numLeaks) {
        // TODO: Have the number of leaks on the gui somewhere?
        if(numLeaks == 0) {

            // level over
            int charLoss = charSetStartCount - characterSet.Count;

            if (charLoss <= gameCtrl.GetLevelMaxCharLoss(3)) {
                EndLevel("You a winner! 3 star play!");
            }
            else if (charLoss <= gameCtrl.GetLevelMaxCharLoss(2)) {
                EndLevel("You a winner! 2 star play!");
            }
            else if (charLoss <= gameCtrl.GetLevelMaxCharLoss(1)) {
                EndLevel("You a winner! 1 star play!");
            }
            else {
                EndLevel("Too many people died!");
            }
        }
    }

    private void EndLevel(string msg) {
        levelCompMsg.Value = msg;
        levelActive = false;

        foreach (Character character in characterSet) {
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
