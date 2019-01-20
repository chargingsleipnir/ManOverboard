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
    private Dictionary<Consts.Skills, GameObject> skillToBtnDict;

    private float singleBtnHeight;

    protected override void Awake() {
        InactiveAwake();
    }
    // ** Bullshit required because Unity is such a load of garbage. **
    public override void InactiveAwake() {
        if (awakeRan)
            return;

        base.InactiveAwake();
        singleBtnHeight = srRef.comp.size.y;
        initBtns = new List<GameObject>();
        skillToBtnDict = new Dictionary<Consts.Skills, GameObject>();
    }

    public void PrepBtn(Consts.Skills skill, UnityAction mouseUpCB) {
        // Set appropriate button
        if(skill == Consts.Skills.DonLifeJacketSelf) {
            InstantiateBtnCommon(donJacketBtnPrefab, skill, mouseUpCB);
        }
        else if (skill == Consts.Skills.DonLifeJacketOther) {
            InstantiateBtnCommon(donJacketBtnPrefab, skill, mouseUpCB);
        }
        else if (skill == Consts.Skills.ScoopWater) {
            InstantiateBtnCommon(scoopBtnPrefab, skill, mouseUpCB);
        }
    }

    private void InstantiateBtnCommon(GameObject prefab, Consts.Skills skill, UnityAction mouseUpCB) {
        GameObject btn = Instantiate(prefab, transform);
        btn.GetComponent<SpriteBase>().InactiveAwake();
        initBtns.Add(btn);
        skillToBtnDict.Add(skill, btn);

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

    // Temporary "disabling". Make it non-responsive as well.
    public void EnableBtn(Consts.Skills skill) {
        SpriteBase btnSprite = skillToBtnDict[skill].GetComponent<SpriteBase>();
        btnSprite.ChangeColour(null, null, null, 1.0f);

        skillToBtnDict[skill].GetComponent<RefShape2DMouseTracker>().enabled = true;
    }
    public void DisableBtn(Consts.Skills skill) {
        SpriteBase btnSprite = skillToBtnDict[skill].GetComponent<SpriteBase>();
        btnSprite.ChangeColour(null, null, null, Consts.BTN_DISABLE_FADE);

        skillToBtnDict[skill].GetComponent<RefShape2DMouseTracker>().enabled = false;
    }
}
