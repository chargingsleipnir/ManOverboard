using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ItemCanScoop : ItemBase {

    public int capacity;
    private ItemCanScoopSet canScoopSet;

    protected override void Awake() {
        base.Awake();
        canScoopSet = AssetDatabase.LoadAssetAtPath<ItemCanScoopSet>("Assets/_ManOverboard/Variables/Sets/ItemCanScoopSet.asset");
        canScoopSet.Add(this);
    }

    public override void RemoveFromSet() {
        canScoopSet.Remove(this);
        base.RemoveFromSet();
    }
}
