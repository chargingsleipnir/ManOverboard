using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour {

    [SerializeField]
    private GameObject objToScale;

    private float fill;
    public float Fill {
        get { return fill; }
        set {
            if (value < 0) value = 0;
            else if (value > 1.0f) value = 1.0f;
            fill = value;
            Utility.Scale(objToScale.transform, fill, null, null);
        }
    }

    public bool IsActive {
        get { return gameObject.activeSelf; }
        set { gameObject.SetActive(value); }
    }
}
