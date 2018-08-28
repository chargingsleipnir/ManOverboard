using System.Collections;
using UnityEngine;
using ZeroProgress.Common;
using ZeroProgress.Common.Collections;

public class AdamLevelManager : MonoBehaviour {

    public GameCtrl gameCtrl;

    const float SINK_STEP_SECS = 0.5f;

    private bool levelActive = true;    
    private Coroutine currCoroutine;

    public Boat boat;

    // TODO: Will need to handle all character types in the future.
    public GameObjectSet passengerSet;
    private int passSetStartLen;
    
    public int PassengerStartingCount
    {
        get { return passSetStartLen; }
    }


    public IntReference loadWeight;
    public IntReference grabWeight;

    public GameObject charContAreaPrefab;
    private GameObject charContArea; 
    
    [SerializeField]
    private GameEvent VictoryEvent;

    [SerializeField]
    private GameEvent LossEvent;

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
    
    private void CharRelease(bool charTossed) {
        if (levelActive) {

            Destroy(charContArea);

            if (charTossed) {
                loadWeight.Value -= grabWeight.Value;
                grabWeight.Value = 0;
                // level over - these conditions will become much more flushed out of course
                if (loadWeight.Value <= 0) {
                    LossEvent.RaiseEvent();
                    return;
                }
                else if (loadWeight.Value <= boat.Buoyancy) {
                                       

                    int charLoss = passSetStartLen - passengerSet.Count;

                    if (charLoss > gameCtrl.GetLevelMaxCharLoss(3))
                        LossEvent.RaiseEvent();
                    else
                        VictoryEvent.RaiseEvent();

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
