using UnityEditor;
using UnityEngine;

public class RefRect2D : MonoBehaviour, IRefShape {

    public float offsetX;
    public float offsetY;
    public float width;
    public float height;
    private Rect rect;

    private void Awake() {
        rect = new Rect();

        Vector3 pos = transform.position;
        rect.position = new Vector2(pos.x + (offsetX - (width * 0.5f)), pos.y + (offsetY - (height * 0.5f)));
        rect.width = width;
        rect.height = height;
    }

    public Rect Rect { get; set; }
    public float XMin {
        get { return transform.position.x + offsetX - ((width * transform.lossyScale.x) * 0.5f); }
    }
    public float XMax {
        get { return transform.position.x + offsetX + ((width * transform.lossyScale.x) * 0.5f); }
    }
    public float YMin {
        get { return transform.position.y + offsetY - ((height * transform.lossyScale.y) * 0.5f); }
    }
    public float YMax {
        get { return transform.position.y + offsetY + ((height * transform.lossyScale.y) * 0.5f); }
    }
    public Vector2 Position {
        get { return new Vector2(transform.position.x + offsetX, transform.position.y + offsetY); }
    }

    public bool ContainsPoint(Vector2 point) {
        return point.x > XMin &&
            point.x < XMax &&
            point.y > YMin &&
            point.y < YMax;
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