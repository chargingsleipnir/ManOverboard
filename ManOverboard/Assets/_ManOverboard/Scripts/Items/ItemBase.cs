using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ZeroProgress.Common;

public class ItemBase : SpriteTossable {

    public CharBase CharHeldBy { get; set; }

    public bool InUse { get; set; }

    private ItemBaseSet items;

    protected override void Awake() {
        base.Awake();
        items = Resources.Load<ItemBaseSet>("ScriptableObjects/SpriteSets/ItemBaseSet");
        items.Add(this);
    }

    protected override void Start() {
        base.Start();

        InUse = false;
    }

    public override void Toss(Vector2 vel) {
        items.Remove(this);

        RemoveFromChar();

        base.Toss(vel);
    }
    public void RemoveFromChar() {
        if (CharHeldBy != null)
            CharHeldBy.LoseItem(this);
    }
    public override void MouseDownCB() {
        if (selectable)
            return;

        base.MouseDownCB();
    }

    public override void MouseUpCB() {
        if (InUse)
            return;

        if (selectable)
            lvlMngr.OnSelection(this);
        else
            base.MouseUpCB();
    }

    public override void HighlightToSelect() {
        if (InUse)
            return;

        base.HighlightToSelect();
    }

    public void Deselect() {
        SortCompFullReset();
        EnableMouseTracking(true);
        lvlMngr.OnDeselection(this);
        InUse = false;
    }
}
