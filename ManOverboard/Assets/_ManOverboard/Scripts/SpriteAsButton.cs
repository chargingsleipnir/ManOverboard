using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpriteAsButton : MonoBehaviour, IPointerDownHandler {

    private void OnMouseDown() {
        Debug.Log("In SpriteAsButton mouse down callback. Gameobject: " + gameObject.name);
    }

    public void OnPointerDown(PointerEventData data) {
        Debug.Log("In SpriteAsButton pointer down callback. Gameobject: " + gameObject.name);
    }
}
