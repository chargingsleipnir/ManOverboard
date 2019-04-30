using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NonColliderMouseHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    public void OnPointerDown(PointerEventData eventData) {
        Debug.Log("Pointer down on: " + name);
    }

    public void OnPointerUp(PointerEventData eventData) {
        Debug.Log("Pointer up on: " + name);
    }
}
