using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Consts {
    public enum ZLayers {
        ActionObjHighlight = 0,
        GrabbedObjHighlight = 1,
        Water = 2,
        BehindWater = 3,
        Boat = 4,
        BehindBoat = 5
    }
    public enum ItemType {
        Scooping = 0
    }
    public enum LevelState {
        Default,
        CharHeldToReplace,
        CharHeldToToss,
        CmdMenuOpen,
        ObjectSelection
    }
    public enum CollType {
        Items,
        Characters
    }
}
