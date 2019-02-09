using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

    public void LayerChange(Consts.DrawLayers newLayer, bool resetGroupParent) {
        GroupParent.RemoveFromGroupList(this);

        if(resetGroupParent)
            SetGroupParent();

        DrawLayerCurr = newLayer;
        CheckChangeZDepth();

        GroupParent.AddToGroupList(this);
    }
    public void LayerReset() {
        LayerChange(DrawLayerOrig, true);
    }
    public void ResetGroupParent() {
        GroupParent.RemoveFromGroupList(this);
        SetGroupParent();
        CheckChangeZDepth();
        GroupParent.AddToGroupList(this);
    }

    public abstract string GetDebugString();

    protected void CheckChangeZDepth() {
        Consts.DrawLayers layerToCheck = GetTopLayerInLineage();

        // Adjust z depth based around water, which is always at z = 0, and is not a sprite
        if (layerToCheck >= Consts.DrawLayers.Water)
            Utility.RepositionZ(sprite.gameObject.transform, (float)Consts.ZLayers.FrontOfWater);
        else
            Utility.RepositionZ(sprite.gameObject.transform, (float)Consts.ZLayers.BehindWater);
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

    public virtual Consts.DrawLayers GetTopLayerInLineage() {
        Consts.DrawLayers retLayer = DrawLayerCurr;

        Transform transParent = sprite.transform.parent;
        while (transParent != null) {
            if (transParent.GetComponent<SortingGroup>() != null) {
                retLayer = transParent.GetComponent<SpriteBase>().SGRef.DrawLayerCurr;
            }
            transParent = transParent.parent;
        }

        return retLayer;
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

    public override Consts.DrawLayers GetTopLayerInLineage() {
        Consts.DrawLayers retLayer = DrawLayerCurr;

        if (sprite.SGRef != null)
            retLayer = sprite.SGRef.DrawLayerCurr;

        Transform transParent = sprite.transform.parent;
        while (transParent != null) {
            if (transParent.GetComponent<SortingGroup>() != null) {
                retLayer = transParent.GetComponent<SpriteBase>().SGRef.DrawLayerCurr;
            }
            transParent = transParent.parent;
        }

        return retLayer;
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

    // TODO: Build this up to take multiple ref shapes per sprite. Give them assignable names and/or indices
    public RefShape RefShape { get; private set; }

    private Transform origParent;
    private Transform baseParent;
    private Transform currParent;

    private Vector3 origPos;

    private SpriteOutline so;
    protected bool selectable;

    protected bool awakeRan = false;
    protected virtual void Awake() {
        InactiveAwake();
    }
    // ** Bullshit required because Unity is such a load of garbage. **
    public virtual void InactiveAwake() {
        if (awakeRan)
            return;

        sgRef = null;
        if (GetComponent<SortingGroup>() != null)
            sgRef = new SortingGroupComponentRef(this, GetComponent<SortingGroup>());

        srRef = new SpriteRendererComponentRef(this, GetComponent<SpriteRenderer>(), sgRef);

        RefShape = GetComponent<RefShape>();

        // Get whatever parent sprite group data is available
        origParent = baseParent = currParent = transform.parent;

        origPos = transform.position;

        selectable = false;

        awakeRan = true;
    }

    protected virtual void Start() {
        Reset();
    }

    protected virtual void Reset() {
        EstablishPlacement();
    }

    // TODO: Cache trackers if I start using them more than this.
    public void ChangeMouseUpToDownLinks(bool linkEvents) {
        RefShape2DMouseTracker[] trackers = GetComponents<RefShape2DMouseTracker>();
        for (int i = 0; i < trackers.Length; i++)
            trackers[i].LinkMouseUpToDown = linkEvents;
    }
    public void MouseUpToDownLinksTrue() {
        RefShape2DMouseTracker[] trackers = GetComponents<RefShape2DMouseTracker>();
        for (int i = 0; i < trackers.Length; i++)
            trackers[i].LinkMouseUpToDown = true;
    }
    public void MouseUpToDownLinksFalse() {
        RefShape2DMouseTracker[] trackers = GetComponents<RefShape2DMouseTracker>();
        for (int i = 0; i < trackers.Length; i++)
            trackers[i].LinkMouseUpToDown = false;
    }
    public void ResetMouseUpToDownLinks() {
        RefShape2DMouseTracker[] trackers = GetComponents<RefShape2DMouseTracker>();
        for (int i = 0; i < trackers.Length; i++)
            trackers[i].SetOrigLinkState();
    }

    public void ChangeColour(float? r, float? g, float? b, float? a) {
        srRef.comp.color = new Color(r ?? srRef.comp.color.r, g ?? srRef.comp.color.g, b ?? srRef.comp.color.b, a ?? srRef.comp.color.a);
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
            currParent = transform.parent;
            LayerSortComp.ResetGroupParent();
            SendChangeToTracker();
        }            
    }

    public void SortCompLayerReset() {
        LayerSortComp.LayerReset();
        SendChangeToTracker();
    }
    public void SortCompLayerReset(Transform newParent) {
        currParent = transform.parent = newParent;
        LayerSortComp.LayerReset();
        SendChangeToTracker();
    }
    public void SortCompResetToOriginal() {
        currParent = transform.parent = origParent;
        LayerSortComp.LayerReset();
        SendChangeToTracker();
    }
    public void SortCompResetToBase() {
        currParent = transform.parent = baseParent;
        LayerSortComp.LayerReset();
        SendChangeToTracker();
    }
    public void SortCompResetToBase(Transform newBaseParent) {
        currParent = baseParent = transform.parent = newBaseParent;
        LayerSortComp.LayerReset();
        SendChangeToTracker();
    }
    public void SortCompLayerChange(Consts.DrawLayers newLayer) {
        LayerSortComp.LayerChange(newLayer, false);
        SendChangeToTracker();
    }
    public void SortCompLayerChange(Consts.DrawLayers newLayer, Transform newParent) {
        currParent = transform.parent = newParent;
        LayerSortComp.LayerChange(newLayer, true);
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

    public void EnableMouseTracking(bool isEnabled) {
        RefShape2DMouseTracker[] trackers = GetComponents<RefShape2DMouseTracker>();
        for (int i = 0; i < trackers.Length; i++)
            trackers[i].enabled = isEnabled;
    }

    public void AddHighlightComponent(bool enableComponent = false) {
        so = GetComponent<SpriteOutline>();
        if (so == null) {
            so = gameObject.AddComponent<SpriteOutline>();
            so.enabled = false;
        }

        so.ChangeColour(0, 1.0f, 0.18f, 1.0f);
        if (enableComponent)
            HighlightToSelect();
    }
    protected void RemoveHighlightComponent() {
        if (so != null)
            Destroy(so);
    }
    public virtual void HighlightToSelect() {
        if (so == null)
            return;

        if (so.enabled)
            return;

        ChangeMouseUpToDownLinks(false);
        SortCompLayerChange(Consts.DrawLayers.FrontOfLevel4, null);
        so.enabled = true;
        selectable = true;
    }

    public void UnHighlight() {
        if (so == null)
            return;

        if (!so.enabled)
            return;

        // If sprite tossable overrides UnHighlight(), use ChangeMouseUpToDownLinks(true); instead of ResetMouseUpToDownLinks();
        ResetMouseUpToDownLinks();
        SortCompResetToBase();
        so.enabled = false;
        selectable = false;
    }

    private void OnDestroy() {
        if (GetComponent<RefShape2DMouseTracker>() != null)
            GetComponent<RefShape2DMouseTracker>().RemoveFromSet();

        // If sgRef is here, there's no point removing the srRef from it.
        if(LayerSortComp.GroupParent != null)
            LayerSortComp.GroupParent.RemoveFromGroupList(LayerSortComp);
    }
}
