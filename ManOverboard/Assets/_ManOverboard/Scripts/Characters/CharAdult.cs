using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharAdult : CharElder {
    protected override void Start() {
        strength = 100;
        speed = 100;
        Reset();
    }
}
