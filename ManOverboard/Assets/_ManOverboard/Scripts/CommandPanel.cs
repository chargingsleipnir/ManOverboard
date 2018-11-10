using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CommandPanel : SpriteBase {

    [SerializeField]
    private SpriteBase scoopBtnPrefab;

    [SerializeField]
    private SpriteBase donJacketPrefab;

    private int numBtns;
    private float startHeight;

    protected override void Awake() {
        base.Awake();
        startHeight = srRef.comp.size.y;
        gameObject.SetActive(false);
        // or is it srRef.comp.sprite.bounds.size.y, though it's listed as "Height" in the inspector
    }

    public void SetDonLifeJacketBtn() {
        Debug.Log("Added don jacket button");
        numBtns++;
        AddBtnToPanel();        
    }

    public void SetScoopBtn(UnityAction eventCB) {
        Debug.Log("Added scoop water button");
        numBtns++;
        AddBtnToPanel();
        // TODO
        // Instantiate button
        // place appropriately
        // Get MouseTracker Component
        // Add eventCB to tracker - AddMouseUpListener(UnityAction eventCB)    
    }

    private void AddBtnToPanel() {
        srRef.comp.size = new Vector2(srRef.comp.size.x, startHeight * numBtns);
        Debug.Log(srRef.comp.size.y);
    }

    private void DeactivateBtn() {

    }
}
