using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefCircle2D : RefShape {

    public float radius;

    public override float Width {
        get { return radius * 2; }
    }
    public override float Height {
        get { return radius * 2; }
    }

    public override Vector2 Position {
        get { return new Vector2(transform.position.x + offsetX, transform.position.y + offsetY); }
    }

    public override bool ContainsPoint(Vector2 point) {
        float magSqr = Vector2.SqrMagnitude(point - new Vector2(transform.position.x + offsetX, transform.position.y + offsetY));
        return magSqr < Mathf.Pow(radius * Utility.GreaterOf(transform.lossyScale.x, transform.lossyScale.y), 2);
    }

    public override float XMin {
        get { return transform.position.x + offsetX - (radius * transform.lossyScale.x); }
    }
    public override float XMax {
        get { return transform.position.x + offsetX + (radius * transform.lossyScale.x); }
    }
    public override float YMin {
        get { return transform.position.y + offsetY - (radius * transform.lossyScale.y); }
    }
    public override float YMax {
        get { return transform.position.y + offsetY + (radius * transform.lossyScale.y); }
    }

    private void OnDrawGizmos() {

        Gizmos.color = Color.red;
        Vector3 oldPos = new Vector3();
        Vector3 newPos = new Vector3();
        Vector3 lastPos = new Vector3();

        float theta = 0.0f;
        float radScaled = radius * Utility.GreaterOf(transform.lossyScale.x, transform.lossyScale.y);
        float x = Mathf.Cos(theta) * radScaled;
        float y = Mathf.Sin(theta) * radScaled;
        lastPos = oldPos = transform.position + new Vector3(offsetX + x, offsetY + y, 0.0f);

        for (theta = 0.3f; theta < Mathf.PI * 2; theta += 0.3f) {
            x = Mathf.Cos(theta) * radScaled;
            y = Mathf.Sin(theta) * radScaled;
            newPos = transform.position + new Vector3(offsetX + x, offsetY + y, 0.0f);
            Gizmos.DrawLine(oldPos, newPos);
            oldPos = newPos;
        }
        Gizmos.DrawLine(oldPos, lastPos);
    }
}
