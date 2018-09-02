using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility {
    public static Vector3 WorldToUISpace(Canvas uiCanvas, Vector3 worldPos) {
        //Convert the world for screen point so that it can be used with ScreenPointToLocalPointInRectangle function
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        Vector2 movePos;

        //Convert the screenpoint to ui rectangle local point
        RectTransformUtility.ScreenPointToLocalPointInRectangle(uiCanvas.transform as RectTransform, screenPos, uiCanvas.worldCamera, out movePos);
        //Convert the local point to world point
        return uiCanvas.transform.TransformPoint(movePos);
    }

    public static Vector3 UIToWorldSpace(Canvas uiCanvas, Vector3 uiPos) {
        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(uiCanvas.worldCamera, uiPos);
        return Camera.main.ScreenToWorldPoint(screenPos);
    }

    public static float GreaterOf(float num1, float num2) {
        return num1 > num2 ? num1 : num2;
    }

    public static void RepositionX(Transform transform, float newX) {
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }
    public static void RepositionY(Transform transform, float newY) {
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
    public static void RepositionZ(Transform transform, float newZ) {
        transform.position = new Vector3(transform.position.x, transform.position.y, newZ);
    }
}
