using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingBuoy : ItemBase
{
    private bool inWater;
    private Quaternion upQuat;

    protected override void Start() {
        base.Start();

        inWater = false;
        upQuat = Quaternion.LookRotation(Vector3.forward);
    }

    protected override void Update() {
        base.Update();
        
        if(inWater) {
            // TODO: Rotate with a tendancy toward normal
            transform.rotation = Quaternion.RotateTowards(transform.rotation, upQuat, Consts.BUOY_ROT_SPEED * Time.deltaTime);
        }
    }

    public override void Toss(Vector2 vel) {
        base.Toss(vel);

        gameObject.layer = (int)Consts.UnityLayers.FloatDev;
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.layer == (int)Consts.UnityLayers.Water) {
            inWater = true;
        }
    }
}