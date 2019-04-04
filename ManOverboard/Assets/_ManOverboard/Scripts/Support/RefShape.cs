using UnityEngine;

public abstract class RefShape : MonoBehaviour {
    public float offsetX;
    public float offsetY;
    public abstract float Width { get; }
    public abstract float Height { get; }
    public abstract Vector2 Position { get; }
    public abstract bool ContainsPoint(Vector2 point);
    //public abstract bool Contacts(RefRect2D rect);
    //public abstract bool Contacts(RefCircle2D circle);
    public abstract float XMin { get; }
    public abstract float XMax { get; }
    public abstract float YMin { get;}
    public abstract float YMax { get; }
}
