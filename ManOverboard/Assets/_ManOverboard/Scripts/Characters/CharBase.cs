using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;
using ZeroProgress.Common.Collections;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class CharBase : MonoBehaviour {

    protected const float CONT_AREA_BUFFER = 0.15f;

    protected bool tossed = false;
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

    public int weight;

    protected SpriteRenderer sr;
    protected Rigidbody2D rb;
    protected BoxCollider2D bc;    
    protected ComponentSetElement setElem;

    // Mouse tracking
    [SerializeField]
    protected Vector2Reference mousePos;
    [SerializeField]
    protected GameObjectParamEvent charMouseDownEvent;
    [SerializeField]
    protected GameEvent charMouseUpEvent;

    protected virtual void Awake() {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();        
        bc = GetComponent<BoxCollider2D>();
        setElem = GetComponent<ComponentSetElement>();
    }

    public virtual void Start () {
        // Rigidbody is essentially inactive until being flung/thrown by user.
        // (Could use posiiton constraints instead) 
        rb.isKinematic = true;
        Utility.RepositionZ(transform, (float)Consts.ZLayers.BehindBoat);
    }

    public void MoveRigidbody() {
        rb.MovePosition(mousePos.Value);
    }

    public virtual void ApplyTransformToContArea(GameObject contAreaObj) {
        contAreaObj.transform.position = new Vector3(transform.position.x, transform.position.y, (float)Consts.ZLayers.Front + 0.1f);
        contAreaObj.transform.localScale = new Vector3(sr.size.x + CONT_AREA_BUFFER, sr.size.y + CONT_AREA_BUFFER, 1);
    }

    public void Toss(Vector2 vel) {
        Utility.RepositionZ(transform, (float)Consts.ZLayers.BehindWater);
        tossed = true;
        setElem.UnregisterComponent();
        rb.isKinematic = false;
        rb.velocity = vel;
    }

    public void ReturnToBoat() {
        Utility.RepositionZ(transform, (float)Consts.ZLayers.BehindBoat);
    }

    public bool SpriteHovered() {
        return bc.OverlapPoint(mousePos.Value);
    }

    public virtual void SetActionBtnActive(bool isActive) {}
    public virtual void SetCommandBtnsActive(bool isActive) {}
    public virtual bool GetMenuOpen() { return false; }
    public virtual bool CmdPanelHovered() { return false; }

    protected virtual void OnMouseDown() {
        if (saved || tossed)
            return;

        charMouseDownEvent.RaiseEvent(gameObject);
    }

    protected virtual void OnMouseUp() {
        if (saved || tossed)
            return;

        charMouseUpEvent.RaiseEvent();
    }
}
