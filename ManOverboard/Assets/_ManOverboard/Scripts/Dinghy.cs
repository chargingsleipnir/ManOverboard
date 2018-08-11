using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dinghy : MonoBehaviour {

    delegate void PauseSinkDelegate();
    PauseSinkDelegate pauseSink;

    public int buoyancy = 350;
    private bool sinking = false;

    public GameObject [] passengers;
    private int loadWeight;

    public UnityEngine.UI.Text buoyancyDisp;
    public UnityEngine.UI.Text loadWeightDisp;
    public UnityEngine.UI.Text endLevelDisp;

    // Use this for initialization
    void Start () {
        buoyancyDisp.text = buoyancy.ToString();
        sinking = true;
        InvokeRepeating("Sink", 1.0f, 1.0f);

        for(var i = 0; i < passengers.Length; i++) {
            Passenger passScpt = passengers[i].GetComponent<Passenger>();
            loadWeight += passScpt.weight;
            passScpt.AddCharGrabCallback(PauseSinking);
            passScpt.AddCharReleaseCallback(ResumeSinking);
        }

        loadWeightDisp.text = loadWeight.ToString();
    }

    void Update() {
    }

    void Sink() {
        if (sinking) {
            buoyancy--;
            buoyancyDisp.text = buoyancy.ToString();
        }
    }

    void PauseSinking() {
        sinking = false;
    }
    void ResumeSinking(bool charTossed, int charWeight) {
        if(charTossed) {
            loadWeight -= charWeight;
            loadWeightDisp.text = loadWeight.ToString();

            // level over - these conditions will become much more flushed out of course
            if (loadWeight <= 0) {
                endLevelDisp.text = "Everyone died!";
                return;
            }
            else if(loadWeight <= buoyancy) {
                endLevelDisp.text = "You a winner!";
                return;
            }
        }

        sinking = true;
    }	
}
