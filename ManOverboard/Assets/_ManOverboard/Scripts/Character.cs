using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;
using ZeroProgress.Common.Collections;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Character : MonoBehaviour {

    protected bool grabbed = false;
    protected bool tossed = false;

    // TODO: Char weights held in "StraightValue", while "Value" is kept at zero while no character being clicked? This is extremely counter-intuitive - change immediately.
    // Need to move input system into LevelManager anyway.
    public IntReference weight;
    public int Weight {
        get { return weight.StraightValue; }
    }

    public delegate void CharGrabDelegate();
    CharGrabDelegate OnCharGrab;

    public delegate void CharReleaseDelegate(bool charTossed, int charWeight);
    CharReleaseDelegate OnCharRelease;

    private Vector3 grabPos;
    private Rigidbody2D rb;
    private GameObjectSetElement setElem;

    public GameObject charContAreaPrefab;
    private GameObject charContArea;
    private SpriteRenderer charContAreaSR;
    private BoxCollider2D charContAreaBC;

    public void Awake() {
        rb = this.GetComponent<Rigidbody2D>();
        setElem = this.GetComponent<GameObjectSetElement>();
    }

    public void Start () {
        // Rigidbody is essentially inactive until being flung/thrown by user.
        // (Could use posiiton constraints instead) 
        rb.isKinematic = true;
        weight.Value = 0;
    }

    public void Update () {
        if (grabbed) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));

            rb.MovePosition(new Vector2(mousePos.x, mousePos.y));
            //transform.position.Set(mousePos.x, mousePos.y, transform.position.z);
        }
    }

    public void AddCharGrabCallback(CharGrabDelegate CB) {
        OnCharGrab += CB;
    }
    public void AddCharReleaseCallback(CharReleaseDelegate CB) {
        OnCharRelease += CB;
    }

    private void OnMouseDown() {
        grabbed = true;
        weight.Value = weight.StraightValue;

        grabPos = transform.position;

        charContArea = Instantiate(charContAreaPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.1f), transform.rotation) as GameObject;
        charContAreaSR = charContArea.GetComponent<SpriteRenderer>();
        charContAreaBC = charContArea.GetComponent<BoxCollider2D>();

        // charContArea prefab inherently set to size 1/1 and scale 1/1, so no calc needed.
        Vector2 thisSize = this.GetComponent<SpriteRenderer>().size;
        charContArea.transform.localScale = new Vector3(thisSize.x, thisSize.y, 1);

        // Pause the boat's sinking
        OnCharGrab();
    }

    private void OnMouseUp() {
        grabbed = false;
        weight.Value = 0;

        // Reduce load weight of boat, if appropriate to do so.
        OnCharRelease(tossed, weight.StraightValue);

        if (tossed) {
            // Move in front of all other objects
            transform.position.Set(transform.position.x, transform.position.y, transform.position.z - 0.5f);
            setElem.UnregisterGameObject();
        }
        else {
            transform.position = grabPos;
            rb.isKinematic = true;
            //rb.MovePosition(new Vector2(0, 0));
        }

        Destroy(charContArea);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (grabbed) {
            if (collision == charContAreaBC) {
                tossed = false;
                charContAreaSR.color = new Color(charContAreaSR.color.r, charContAreaSR.color.g, charContAreaSR.color.b, 0.66f);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (grabbed) {
            if (collision == charContAreaBC) {
                tossed = true;
                charContAreaSR.color = new Color(charContAreaSR.color.r, charContAreaSR.color.g, charContAreaSR.color.b, 0.33f);
            }
        }
    }
}
