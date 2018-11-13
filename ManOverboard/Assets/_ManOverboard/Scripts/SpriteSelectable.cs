using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Make sure every derived object has a SpriteOutline component

[RequireComponent(typeof(SpriteOutline))]
public class SpriteSelectable : SpriteBase {

    protected SpriteOutline so;
    protected bool selectable;

    protected override void Awake() {
        base.Awake();

        so = GetComponent<SpriteOutline>();
        so.ChangeColour(0, 1.0f, 0.18f, 1.0f);
        so.enabled = false;
    }

    protected override void Start() {
        Reset();
    }

    protected override void Reset() {
        base.Reset();
        selectable = false;
    }

    public virtual void HighlightToClick() {
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

        // If sprite tossable overrides UnHighlight(), use ChangeMouseUpToDownLinks(true); instead of ResetMouseUpToDownLinks();
        ResetMouseUpToDownLinks();
        SortCompFullReset();
        so.enabled = false;
        selectable = false;
    }
}
