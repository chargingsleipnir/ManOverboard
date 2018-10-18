using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dinghy : Boat {

    const int BUOYANCY_MAX = 1000;
    public int BuoyancyMax {
        get { return BUOYANCY_MAX; }
    }

    protected override void Start() {
        base.Start();
        OnStart(BUOYANCY_MAX);
    }
}
