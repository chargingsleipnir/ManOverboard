using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;

public class ItemBase : SpriteTossable, IMouseUpDetector {

    protected SpriteOutline so;
    protected bool selectable;
    protected bool selected;

    public bool Selected {
        get { return selected; }
        set { selected = value; }
    }

    [SerializeField]
    protected Vector2Reference mousePos;
    [SerializeField]
    protected GameObjectParamEvent itemMouseUpEvent;

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
        MoveToTopSpriteGroup();
        ChangeSortCompLayer(Consts.DrawLayers.FrontOfLevel4);
        so.enabled = true;
        selectable = true;
    }

    public void UnHighlight() {
        MoveToOrigSpriteGroup();
        ChangeSortCompLayer();
        so.enabled = false;
        selectable = false;
        selected = false;
    }

    public void MouseUpCB() {
        if (selectable) {
            selected = true;
            itemMouseUpEvent.RaiseEvent(gameObject);
        }
    }
}
