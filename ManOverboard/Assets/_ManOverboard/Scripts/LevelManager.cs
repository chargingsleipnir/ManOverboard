using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;
using ZeroProgress.Common.Collections;

public class LevelManager : MonoBehaviour {

    const float SINK_STEP_SECS = 0.5f;

    private bool levelActive = true;    
    private Coroutine currCoroutine;

    public Boat boat;

    // TODO: Will need to handle all character types in the future.
    public GameObjectSet passengerSet;
    public IntReference loadWeight;
    public IntReference grabWeight;

    public GameObject charContAreaPrefab;
    private GameObject charContArea;
    private BoxCollider2D charContAreaBC;    

    public StringReference levelCompMsg;

    private void Awake() {
        currCoroutine = null;
        loadWeight.Value = 0;
        grabWeight.Value = 0;

        // TODO: Will need to handle all character types in the future.
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
        if (levelActive) {
            charContArea = Instantiate(charContAreaPrefab, new Vector3(pos.x, pos.y, pos.z + 0.1f), rot) as GameObject;
            charContArea.transform.localScale = new Vector3(size.x, size.y, 1);

            scpt.GetCharContAreaBoxCollider(charContArea.GetComponent<BoxCollider2D>());

            grabWeight.Value = weight;
            boat.PauseSinking();
        }
    }

    private void EndLevel(string msg) {
        levelCompMsg.Value = msg;
        levelActive = false;

        foreach (GameObject passenger in passengerSet) {
            passenger.GetComponent<Passenger>().Saved = true;
        }
    }

    private void CharRelease(bool charTossed) {
        if (levelActive) {

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
                    EndLevel("You a winner!");
                    return;
                }
            }
            else {
                grabWeight.Value = 0;
            }

            boat.ResumeSinking();
        }
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
