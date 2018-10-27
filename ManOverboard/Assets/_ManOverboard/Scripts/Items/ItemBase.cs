using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;

public class ItemBase : SpriteTossable {

    public delegate void DelPassItemBase(ItemBase item);
    DelPassItemBase OnItemSelect;
    DelPassItemBase OnItemDeselect;

    public CharBase heldBy;

    protected SpriteOutline so;
    protected bool selectable;
    protected bool selected;

    public bool Selected {
        get { return selected; }
        set { selected = value; }
    }

    protected override void Awake() {
        base.Awake();
        so = GetComponent<SpriteOutline>();
    }

    protected override void Start() {
        base.Start();

        selectable = false;
        selected = false;
    }

    public override void Toss(Vector2 vel) {
        if (heldBy != null)
            heldBy.LoseItem(this);

        base.Toss(vel);
    }

    public void HighlightToClick() {
        if (so.enabled)
            return;

        ChangeMouseUpToDownLinks(false);
        SortCompLayerChange(Consts.DrawLayers.FrontOfLevel4, null);
        so.enabled = true;
        selectable = true;
    }

    public void UnHighlight() {
        if (!so.enabled)
            return;

        ChangeMouseUpToDownLinks(true);
        SortCompFullReset();
        so.enabled = false;
        selectable = false;
        selected = false;
    }

    public override void MouseDownCB() {
        if (selectable)
            return;

        base.MouseDownCB();
    }

    public override void MouseUpCB() {
        if (selectable) {
            selected = true;
            OnItemSelect(this);
        }
        else {
            base.MouseUpCB();
        }
    }

    public void Deselect() {
        SortCompFullReset();
        OnItemDeselect(this);
    }

    public void SetItemSelectionCallback(DelPassItemBase OnItemSelect, DelPassItemBase OnItemDeselect) {
        this.OnItemSelect = OnItemSelect;
        this.OnItemDeselect = OnItemDeselect;
    }
}
