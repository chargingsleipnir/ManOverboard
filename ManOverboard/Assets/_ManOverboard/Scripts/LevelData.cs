using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelList {
    public int[] maxCharLoss;
}

[Serializable]
public class JSONRoot {
    public LevelList[] level;
}
