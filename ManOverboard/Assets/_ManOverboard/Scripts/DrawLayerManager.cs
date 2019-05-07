using System;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common.Collections;
using UnityEngine.UI;

public static class DrawLayerMngr {

    private static Dictionary<Consts.DrawLayers, string> sortingLayers;
    private static bool initialized = false;

    public static SortingGroupComponentRef topSortGroup;

    private static SpriteBase topSprite;

    // TODO: Erase when no longer needed
    private static Text debugTextBox1;
    private static Text debugTextBox2;

    // Set up once time per game
    public static void Init() {
        if (initialized)
            return;

        topSortGroup = new SortingGroupComponentRef();

        sortingLayers = new Dictionary<Consts.DrawLayers, string>();
        foreach (Consts.DrawLayers drawLayer in Enum.GetValues(typeof(Consts.DrawLayers)))
            sortingLayers.Add(drawLayer, drawLayer.ToString());

        initialized = true;

        // TODO: Erase when no longer needed
        //debugTextBox1 = GameObject.Find("DebugDisp1").GetComponent<Text>();
        //debugTextBox2 = GameObject.Find("DebugDisp2").GetComponent<Text>();
    }

    // TODO: Erase when no longer needed
    public static void Update(RefShape2DMouseTrackerSet mouseTrackerFullSet, RefShape2DMouseTrackerSet mouseTrackerEnteredSet) {
        //debugTextBox1.text = "RAW SPRITE DRAW ORDER\n";
        //debugTextBox1.text += topSortGroup.GetDebugString();

        //debugTextBox1.text = "MOUSE TRACKER ORDER\n";
        //for (int i = 0; i < mouseTrackerFullSet.Count; i++) {
        //    debugTextBox1.text += mouseTrackerFullSet[i].name + "\n";
        //}

        //debugTextBox2.text = "TRACKERS HOVERED\n";
        //for (int i = 0; i < mouseTrackerEnteredSet.Count; i++) {
        //    debugTextBox2.text += mouseTrackerEnteredSet[i].name + "\n";
        //}
    }

    // TODO: Function that simply says "Layer this over or under that". so when two things meet, it's that simple.

    public static void ClearSpriteRef() {
        topSortGroup.groupList.Clear();
    }
}