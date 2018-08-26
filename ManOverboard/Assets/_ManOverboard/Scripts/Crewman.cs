using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crewman : Character {

    public override void Start () {
        base.Start();
    }

    public override void Update () {
        base.Update();
	}

    protected override void OnMouseDown() {
        base.OnMouseDown();

        // Need action command system
    }

    protected override void OnMouseUp() {
        base.OnMouseUp();

        // Need action command system
    }
}
