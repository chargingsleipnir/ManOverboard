using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class SpriteLayerComponentData {
    protected Consts.DrawLayers DrawLayerOrig { get; private set; }
    protected Consts.DrawLayers drawLayerCurr;
    public abstract Consts.DrawLayers DrawLayerCurr { get; set; }
    public abstract int OrderInLayer { get; set; }

    public SpriteBase sprite;

    public SortingGroupComponentRef GroupParent { get; protected set; }

    public SpriteLayerComponentData() {
        drawLayerCurr = Consts.DrawLayers.Default;
    }

    public SpriteLayerComponentData(SpriteBase sprite, Consts.DrawLayers drawLayerOrig) {
        DrawLayerOrig = drawLayerCurr = drawLayerOrig;
        this.sprite = sprite;
    }

    ///<summary>
    ///Resets to original layer
    ///</summary>
    public void ChangeLayer() {
        ChangeLayer(DrawLayerOrig, false);
    }
    public void ChangeLayer(Consts.DrawLayers newLayer, bool resetGroupParent) {
        CheckChangeZDepth(newLayer);

        GroupParent.RemoveFromGroupList(this);

        DrawLayerCurr = newLayer;
        if(resetGroupParent)
            SetGroupParent();

        GroupParent.AddToGroupList(this);
    }

    public abstract string GetDebugString();

    protected void CheckChangeZDepth(Consts.DrawLayers newLayer) {
        // Applies to only DrawLayerMngr.topSortGroup
        if (GroupParent == DrawLayerMngr.topSortGroup) {
            // Adjust z depth based around water, which is always at z = 0, and is not a sprite
            if (newLayer >= Consts.DrawLayers.Water)
                Utility.RepositionZ(sprite.gameObject.transform, (float)Consts.ZLayers.FrontOfWater);
            else
                Utility.RepositionZ(sprite.gameObject.transform, (float)Consts.ZLayers.BehindWater);
        }
    }

    enum QtyCompare { GREATER, EQUAL, LESSER }
    private QtyCompare ARelativeToB(SpriteLayerComponentData layerCompA, SpriteLayerComponentData layerCompB) {
        if (layerCompA.DrawLayerCurr > layerCompB.DrawLayerCurr)
            return QtyCompare.GREATER;
        else if (layerCompA.DrawLayerCurr < layerCompB.DrawLayerCurr)
            return QtyCompare.LESSER;
        else {
            if (layerCompA.OrderInLayer > layerCompB.OrderInLayer)
                return QtyCompare.GREATER;
            else if (layerCompA.OrderInLayer < layerCompB.OrderInLayer)
                return QtyCompare.LESSER;
        }

        return QtyCompare.EQUAL;
    }

    public bool IsCloserThan(SpriteLayerComponentData layerComp) {
        List<SpriteLayerComponentData> sortLineageListA = GetLineageList();
        List<SpriteLayerComponentData> sortLineageListB = layerComp.GetLineageList();

        int incrementsToCheck = Utility.LesserOf(sortLineageListA.Count, sortLineageListB.Count);

        // Starting at 1 because all parent chains start with DrawLayerMngr.topSortGroup, which there's no point in comparing to itself
        for (int i = 1; i < incrementsToCheck; i++) {
            QtyCompare compValue = ARelativeToB(sortLineageListA[i], sortLineageListB[i]);
            if (compValue == QtyCompare.GREATER)
                return true;
            else if (compValue == QtyCompare.LESSER)
                return false;
        }

        return false;
    }

    public virtual void SetGroupParent() {
        GroupParent = null;

        Transform transParent = sprite.transform.parent;
        while (transParent != null) {
            if (transParent.GetComponent<SortingGroup>() != null) {
                GroupParent = transParent.GetComponent<SpriteBase>().SGRef;
                return;
            }
            transParent = transParent.parent;
        }

        if (GroupParent == null)
            GroupParent = DrawLayerMngr.topSortGroup;
    }

    public virtual List<SpriteLayerComponentData> GetLineageList() {
        List<SpriteLayerComponentData> retList = new List<SpriteLayerComponentData> { this };

        Transform transParent = sprite.transform.parent;
        while (transParent != null) {
            if (transParent.GetComponent<SortingGroup>() != null) {
                retList.Add(transParent.GetComponent<SpriteBase>().SGRef);
            }
            transParent = transParent.parent;
        }

        retList.Add(DrawLayerMngr.topSortGroup);
        retList.Reverse();

        return retList;
    }

    public void EstablishRelationships() {
        GroupParent.RemoveFromGroupList(this);
        SetGroupParent();
        GroupParent.AddToGroupList(this);
    }
}

