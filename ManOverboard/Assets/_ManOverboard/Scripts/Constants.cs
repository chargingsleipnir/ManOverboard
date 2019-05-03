using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Consts {
    public const int LEVEL_SCENE_IDX_DIFF = 2;
    public const int LEVEL_DATA_IDX_DIFF = 1;

    public const float CONT_AREA_BUFFER = 0.0f;

    public const float BOAT_LEDGE_FLOOR_DIFF = 0.2f;
    public const float ITEM_DROP_X_BUFF = 0.25f;

    public const float SINK_STEP_SECS = 0.25f;

    public const float BTN_DISABLE_FADE = 0.33f;

    public const float SCOOP_RATE = 200.0f;
    public const float DON_RATE = 250.0f;
    public const float REPAIR_RATE = 500.0f;
    public const float ATTACK_RATE = 25.0f;

    public const float TOSS_NOISE_MIN = -10.0f;
    public const float TOSS_NOISE_MAX = 10.0f;

    public const float OFFSCREEN_CATCH_BUFF = 5.0f;

    public const float BUOY_ROT_SPEED = 270.0f; // Degrees per second

    public const float MIN_SEL_REACH_DIST = 0.6f;
    public const float MOVE_SPEED_REDUC = 0.02f;

    public enum LeakTypesAndRates {
        Pinhole = 1,
        Bullet = 3,
        CannonBall = 10
    }

    public enum ZLayers {
        BehindWater = 1,
        Water = 0,
        FrontOfWater = -1,
    }

    public enum DrawLayers {
        Default,
        Background2,
        Background1,
        BehindBoat,
        BoatLevel1Rear,
        BoatLevel1Mid,
        BoatLevel1Front,
        BehindWater,
        Water,
        Foreground1,
        Foreground2,
        FrontOfLevel1,
        FrontOfLevel2,
        FrontOfLevel3,
        FrontOfLevel4
    }

    public enum UnityLayers {
        Water = 4,
        TossedObj = 9,
        Envir = 10,
        FloatDev = 11,
        Enemy = 12
    }

    public static int LAYER_COUNT { get; private set; }
    public static void Init() {
        LAYER_COUNT = Enum.GetValues(typeof(DrawLayers)).Length;
    }

    public enum HighlightGroupType {
        Scooping,
        LifeJacketChild,
        LifeJacketAdult,
        Children,
        RepairKits,
        PinHoles
    }
    public enum FitSizes { adult, child }
    public enum CharState {
        Default,
        InMenu,
        Walking,
        InAction,
        Dazed,
        Dead,
        Saved
    }
    public enum LevelState {
        Default,
        SpriteHeldToToss,
        ObjectSelection
    }
    public enum CollType {
        Items,
        Characters
    }
    public enum Skills {
        None,
        DonLifeJacket,
        ScoopWater,
        RepairPinhole,
        LowerAnchor,
        ReleaseAnchor,
        RaiseSail
    }
}
