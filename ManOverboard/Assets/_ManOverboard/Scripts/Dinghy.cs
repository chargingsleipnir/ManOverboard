using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dinghy : MonoBehaviour {

    public int buoyancy = 350;
    public GameObject [] passengers;
    public int loadWeight;

    public UnityEngine.UI.Text buoyancyDisp;
    public UnityEngine.UI.Text loadWeightDisp;

    // Use this for initialization
    void Start () {
        buoyancyDisp.text = buoyancy.ToString();
        InvokeRepeating("Sink", 2.0f, 1.0f);

        for(var i = 0; i < passengers.Length; i++) {
            loadWeight += passengers[i].GetComponent<Passenger>().weight;
        }

        loadWeightDisp.text = loadWeight.ToString();
    }

    void Sink() {
        buoyancy--;
        buoyancyDisp.text = buoyancy.ToString();
    }
	
	// Update is called once per frame
	void Update () {
	}
}
