using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;

public class ItemBase : SpriteTossable {

    public delegate void DelPassItemBase(ItemBase item);
    DelPassItemBase OnItemSelect;

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

    public void HighlightToClick() {
        if (so.enabled)
            return;

        SortCompLayerChange(Consts.DrawLayers.FrontOfLevel4, null);
        so.enabled = true;
        selectable = true;
    }

    public void UnHighlight() {
        if (!so.enabled)
            return;

        SortCompFullReset();
        so.enabled = false;
        selectable = false;
        selected = false;
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

    public void SetItemSelectionCallback(DelPassItemBase OnItemSelect) {
        this.OnItemSelect = OnItemSelect;
    }
}
