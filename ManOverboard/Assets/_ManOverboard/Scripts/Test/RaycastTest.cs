using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastTest : MonoBehaviour {

    Camera cam = Camera.main;
    Vector2 mousePos;
    RaycastHit2D[] hits;

    void Update () {
        if (Input.GetMouseButtonDown(0)) {
            
            mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log(mousePos);

            //RaycastHit2D hit = Physics2D.Raycast(mousePos, cam.transform.forward);
            //Debug.Log(hit.transform.gameObject.name);
            
            // * As suspected, objects must have colliders and are ordered by z-depth only
            hits = Physics2D.RaycastAll(mousePos, cam.transform.forward, 100.0f);
            for (int i = 0; i < hits.Length; i++) {
                Debug.Log(hits[i].transform.gameObject.name);
            }
        }
    }
}
