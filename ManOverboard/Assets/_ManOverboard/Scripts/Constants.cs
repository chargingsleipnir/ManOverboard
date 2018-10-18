using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Consts {
    public const int LEVEL_SCENE_IDX_DIFF = 2;
    public const int LEVEL_DATA_IDX_DIFF = 1;

    public enum ZLayers {
        BehindWater = 1,
        Water = 0,
        FrontOfWater = -1,
    }

    public enum DrawLayers {
        Default,
        Background2,
        Background1,
        BoatLevel1Contents,
        BoatLevel1,
        BehindWater,
        Water,
        Foreground1,
        Foreground2,
        FrontOfLevel1,
        FrontOfLevel2,
        FrontOfLevel3,
        FrontOfLevel4
    }
    public static int LAYER_COUNT { get; private set; }
    public static void Init() {
        LAYER_COUNT = Enum.GetValues(typeof(DrawLayers)).Length;
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
