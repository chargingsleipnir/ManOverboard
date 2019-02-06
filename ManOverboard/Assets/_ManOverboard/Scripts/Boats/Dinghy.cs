using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dinghy : Boat {

    const int BUOYANCY_MAX = 1000;

    protected void Start() {
        OnStart(BUOYANCY_MAX);
    }
}
