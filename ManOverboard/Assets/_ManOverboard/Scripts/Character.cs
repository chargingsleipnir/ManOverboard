using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;
using ZeroProgress.Common.Collections;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Character : MonoBehaviour {

    const float TOSSED_CHAR_DEPTH_STEP = 0.5f;

    protected bool grabbed = false;
    protected bool prepToss = false;
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

    public delegate void CharGrabDelegate(Vector3 pos, Quaternion rot, Vector2 size, int weight, Character scpt);
    CharGrabDelegate OnCharGrab;

    public delegate void CharReleaseDelegate(bool charTossed);
    CharReleaseDelegate OnCharRelease;

    private Vector3 grabPos;
    private Vector2 mouseLastPos = Vector2.zero;
    private Vector2 mouseCurrPos = Vector2.zero;
    private Vector2 mouseDelta = Vector2.zero;
    private Rigidbody2D rb;
    private GameObjectSetElement setElem;

    private BoxCollider2D charContAreaBC;

    public void Awake() {
        rb = this.GetComponent<Rigidbody2D>();
        setElem = this.GetComponent<GameObjectSetElement>();
    }

    public void Start () {
        // Rigidbody is essentially inactive until being flung/thrown by user.
        // (Could use posiiton constraints instead) 
        rb.isKinematic = true;
    }

    public void Update () {
        if (!saved) {
            if (grabbed) {
                mouseLastPos = mouseCurrPos;

                Vector3 mouseCalcPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
                mouseCurrPos.Set(mouseCalcPos.x, mouseCalcPos.y);

                mouseDelta = mouseCurrPos - mouseLastPos;

                rb.MovePosition(mouseCurrPos);
                //transform.position.Set(mousePos.x, mousePos.y, transform.position.z);
            }
        }
    }

    public void AddCharGrabCallback(CharGrabDelegate CB) {
        OnCharGrab += CB;
    }
    public void AddCharReleaseCallback(CharReleaseDelegate CB) {
        OnCharRelease += CB;
    }

    private void OnMouseDown() {
        if (!saved) {
            if (!tossed) {
                grabbed = true;
                grabPos = transform.position;
                OnCharGrab(transform.position, transform.rotation, this.GetComponent<SpriteRenderer>().size, weight, this);
            }
        }
    }

    private void OnMouseUp() {
        if (!saved) {
            if (!tossed) {
                grabbed = false;

                if (prepToss) {
                    tossed = true;
                    // Move in front of all other non-water objects
                    transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - TOSSED_CHAR_DEPTH_STEP);
                    gameObject.layer = 9; // tossed objects

                    setElem.UnregisterGameObject();

                    rb.isKinematic = false;

                    // TODO: Top out the toss speed to something not TOO unreasonable
                    float tossSpeed = mouseDelta.magnitude / Time.deltaTime;
                    rb.velocity = mouseDelta * tossSpeed;
                }
                else {
                    transform.position = grabPos;
                    rb.isKinematic = true;
                }

                // Reduce load weight of boat, if appropriate to do so.
                OnCharRelease(tossed);
            }
        }
    }

    public void GetCharContAreaBoxCollider(BoxCollider2D bc) {
        charContAreaBC = bc;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!saved) {
            if (grabbed) {
                if (collision == charContAreaBC) {
                    prepToss = false;
                    collision.gameObject.GetComponent<CharContArea>().CharCollTrue();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (!saved) {
            if (grabbed) {
                if (collision == charContAreaBC) {
                    prepToss = true;
                    collision.gameObject.GetComponent<CharContArea>().CharCollFalse();
                }
            }
        }
    }
}
