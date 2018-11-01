using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;
using ZeroProgress.Common.Collections;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class CharBase : SpriteTossable, IMouseDownDetector, IMouseUpDetector {

    protected Consts.CharState state;

    public delegate void DelPassItemType(Consts.ItemType type);
    public delegate Coroutine DelPassWaterRemoveData(int waterWeight, float removalRate);
    public delegate void DelPassWaterRemoveCo(Coroutine co);

    protected List<ItemBase> heldItems;
    protected int heldItemWeight;
    public override int Weight {
        get { return weight + heldItemWeight; }
    }

    protected bool saved = false;
    public bool Saved {
        get {
            return saved;
        }
        set {
            saved = value;
            // TODO: Set animation
        }
    }

    public virtual bool IsActionBtnActive { get { return false; } set { } }
    public virtual bool IsCancelBtnActive { get { return false; } set { } }
    public virtual bool IsCommandPanelOpen { get { return false; } set { } }

    protected override void Start () {
        base.Start();

        state = Consts.CharState.Default;
        heldItems = new List<ItemBase>();
        heldItemWeight = 0;

        SortCompLayerChange(Consts.DrawLayers.BoatLevel1Contents);
    }

    public override void MouseDownCB() {
        if (saved || tossed)
            return;

        held = true;
        OnMouseDownCB(gameObject);
    }
    public override void MouseUpCB() {
        if (saved || tossed || held == false)
            return;

        held = false;
        OnMouseUpCB();
    }

    public void LoseItem(ItemBase item) {
        if (heldItems.Remove(item))
            heldItemWeight -= item.Weight;
    }

    public virtual void UseItem(ItemBase item) { }
    public virtual void SetCallbacks(DelPassItemType setTypeCB, DelPassWaterRemoveData startCoCB, DelPassWaterRemoveCo stopCoCB, DelVoid fadeLevelCB, DelVoid unfadeLevelCB) { }
}
