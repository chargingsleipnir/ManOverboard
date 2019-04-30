using System;
using UnityEditor;
using UnityEngine;

public class RefRect2D : RefShape {

    public float width;
    public float height;

    public override float Width {
        get { return width; }
    }
    public override float Height {
        get { return height; }
    }

    public override Vector2 Position {
        get { return new Vector2(transform.position.x + offsetX, transform.position.y + offsetY); }
    }

    public override bool ContainsPoint(Vector2 point) {
        return point.x > XMin &&
            point.x < XMax &&
            point.y > YMin &&
            point.y < YMax;
    }
    //public override bool Contacts(RefRect2D rect) {
    //    throw new System.NotImplementedException();
    //}
    //public override bool Contacts(RefCircle2D circle) {
    //    throw new System.NotImplementedException();
    //}

    public override float XMin {
        get { return transform.position.x + offsetX - ((width * Math.Abs(transform.lossyScale.x)) * 0.5f); }
    }
    public override float XMax {
        get { return transform.position.x + offsetX + ((width * Math.Abs(transform.lossyScale.x)) * 0.5f); }
    }
    public override float YMin {
        get { return transform.position.y + offsetY - ((height * Math.Abs(transform.lossyScale.y)) * 0.5f); }
    }
    public override float YMax {
        get { return transform.position.y + offsetY + ((height * Math.Abs(transform.lossyScale.y)) * 0.5f); }
    }

    private void OnDrawGizmos() {
        Vector3 pos = transform.position;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(XMin, YMin, pos.z), new Vector3(XMax, YMin, pos.z));
        Gizmos.DrawLine(new Vector3(XMax, YMin, pos.z), new Vector3(XMax, YMax, pos.z));
        Gizmos.DrawLine(new Vector3(XMax, YMax, pos.z), new Vector3(XMin, YMax, pos.z));
        Gizmos.DrawLine(new Vector3(XMin, YMax, pos.z), new Vector3(XMin, YMin, pos.z));
    }

    
}