public class SpriteRendererComponentRef : SpriteLayerComponentData {
    public SpriteRenderer comp;
    public override Consts.DrawLayers DrawLayerCurr {
        get { return drawLayerCurr; }
        set {
            drawLayerCurr = value;
            comp.sortingLayerName = value.ToString();
        }
    }

    public override int OrderInLayer {
        get {
            if (comp == null)
                return -1;
            return comp.sortingOrder;
        }
        set { comp.sortingOrder = value; }
    }

    public SpriteRendererComponentRef(SpriteBase sprite, SpriteRenderer comp, SortingGroupComponentRef sgCompRef) : this(sprite, comp, sgCompRef, (Consts.DrawLayers)Enum.Parse(typeof(Consts.DrawLayers), comp.sortingLayerName)) { }
    public SpriteRendererComponentRef(SpriteBase sprite, SpriteRenderer comp, SortingGroupComponentRef sgCompRef, Consts.DrawLayers drawLayerOrig) : base(sprite, drawLayerOrig) {
        this.comp = comp;
        if(sgCompRef != null)
            sgCompRef.AddToGroupList(this);
    }

    public override string GetDebugString() {
        return sprite.name + "\n";
    }

    public override void SetGroupParent() {
        if (sprite.SGRef != null) {
            GroupParent = sprite.SGRef;
            return;
        }

        base.SetGroupParent();
    }

    public override List<SpriteLayerComponentData> GetLineageList() {
        List<SpriteLayerComponentData> retList = new List<SpriteLayerComponentData> { this };

        if (sprite.SGRef != null)
            retList.Add(sprite.SGRef);

        Transform transParent = sprite.transform.parent;
        while (transParent != null) {
            if (transParent.GetComponent<SortingGroup>() != null)
                retList.Add(transParent.GetComponent<SpriteBase>().SGRef);

            transParent = transParent.parent;
        }

        retList.Add(DrawLayerMngr.topSortGroup);
        retList.Reverse();

        return retList;
    }

    public bool IsChildOf(SortingGroupComponentRef sgRef) {
        if (sprite.SGRef == sgRef)
            return true;

        Transform transParent = sprite.transform.parent;
        while (transParent != null) {
            if (transParent.GetComponent<SortingGroup>() != null)
                if (transParent.GetComponent<SpriteBase>().SGRef == sgRef)
                    return true;

            transParent = transParent.parent;
        }

        // No need to check DrawLayerMngr.topSortGroup of course, as it does not exist on screen.
        return false;
    }
}

public class SortingGroupComponentRef : SpriteLayerComponentData {
    public SortingGroup comp;
    public List<SpriteLayerComponentData> groupList = new List<SpriteLayerComponentData>();

    public override Consts.DrawLayers DrawLayerCurr {
        get { return drawLayerCurr; }
        set {
            drawLayerCurr = value;
            comp.sortingLayerName = value.ToString();
        }
    }

    public override int OrderInLayer {
        get {
            if (comp == null)
                return -1;
            return comp.sortingOrder;
        }
        set { comp.sortingOrder = value; }
    }

    public SortingGroupComponentRef() : base() { }
    public SortingGroupComponentRef(SpriteBase sprite, SortingGroup sgComp) : this(sprite, sgComp, (Consts.DrawLayers)Enum.Parse(typeof(Consts.DrawLayers), sgComp.sortingLayerName)) {}
    public SortingGroupComponentRef(SpriteBase sprite, SortingGroup sgComp, Consts.DrawLayers drawLayerOrig) : base(sprite, drawLayerOrig) {
        comp = sgComp;
        comp.enabled = true;
    }

    // TODO: Add a top level SortingGroupComponentRef somewhere to hold the top layer list
    // and continue on from there.

    public void AddToGroupList(SpriteLayerComponentData layerSortComp) {
        for(int i = 0; i < groupList.Count; i++) {
            if (layerSortComp.DrawLayerCurr > groupList[i].DrawLayerCurr) {
                groupList.Insert(i, layerSortComp);
                return;
            }
            else if (layerSortComp.DrawLayerCurr == groupList[i].DrawLayerCurr) {
                if (layerSortComp.OrderInLayer > groupList[i].OrderInLayer) {
                    groupList.Insert(i, layerSortComp);
                    return;
                }
                else if (layerSortComp.OrderInLayer == groupList[i].OrderInLayer) {
                    // Rerun the function from the front again until the increased order number's equal isn't found
                    layerSortComp.OrderInLayer++;
                    AddToGroupList(layerSortComp);
                    return;
                }
            }
        }
        // Will only happen if it's the furthest item in the list
        groupList.Add(layerSortComp);
    }

