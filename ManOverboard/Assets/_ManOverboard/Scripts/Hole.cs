using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : SpriteBase, IMouseUpDetector {
    public int LeakRate { get; private set; }
    public int HeightByBuoyancy { get; private set; }
    private LevelManager lvlMngr;

    [SerializeField]
    private GameObject bandage;

    public void Init(LevelManager lvlMngr, int leakRate, int heightByBuoyancy) {
        this.lvlMngr = lvlMngr;
        this.LeakRate = leakRate;
        this.HeightByBuoyancy = heightByBuoyancy;
    }

    public void MouseUpCB() {
        if (selectable)
            lvlMngr.OnSelection(this);
    }

    // TODO: FIND BETTER/PROPER WAY TO HIGHLIGHT HOLES

    public override void AddHighlightComponent(bool enableComponent = false) {
        so = GetComponent<SpriteOutline>();
        if (so == null) {
            so = gameObject.AddComponent<SpriteOutline>();
            so.enabled = false;
        }

        so.OutlineSize = 3;

        so.ChangeColour(0, 1.0f, 0.18f, 1.0f);
        if (enableComponent)
            HighlightToSelect();
    }

    public override void HighlightToSelect() {
        if (so == null)
            return;
        if (so.enabled)
            return;

        EnableMouseTracking(true);
        SortCompLayerChange(Consts.DrawLayers.FrontOfLevel4, null);
        so.enabled = true;
        selectable = true;
    }

    public override void UnHighlight() {
        if (so == null)
            return;
        if (!so.enabled)
            return;

        EnableMouseTracking(false);
        SortCompResetToBase();
        so.enabled = false;
        selectable = false;
    }

    public void Repair() {
        bandage.SetActive(true);
    }
}