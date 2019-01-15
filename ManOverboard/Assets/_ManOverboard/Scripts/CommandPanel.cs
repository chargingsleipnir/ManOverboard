using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CommandPanel : SpriteBase {

    [SerializeField]
    private GameObject scoopBtnPrefab;
    [SerializeField]
    private GameObject donJacketBtnPrefab;

    private List<GameObject> initBtns;

    private float singleBtnHeight;

    protected override void Awake() {
        base.Awake();
        singleBtnHeight = srRef.comp.size.y;
        initBtns = new List<GameObject>();
        gameObject.SetActive(false);
    }

    public void PrepBtn(Consts.Skills skill, UnityAction mouseUpCB) {
        // Set appropriate button
        if(skill == Consts.Skills.DonLifeJacketSelf) {
            InstantiateBtnCommon(donJacketBtnPrefab, mouseUpCB);
        }
        else if (skill == Consts.Skills.DonLifeJacketOther) {
            InstantiateBtnCommon(donJacketBtnPrefab, mouseUpCB);
        }
        else if (skill == Consts.Skills.ScoopWater) {
            InstantiateBtnCommon(scoopBtnPrefab, mouseUpCB);
        }
    }

    private void InstantiateBtnCommon(GameObject prefab, UnityAction mouseUpCB) {
        GameObject btn = Instantiate(prefab, transform);
        initBtns.Add(btn);

        RefShape2DMouseTracker tracker = btn.GetComponent<RefShape2DMouseTracker>();
        tracker.AddMouseUpListener(mouseUpCB);
    }

    public void SetBtns() {
        srRef.comp.size = new Vector2(srRef.comp.size.x, singleBtnHeight * initBtns.Count);
        float firstBtnYLocalPos = (srRef.comp.size.y * 0.5f) - (singleBtnHeight * 0.5f);

        for(int i = 0; i < initBtns.Count; i++) {
            initBtns[i].transform.localScale = new Vector3(0.75f, 0.75f, 1.0f);
            initBtns[i].transform.localPosition = new Vector3(0.0f, firstBtnYLocalPos - (i * singleBtnHeight), -0.1f);
        }
    }

    private void DeactivateBtn() {

    }
}
