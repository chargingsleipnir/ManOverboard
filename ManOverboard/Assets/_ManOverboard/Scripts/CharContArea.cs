using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CharContArea : MonoBehaviour {

    private SpriteRenderer sr;

    private void Awake() {
        sr = GetComponent<SpriteRenderer>();
    }

    public void CharCollTrue() {
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.66f);
    }

    public void CharCollFalse() {
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.33f);
    }
}
