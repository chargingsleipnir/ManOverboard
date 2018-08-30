using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;
using ZeroProgress.Common.Collections;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Character : MonoBehaviour {

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
    CharGrabDelegate OnCharGrab;

    public delegate void CharReleaseDelegate();
    CharReleaseDelegate OnCharRelease;

    private Rigidbody2D rb;
    private ComponentSetElement setElem;

    public void Awake() {
        rb = GetComponent<Rigidbody2D>();
        setElem = GetComponent<ComponentSetElement>();
    }

    public virtual void Start () {
        // Rigidbody is essentially inactive until being flung/thrown by user.
        // (Could use posiiton constraints instead) 
        rb.isKinematic = true;
    }

    public void AddCharGrabCallback(CharGrabDelegate CB) {
        OnCharGrab += CB;
    }
    public void AddCharReleaseCallback(CharReleaseDelegate CB) {
        OnCharRelease += CB;
    }

    public void Toss(Vector2 vel) {
        tossed = true;
        setElem.UnregisterComponent();
        rb.isKinematic = false;
        rb.velocity = vel;
    }

    protected virtual void OnMouseDown() {
        if (saved)
            return;
        if (tossed)
            return;

        // Bring character to focus, in front of everything.
        transform.Translate(0, 0, -1.2f);

        // Let level manager take control from here.
        OnCharGrab(transform.position, transform.rotation, GetComponent<SpriteRenderer>().size, weight, gameObject);
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
