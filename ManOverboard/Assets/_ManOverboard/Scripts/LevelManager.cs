using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;
using ZeroProgress.Common.Collections;

public class LevelManager : MonoBehaviour {

    public GameCtrl gameCtrl;

    const float SINK_STEP_SECS = 0.5f;

    private bool levelActive = true;    
    private Coroutine currCoroutine;

    public Boat boat;

    // TODO: Will need to handle all character types in the future.
    public GameObjectSet passengerSet;
    private int passSetStartLen;

    public IntReference loadWeight;
    public IntReference grabWeight;

    public GameObject charContAreaPrefab;
    private GameObject charContArea; 

    public StringReference levelCompMsg;

    private void Awake() {
        currCoroutine = null;
        loadWeight.Value = 0;
        grabWeight.Value = 0;

        // TODO: Will need to handle all character types in the future.
        passSetStartLen = passengerSet.Count;
        foreach (GameObject passenger in passengerSet) {
            Passenger passScpt = passenger.GetComponent<Passenger>();
            loadWeight.Value += passScpt.weight;
            passScpt.AddCharGrabCallback(CharGrab);
            passScpt.AddCharReleaseCallback(CharRelease);
        }

        passengerSet.OnItemRemoved += CharReleased;
    }

    private void Start () {
        levelCompMsg.Value = "";
        levelActive = true;

        if (currCoroutine != null)
            StopCoroutine(currCoroutine);
        currCoroutine = StartCoroutine(SinkShipInterval());
    }

    private void CharGrab(Vector3 pos, Quaternion rot, Vector2 size, int weight, Character scpt) {
        if (!levelActive)
            return;

        charContArea = Instantiate(charContAreaPrefab, new Vector3(pos.x, pos.y, pos.z + 0.1f), rot) as GameObject;
        charContArea.transform.localScale = new Vector3(size.x, size.y, 1);

        scpt.GetCharContAreaBoxCollider(charContArea.GetComponent<BoxCollider2D>());

        grabWeight.Value = weight;
        boat.PauseSinking();
    }

    private void EndLevel(string msg) {
        levelCompMsg.Value = msg;
        levelActive = false;

        foreach (GameObject passenger in passengerSet) {
            passenger.GetComponent<Passenger>().Saved = true;
        }
    }

    private void CheckLevelEndResult() {
        gameCtrl.GetCurrLevel();
    }

    private void CharRelease(bool charTossed) {
        if (!levelActive)
            return;

        Destroy(charContArea);

        if (charTossed) {
            loadWeight.Value -= grabWeight.Value;
            grabWeight.Value = 0;
            // level over - these conditions will become much more flushed out of course
            if (loadWeight.Value <= 0) {
                EndLevel("Everyone died!");
                return;
            }
            else if (loadWeight.Value <= boat.Buoyancy) {
                int charLoss = passSetStartLen - passengerSet.Count;
 
                if(charLoss <= gameCtrl.GetLevelMaxCharLoss(3)) {
                    EndLevel("You a winner! 3 star play!");
                }
                else if(charLoss <= gameCtrl.GetLevelMaxCharLoss(2)) {
                    EndLevel("You a winner! 2 star play!");
                }
                else if(charLoss <= gameCtrl.GetLevelMaxCharLoss(1)) {
                    EndLevel("You a winner! 1 star play!");
                }
                else {
                    EndLevel("Too many people died!");
                }
                    
                return;
            }
        }
        else {
            grabWeight.Value = 0;
        }

        boat.ResumeSinking();
    }

    void CharReleased(object sender, SetModifiedEventArgs<GameObject> e) {
        //Debug.Log("Passenger tossed!");
        //Debug.Log(sender);
        //Debug.Log(e);
    }

    IEnumerator SinkShipInterval() {
        while (levelActive) {
            boat.SinkInterval();
            yield return new WaitForSeconds(SINK_STEP_SECS);
        }
    }
}
