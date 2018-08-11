using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Passenger : MonoBehaviour {

    public int weight = 100;
    private bool grabbed = false;
    private bool tossed = false;

    private Vector3 grabPos;
    private Rigidbody2D rb;

    public UnityEngine.UI.Text weightDisp;

    public GameObject charContAreaPrefab;
    private GameObject charContArea;
    private SpriteRenderer charContAreaSR;
    private BoxCollider2D charContAreaBC;

    void Start() {
        rb = this.GetComponent<Rigidbody2D>();

        // Rigidbody is essentially inactive until being flung/thrown by user.
        // (Could use posiiton constraints instead)
        rb.isKinematic = true;
        weightDisp.text = "";
    }

    // TODO
    void Update() {
        if (grabbed) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));

            rb.MovePosition(new Vector2(mousePos.x, mousePos.y));
            //transform.position.Set(mousePos.x, mousePos.y, transform.position.z);
        }
    }

    private void OnMouseDown() {
        grabbed = true;
        weightDisp.text = weight.ToString();

        grabPos = transform.position;

        charContArea = Instantiate(charContAreaPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.1f), transform.rotation) as GameObject;
        charContAreaSR = charContArea.GetComponent<SpriteRenderer>();
        charContAreaBC = charContArea.GetComponent<BoxCollider2D>();

        // charContArea prefab inherently set to size 1/1 and scale 1/1, so no calc needed.
        Vector2 thisSize = this.GetComponent<SpriteRenderer>().size;
        charContArea.transform.localScale = new Vector3(thisSize.x, thisSize.y, 1);
    }

    private void OnMouseUp() {
        grabbed = false;
        weightDisp.text = "";

        if(tossed) {

        }
        else {
            transform.position = grabPos;
            rb.isKinematic = true;
            //rb.MovePosition(new Vector2(0, 0));
        }
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
