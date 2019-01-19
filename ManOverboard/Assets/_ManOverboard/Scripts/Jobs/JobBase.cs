using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class JobBase : MonoBehaviour
{
    public abstract void Init(CharBase character);

    // TODO: Pass in lvlMngr and commandPanel?
    public abstract void SetActionBtns();
    public abstract void CheckCanAct();
}
