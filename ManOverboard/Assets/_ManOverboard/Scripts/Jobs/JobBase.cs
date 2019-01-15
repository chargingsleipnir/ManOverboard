using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class JobBase : MonoBehaviour
{
    public abstract void Init(CharBase character);
    public abstract void CheckCanAct(/* Items and other things to check that would determine if an actions can still be perfomed */);
}
