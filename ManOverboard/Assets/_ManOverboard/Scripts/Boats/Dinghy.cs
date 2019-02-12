using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dinghy : Boat {

    const int BUOYANCY_MAX = 1000;

    public override void Start() {
        if (startRan)
            return;

        OnStart(BUOYANCY_MAX);
        startRan = true;
    }
}
