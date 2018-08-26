using UnityEngine;

public class RefRect2D : MonoBehaviour {

    public float offsetX;
    public float offsetY;
    public float width;
    public float height;

    private Rect rect;

    private void Awake() {
        rect = new Rect();
    }

    public float XMin {
        get { return transform.position.x + offsetX - (width * 0.5f); }
    }
    public float XMax {
        get { return transform.position.x + offsetX + (width * 0.5f); }
    }
    public float YMin {
        get { return transform.position.y + offsetY - (height * 0.5f); }
    }
    public float YMax {
        get { return transform.position.y + offsetY + (height * 0.5f); }
    }

    private void OnDrawGizmos() {
        Vector3 pos = transform.position;

        rect.position = new Vector2(pos.x + (offsetX - (width * 0.5f)), pos.y + (offsetY - (height * 0.5f)));
        rect.width = width;
        rect.height = height;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(rect.xMin, rect.yMin, pos.z), new Vector3(rect.xMax, rect.yMin, pos.z));
        Gizmos.DrawLine(new Vector3(rect.xMax, rect.yMin, pos.z), new Vector3(rect.xMax, rect.yMax, pos.z));
        Gizmos.DrawLine(new Vector3(rect.xMax, rect.yMax, pos.z), new Vector3(rect.xMin, rect.yMax, pos.z));
        Gizmos.DrawLine(new Vector3(rect.xMin, rect.yMax, pos.z), new Vector3(rect.xMin, rect.yMin, pos.z));
    }
}