using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ZeroProgress.Common;

public class ItemBase : SpriteTossable {

    public delegate void DelPassItemBase(ItemBase item);
    DelPassItemBase OnItemSelect;
    DelPassItemBase OnItemDeselect;

    public CharBase CharHeldBy { get; set; }

    protected SpriteOutline so;
    protected bool selectable;
    protected bool inUse;

    private ItemBaseSet items;

    public Vector3? RetPosLocal { get; set; }

    protected override void Awake() {
        base.Awake();
        so = GetComponent<SpriteOutline>();

        items = AssetDatabase.LoadAssetAtPath<ItemBaseSet>("Assets/_ManOverboard/Variables/Sets/ItemBaseSet.asset");
        items.Add(this);
    }

    protected override void Start() {
        base.Start();

        selectable = false;
        inUse = false;
    }

    public override void Toss(Vector2 vel) {
        items.Remove(this);

        if (CharHeldBy != null)
            CharHeldBy.LoseItem(this);

        base.Toss(vel);
    }

    public void HighlightToClick() {
        if (inUse || so.enabled)
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
    }

    public override void MouseDownCB() {
        if (selectable)
            return;

        base.MouseDownCB();
    }

    public override void MouseUpCB() {
        if (inUse)
            return;

        if (selectable) {
            inUse = true;
            OnItemSelect(this);
        }
        else {
            base.MouseUpCB();
        }
    }

    public void Deselect() {
        SortCompFullReset();
        EnableMouseTracking(true);
        OnItemDeselect(this);
        inUse = false;
        if (RetPosLocal != null)
            transform.localPosition = (Vector3)RetPosLocal;
    }

    public void SetItemSelectionCallback(DelPassItemBase OnItemSelect, DelPassItemBase OnItemDeselect) {
        this.OnItemSelect = OnItemSelect;
        this.OnItemDeselect = OnItemDeselect;
    }
}