    public void RemoveFromGroupList(SpriteLayerComponentData layerSortComp) {
        groupList.Remove(layerSortComp);
    }

    public override string GetDebugString() {
        string retString = "";
        for (int i = 0; i < groupList.Count; i++) {
            retString += groupList[i].GetDebugString();
        }
        return retString;
    }
}

public class SpriteBase : MonoBehaviour {

    protected SpriteRendererComponentRef srRef;
    protected SortingGroupComponentRef sgRef;

    public SpriteRendererComponentRef SRRef { get { return srRef; } }
    public SortingGroupComponentRef SGRef { get { return sgRef; } }
    public SpriteLayerComponentData LayerSortComp {
        get {
            if (sgRef == null)
                return srRef;
            return sgRef;
        }
    }

    private bool removedFromGroup = false;
    private Transform origParent;
    private Transform currParent;

    protected virtual void Awake() {
        sgRef = null;
        if (GetComponent<SortingGroup>() != null)
            sgRef = new SortingGroupComponentRef(this, GetComponent<SortingGroup>());

        srRef = new SpriteRendererComponentRef(this, GetComponent<SpriteRenderer>(), sgRef);

        // Get whatever parent sprite group data is available
        origParent = currParent = transform.parent;
    }

    protected virtual void Start() {
        EstablishPlacement();
    }

    public void EstablishPlacement() {
        if (srRef.GroupParent != null)
            return;

        // The srRef only needs to be added to it's next group parent if there is no sorting group on this item, otherwise it's already done when the sgRef is instantiated
        srRef.SetGroupParent();
        if (sgRef != null)
            sgRef.SetGroupParent();

        LayerSortComp.GroupParent.AddToGroupList(LayerSortComp);
        //SendChangeToTracker(); --> Might need to be done here as well, if handling in the Start function of RefShape2DMouseTracker does not work well
    }

    protected virtual void Update() {
        if(currParent != transform.parent) {
            LayerSortComp.EstablishRelationships();
            currParent = transform.parent;
            SendChangeToTracker();
        }            
    }

    ///<summary>
    ///Resets to original layer
    ///</summary>
    public void ChangeSortCompLayer() {
        LayerSortComp.ChangeLayer();
        SendChangeToTracker();
    }
    public void ChangeSortCompLayer(Consts.DrawLayers newLayer) {
        LayerSortComp.ChangeLayer(newLayer, false);
        SendChangeToTracker();
    }
    public void ChangeSortCompLayer(Consts.DrawLayers newLayer, Transform newParent) {
        currParent = transform.parent = newParent;
        LayerSortComp.ChangeLayer(newLayer, true);
        SendChangeToTracker();

        // TODO: Should also have a version that changes the layer order
        // Possibly one that specifies whether to change the sprite renderer or sorting group specifically, but this is fine for now.
    }

    // Using sprite renderers specifically, as they already incorporate whatever sorting group this sprite may have
    // and will be what shows in whatever given order at the end of the day.
    public bool IsCloserThan(SpriteBase sprite) {
        return srRef.IsCloserThan(sprite.srRef);
    }

    public bool IsChildOf(SpriteBase sprite) {
        if (sprite.sgRef == null)
            return false;

        return srRef.IsChildOf(sprite.sgRef);
    }

    private void SendChangeToTracker() {
        if (GetComponent<RefShape2DMouseTracker>() != null)
            GetComponent<RefShape2DMouseTracker>().RepositionInTrackerSet();
    }

    protected void MoveToTopSpriteGroup() {
        removedFromGroup = true;
        transform.parent = null;
        //DrawLayerMngr.AddSprite(this);
    }

    protected void MoveToOrigSpriteGroup() {
        if (!removedFromGroup)
            return;

        removedFromGroup = false;
        transform.parent = origParent;
        //DrawLayerMngr.RemoveSprite(this);
    }

    private void OnDestroy() {
        if (GetComponent<RefShape2DMouseTracker>() != null)
            GetComponent<RefShape2DMouseTracker>().RemoveFromSet();

        // If sgRef is here, there's no point removing the srRef from it.
        if(LayerSortComp.GroupParent != null)
            LayerSortComp.GroupParent.RemoveFromGroupList(LayerSortComp);
    }
}
