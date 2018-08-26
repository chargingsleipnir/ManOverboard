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

    // TODO: Will need to handle all character types in the future.
    public ComponentSet characterSet;
    private int charSetStartCount;

    public IntReference loadWeight;
    public IntReference grabWeight;

    public GameObject charContAreaPrefab;
    private GameObject charContArea; 

    public StringReference levelCompMsg;

    private void Awake() {
        currCoroutine = null;

        boat.AddNumLeaksCallback(NumLeaks);

        grabWeight.Value = 0;

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

        if (currCoroutine != null)
            StopCoroutine(currCoroutine);
        currCoroutine = StartCoroutine(SinkShipInterval());
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

    private void CharGrab(Vector3 pos, Quaternion rot, Vector2 size, int weight, Character scpt) {
        if (!levelActive)
            return;

        charContArea = Instantiate(charContAreaPrefab, new Vector3(pos.x, pos.y, pos.z + 0.1f), rot) as GameObject;
        charContArea.transform.localScale = new Vector3(size.x, size.y, 1);

        scpt.GetCharContAreaBoxCollider(charContArea.GetComponent<BoxCollider2D>());

        grabWeight.Value = weight;

        PauseLevel();
    }

    private void CharRelease(bool charTossed) {
        if (!levelActive)
            return;

        Destroy(charContArea);

        if (charTossed)
            boat.LightenLoad(grabWeight.Value);

        grabWeight.Value = 0;
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
