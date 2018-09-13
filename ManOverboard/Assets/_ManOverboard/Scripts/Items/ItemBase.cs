using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;

public class ItemBase : MonoBehaviour, IMouseUpDetector {

    protected SpriteOutline so;
    protected float currZPos;
    protected bool selectable;
    protected bool selected;
    public bool Selected {
        get { return selected; }
        set { selected = value; }
    }

    private Transform currParent;

    [SerializeField]
    protected Vector2Reference mousePos;
    [SerializeField]
    protected GameObjectParamEvent itemMouseUpEvent;

    protected void Awake() {
        so = GetComponent<SpriteOutline>();
    }

    protected void Start() {
        selectable = false;
        selected = false;
    }

    public void HighlightToClick() {
        currParent = transform.parent;
        transform.parent = null;

        currZPos = transform.position.z;
        Utility.RepositionZ(transform, (float)Consts.ZLayers.ActionObjHighlight);
        so.enabled = true;
        selectable = true;
    }

    public void UnHighlight() {
        Utility.RepositionZ(transform, currZPos);
        transform.parent = currParent;

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
