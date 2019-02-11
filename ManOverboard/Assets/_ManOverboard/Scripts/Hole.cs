using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : SpriteBase, IMouseUpDetector {
    public int LeakRate { get; private set; }
    public int HeightByBuoyancy { get; private set; }
    private LevelManager lvlMngr;

    [SerializeField]
    private GameObject selectionArrow;

    [SerializeField]
    private GameObject bandage;

    public bool InRepair { get; set; }

    public void Init(LevelManager lvlMngr, int leakRate, int heightByBuoyancy) {
        this.lvlMngr = lvlMngr;
        this.LeakRate = leakRate;
        this.HeightByBuoyancy = heightByBuoyancy;
        InRepair = false;
    }

    public void MouseUpCB() {
        if (selectable)
            lvlMngr.OnSelection(this);
    }

    // TODO: FIND BETTER/PROPER WAY TO HIGHLIGHT HOLES
    public override void HighlightToSelect() {
        if (InRepair)
            return;
        if (selectionArrow.activeSelf)
            return;

        EnableMouseTracking(true);
        SortCompLayerChange(Consts.DrawLayers.FrontOfLevel4, null);
        selectionArrow.SetActive(true);
        selectable = true;
    }

    public override void UnHighlight() {
        if (!selectionArrow.activeSelf)
            return;

        EnableMouseTracking(false);
        SortCompResetToBase();
        selectionArrow.SetActive(false);
        selectable = false;
    }

    public void Repair() {
        bandage.SetActive(true);
    }
}