using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class JobBase : MonoBehaviour
{
    protected CharBase character;

    public abstract void Init(CharBase character);

    // TODO: Pass in lvlMngr and commandPanel?
    public abstract void SetActionBtns();
    public abstract void CheckCanAct();
    public abstract void OnSelectionItem(ItemBase item);
    public abstract void ReleaseItemHeld(ItemBase item);
    public abstract void ItemLost(ItemBase item);
}
