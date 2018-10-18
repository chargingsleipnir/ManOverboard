using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;
using ZeroProgress.Common.Collections;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class CharBase : SpriteTossable, IMouseDownDetector, IMouseUpDetector {

    protected const float CONT_AREA_BUFFER = 0.1f;

    protected bool saved = false;
    public bool Saved {
        get {
            return saved;
        }
        set {
            saved = value;
            // TODO: Set animation to have character doing arm-waving-happy-dance-deal.
        }
    }

    protected ComponentSetElement setElem;

    // Mouse tracking
    [SerializeField]
    protected Vector2Reference mousePos;
    [SerializeField]
    protected GameObjectParamEvent charMouseDownEvent;
    [SerializeField]
    protected GameEvent charMouseUpEvent;

    protected override void Awake() {
        base.Awake();
        setElem = GetComponent<ComponentSetElement>();
    }

    protected override void Start () {
        base.Start();

        // Rigidbody is essentially inactive until being flung/thrown by user.
        // (Could use posiiton constraints instead)
        rb.isKinematic = true;
        bc.enabled = false;

        ChangeSortCompLayer(Consts.DrawLayers.BoatLevel1Contents);
    }

    public void MoveRigidbody() {
        rb.MovePosition(mousePos.Value);
    }

    public virtual void ApplyTransformToContArea(GameObject contAreaObj) {
        contAreaObj.transform.position = new Vector3(transform.position.x, transform.position.y, (float)Consts.ZLayers.FrontOfWater);
        contAreaObj.transform.localScale = new Vector3(srRef.comp.size.x + CONT_AREA_BUFFER, srRef.comp.size.y + CONT_AREA_BUFFER, 1);
    }

    public void ChangeMouseUpWithDownLinks(bool linkEvents) {
        RefShape2DMouseTracker[] trackers = GetComponents<RefShape2DMouseTracker>();
        for (int i = 0; i < trackers.Length; i++)
            trackers[i].LinkMouseUpToDown = linkEvents;
    }

    public void Toss(Vector2 vel) {
        transform.parent = null;
        // Move in front of all other non-water objects
        ChangeSortCompLayer(Consts.DrawLayers.BehindWater);
        tossed = true;
        setElem.UnregisterComponent();
        rb.isKinematic = false;
        rb.velocity = vel;
        bc.enabled = true;
    }

    public void ReturnToBoat() {
        ChangeSortCompLayer(Consts.DrawLayers.BoatLevel1Contents);
    }

    public virtual void SetActionBtnActive(bool isActive) {}
    public virtual void SetCommandBtnsActive(bool isActive) {}
    public virtual bool GetMenuOpen() { return false; }
    public virtual void UseItem(GameObject item) {}

    public virtual void MouseDownCB() {
        if (saved || tossed)
            return;

        held = true;
        charMouseDownEvent.RaiseEvent(gameObject);
    }
    public virtual void MouseUpCB() {
        if (saved || tossed || held == false)
            return;

        held = false;
        charMouseUpEvent.RaiseEvent();
    }
}
