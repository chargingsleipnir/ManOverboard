using UnityEngine;

public class SpriteTossable : SpriteBase {

    protected bool held = false;
    protected bool tossed = false;

    protected Rigidbody2D rb;
    protected BoxCollider2D bc;

    [SerializeField]
    protected int weight;
    public int Weight {
        get { return weight; }
    }

    protected override void Awake() {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
    }
}
