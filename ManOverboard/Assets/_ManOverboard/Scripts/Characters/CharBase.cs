﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;
using ZeroProgress.Common.Collections;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class CharBase : SpriteTossable, IMouseDownDetector, IMouseUpDetector {

    public Consts.CharState state;

    public delegate void DelSetItemType(Consts.ItemType type);
    public delegate Coroutine DelStartCoroutine(int waterWeight, float removalRate);
    public delegate void DelStopCoroutine(Coroutine co);

    protected List<ItemBase> heldItems;

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

        SortCompLayerChange(Consts.DrawLayers.BoatLevel1Contents);
    }

    public void ChangeMouseUpToDownLinks(bool linkEvents) {
        RefShape2DMouseTracker[] trackers = GetComponents<RefShape2DMouseTracker>();
        for (int i = 0; i < trackers.Length; i++)
            trackers[i].LinkMouseUpToDown = linkEvents;
    }

    public override void MouseDownCB() {
        if (saved)
            return;

        base.MouseDownCB();
    }
    public override void MouseUpCB() {
        if (saved)
            return;

        base.MouseUpCB();
    }

    public virtual void UseItem(ItemBase item) { }
    public virtual void SetCallbacks(DelSetItemType setTypeCB, DelStartCoroutine startCoCB, DelStopCoroutine stopCoCB) { }
}
