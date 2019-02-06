using UnityEngine;

[ExecuteInEditMode]
public class SpriteOutline : MonoBehaviour {
    public Color color = Color.white;

    //[Range(0, 16)]
    //public int outlineSize = 1;

    public int OutlineSize { get; set; }

    private SpriteRenderer spriteRenderer;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.material = Resources.Load<Material>("Materials/SpriteOutline");
        OutlineSize = 1;
    }

    void OnEnable() {
        UpdateOutline(true);
    }

    void OnDisable() {
        UpdateOutline(false);
    }

    void Update() {
        UpdateOutline(true);
    }

    void UpdateOutline(bool outline) {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat("_Outline", outline ? 1f : 0);
        mpb.SetColor("_OutlineColor", color);
        mpb.SetFloat("_OutlineSize", OutlineSize);
        spriteRenderer.SetPropertyBlock(mpb);
    }

    public void ChangeColour(float? r, float? g, float? b, float? a) {
        color = new Color(r ?? color.r, g ?? color.g, b ?? color.b, a ?? color.a);
    }
}