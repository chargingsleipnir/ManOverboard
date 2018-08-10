using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passenger : MonoBehaviour {

    public int weight = 100;
    public bool mouseHeld = false;

    public UnityEngine.UI.Text weightDisp;

    // Use this for initialization
    void Start () {
        weightDisp.text = "";
    }
	
	// Update is called once per frame
	void Update () {

    }

    private void OnMouseDown()
    {
        mouseHeld = true;
    }

    private void OnMouseUp()
    {
        mouseHeld = false;
        weightDisp.text = "";
    }

    private void OnMouseExit()
    {
        mouseHeld = false;
        weightDisp.text = "";
    }

    private void OnMouseOver()
    {
        if (mouseHeld)
            weightDisp.text = weight.ToString();
    }
}
