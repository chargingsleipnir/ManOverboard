using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClingableSurface : MonoBehaviour
{
    [SerializeField]
    private List<Transform> clingPoints;

    private RefShape shape;

    private void Awake() {
        shape = GetComponent<RefShape>();
    }

    public bool Cling(CharBase character) {
        if (clingPoints.Count < 1)
            return false;

        // TODO: Determine which of the available points the character is closest to, and use/remove that one.
        character.transform.position = clingPoints[0].position;
        clingPoints.RemoveAt(0);
        return true;
    }
}
