using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common.Collections;

public class Crewman : CharActionable {

    enum Actions {
        None,
        Scoop
    }
    Actions setAction;

    [SerializeField]
    protected LevelManager levelManager;

    [SerializeField]
    protected Transform trans_ItemUseHand;

    protected override void Start() {
        base.Start();

        setAction = Actions.None;
    }

    public void PrepScoop() {
        levelManager.HighlightToSelect(Consts.ItemType.Scooping);
        setAction = Actions.Scoop;
    }

    public override void UseItem(GameObject item) {
        if(setAction == Actions.Scoop) {
            // Logic for scooping wth item

            Debug.Log("Going to scoop with: " + item.name);
            item.transform.position = trans_ItemUseHand.position;
        }
    }
}
