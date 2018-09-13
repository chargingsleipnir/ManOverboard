using UnityEngine;

public abstract class RefShape : MonoBehaviour {
    public float offsetX;
    public float offsetY;
    public abstract Vector2 Position { get; }
    public abstract bool ContainsPoint(Vector2 point);
}
