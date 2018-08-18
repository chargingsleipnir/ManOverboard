using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;
using ZeroProgress.Common.Collections;

public class LevelManager : MonoBehaviour {

    private bool levelActive = true;
    const float SINK_STEP_SECS = 0.5f;
    private Coroutine currCoroutine;

    public Boat boat;

    // TODO: Will need to handle all character types in the future.
    public GameObjectSet passengerSet;

    public IntReference loadWeight;
    public StringReference levelCompMsg;

    private void Awake() {
        currCoroutine = null;
        loadWeight.Value = 0;

        // TODO: Will need to handle all character types in the future.
        foreach (GameObject passenger in passengerSet) {
            Passenger passScpt = passenger.GetComponent<Passenger>();
            loadWeight.Value += passScpt.Weight;
            passScpt.AddCharGrabCallback(boat.PauseSinking);
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

    private void CharRelease(bool charTossed, int charWeight) {
        if (charTossed) {
            loadWeight.Value -= charWeight;

            // level over - these conditions will become much more flushed out of course
            if (loadWeight.Value <= 0) {
                levelCompMsg.Value = "Everyone died!";
                levelActive = false;
                return;
            }
            else if (loadWeight.Value <= boat.Buoyancy) {
                levelCompMsg.Value = "You a winner!";
                levelActive = false;
                return;
            }
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
