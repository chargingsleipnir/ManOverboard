using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;
using ZeroProgress.Common.Collections;


/* TODO: Get rid of CharActionable - all characters will be potentially actionable at some point in some way. I'm better off to check what the scene is like
 * (what items are available, etc) and adjust each characters settings for that level at that point (like whether or not the action button will appear)
 * 
 * Have children be able to don their own life jackets at about 30 seconds, or click on any adult and change the amount of time. 5 secs for crewman, 10 secs for passenger, etc.
 * The click to have an adult do it should come from the adult themselves:
 * Click adult,
 * Click life jacket button, all life jackets highlight.
 * If adult jacket is clicked, it's donned. 
 * Maybe Clicking the life jacket button again will allow your own to be clicked to remove. Maybe a second hold & release button needs to be available for quicker doffing
 * If child jacket is clicked, kids are highlighted to put in on.
 * 
 * Show a meter at one side that’s like a close-up of the water level reaching the next hole or boat-level-ledge, as this is very hard to see as is, 
 * and once the water goes over, that level is flooded/lost, whatever exactly that means right now.
 * 
 * Need a better way to see items/characters:
Translucent masking for items behind boat wall (bucket, life jacket, characters legs?) 

OR

Instead of masking, make is more general, and design the entire boat to be translucent anyway.

 */


[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class CharBase : SpriteTossable, IMouseDownDetector, IMouseUpDetector {

    protected Consts.CharState state;

    public delegate void DelPassItemType(Consts.ItemType type);
    public delegate void DelPassInt(int waterWeight);

    protected List<ItemBase> heldItems;
    protected int heldItemWeight;
    public override int Weight {
        get { return weight + heldItemWeight; }
    }

    protected bool saved = false;
    public bool Saved {
        get {
            return saved;
        }
        set {
            saved = value;
            if (value) {
                CancelAction();
            }
        }
    }

    public bool Paused { get; set; }

    public virtual bool IsActionBtnActive { get { return false; } set { } }
    public virtual bool IsCancelBtnActive { get { return false; } set { } }
    public virtual bool IsCommandPanelOpen { get { return false; } set { } }

    protected override void Start () {
        base.Start();

        state = Consts.CharState.Default;
        heldItems = new List<ItemBase>();
        heldItemWeight = 0;

        SortCompLayerChange(Consts.DrawLayers.BoatLevel1Contents);
    }

    public override void MouseDownCB() {
        if (saved || tossed)
            return;

        held = true;
        OnMouseDownCB(gameObject);
    }
    public override void MouseUpCB() {
        if (saved || tossed || held == false)
            return;

        held = false;
        OnMouseUpCB();
    }

    public void LoseItem(ItemBase item) {
        if (heldItems.Remove(item))
            heldItemWeight -= item.Weight;
    }

    public virtual void CancelAction() { }
    public virtual void UseItem(ItemBase item) { }    
    public virtual void SetCallbacks(DelPassItemType setTypeCB, DelPassInt startCoCB, DelVoid fadeLevelCB, DelVoid unfadeLevelCB) { }
}
