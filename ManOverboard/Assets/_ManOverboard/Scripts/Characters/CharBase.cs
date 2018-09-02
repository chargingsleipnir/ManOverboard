using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;
using ZeroProgress.Common.Collections;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class CharBase : MonoBehaviour {

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

    public delegate void CharGrabDelegate(Vector3 pos, Quaternion rot, Vector2 size, int weight, GameObject obj);
    CharGrabDelegate OnCharHold;

    public delegate void CharReleaseDelegate();
    CharReleaseDelegate OnCharRelease;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private ComponentSetElement setElem;


    public void Awake() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        setElem = GetComponent<ComponentSetElement>();
    }

    public virtual void Start () {
        // Rigidbody is essentially inactive until being flung/thrown by user.
        // (Could use posiiton constraints instead) 
        rb.isKinematic = true;
        Utility.RepositionZ(transform, (float)Consts.ZLayers.BehindBoat);
    }

    public void MoveRigidbody(Vector2 mousePos) {
        rb.MovePosition(mousePos);
    }

    public void AddCharGrabCallback(CharGrabDelegate CB) {
        OnCharHold += CB;
    }
    public void AddCharReleaseCallback(CharReleaseDelegate CB) {
        OnCharRelease += CB;
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

    public virtual void SetActionBtnActive(bool isActive) {}
    public virtual void SetCommandBtnsActive(bool isActive) {}
    public virtual Rect GetActionBtnRect(bool scaledValues) { return new Rect(0, 0, 0, 0); }

    protected virtual void OnMouseDown() {
        if (saved)
            return;
        if (tossed)
            return;

        // Bring character to focus, in front of everything.
        Utility.RepositionZ(transform, (float)Consts.ZLayers.Front);

        // Let level manager take control from here.
        OnCharHold(transform.position, transform.rotation, sr.size, weight, gameObject);
    }

    protected virtual void OnMouseUp() {
        if (saved)
            return;
        if (tossed)
            return;

        // Let level manager take control from here.
        OnCharRelease();
    }
}
