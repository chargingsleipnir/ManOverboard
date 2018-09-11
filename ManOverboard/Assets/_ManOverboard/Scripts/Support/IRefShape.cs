using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRefShape {
    Vector2 Position { get; }
    bool ContainsPoint(Vector2 point);
}
