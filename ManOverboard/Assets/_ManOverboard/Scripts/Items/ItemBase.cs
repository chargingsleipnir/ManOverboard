using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ZeroProgress.Common;

public class ItemBase : SpriteTossable {

    public CharBase CharHeldBy { get; set; }

    public bool InUse { get; private set; }

    private ItemBaseSet items;

    public Vector3? RetPosLocal { get; set; }

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

        if (CharHeldBy != null)
            CharHeldBy.LoseItem(this);

        base.Toss(vel);
    }

    public override void MouseDownCB() {
        if (selectable)
            return;

        base.MouseDownCB();
    }

    public override void MouseUpCB() {
        if (InUse)
            return;

        if (selectable) {
            InUse = true;
            lvlMngr.OnSelection(this);
        }
        else {
            base.MouseUpCB();
        }
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
        if (RetPosLocal != null)
            transform.localPosition = (Vector3)RetPosLocal;
    }
}
