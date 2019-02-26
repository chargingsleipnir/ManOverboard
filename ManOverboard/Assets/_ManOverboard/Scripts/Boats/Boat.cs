using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ZeroProgress.Common;
using ZeroProgress.Common.Collections;
using Game2DWaterKit;

[System.Serializable]
public struct HoleData {
    [Range(0, 1.0f)]
    public float xByPercent;
    public int yByBuoyancy;    
    public Consts.LeakTypesAndRates leakType;
}

//[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Boat : MonoBehaviour {

    protected bool startRan; // only here so start can be called from level manager because boat start needs to run before character start
    protected LevelManager lvlMngr;
    public LevelManager LvlMngr { set { lvlMngr = value; } }

    [SerializeField]
    [Tooltip("The rigid body of this object")]
    protected Rigidbody2D myRigidbody;

    [SerializeField]
    [Tooltip("The colliders that make up this object")]
    protected Collider2D[] myColliders;

    [SerializeField]
    protected Leak LeakPrefab;

    private RefRect2D SubmergableAreaRef;

    private StringParamEvent levelMsg;

    public float FloorY {
        get { return SubmergableAreaRef.YMax - Consts.BOAT_LEDGE_FLOOR_DIFF; }
    }

    float sinkHeightIncr;

    public GameObject hole;
    [Tooltip("Use values in line with object weight, buoyancy, etc. from the bottom of the boat, upward.")]
    public HoleData[] holeDataSet;

    private List<Hole> holesSubm;
    public int HolesSubmCount { get { return holesSubm.Count; } }
    private List<Hole> holesSurf;
    public List<Hole> Pinholes { get; private set; }

    public int waterStartWeight;
    private ScriptableInt buoyancy;
    private ScriptableInt weightWater;
    private ScriptableInt weightLoad; // Weight of people and items, things that can be tossed off by the player directly.
    private ScriptableInt weightTotal;

    private ScriptableInt numLeaksAbove;
    private ScriptableInt numLeaksBelow;
    private ScriptableInt distLeakAbove;
    private ScriptableInt distLeakBelow;

    private SpriteTossableSet spriteTossableSet;

    // UI update event
    [SerializeField]
    private GameEvent uiUpdate;

    /// <summary>
    /// The game objects that represent leaks in the boat
    /// </summary>
    protected List<Leak> activeLeaks = new List<Leak>();

    protected void Awake()
    {
        buoyancy = Resources.Load<ScriptableInt>("ScriptableObjects/buoyancy");
        weightWater = Resources.Load<ScriptableInt>("ScriptableObjects/weightWater");
        weightLoad = Resources.Load<ScriptableInt>("ScriptableObjects/weightLoad");
        weightTotal = Resources.Load<ScriptableInt>("ScriptableObjects/weightTotal");

        numLeaksAbove = Resources.Load<ScriptableInt>("ScriptableObjects/numLeaksAbove");
        numLeaksBelow = Resources.Load<ScriptableInt>("ScriptableObjects/numLeaksBelow");
        distLeakAbove = Resources.Load<ScriptableInt>("ScriptableObjects/distLeakAbove");
        distLeakBelow = Resources.Load<ScriptableInt>("ScriptableObjects/distLeakBelow");

        spriteTossableSet = Resources.Load<SpriteTossableSet>("ScriptableObjects/SpriteSets/SpriteTossableSet");

        levelMsg = Resources.Load<StringParamEvent>("ScriptableObjects/Events/WithParam/Str_AnyMsg");

        myRigidbody = this.GetComponentIfNull(myRigidbody);
        
        if(myColliders == null || myColliders.Length == 0)
            myColliders = GetComponentsInChildren<Collider2D>();

        SubmergableAreaRef = GetComponent<RefRect2D>();
        holesSubm = new List<Hole>();
        holesSurf = new List<Hole>();
        Pinholes = new List<Hole>();
    }

    public virtual void Start() {} // only here so start can be called from level manager because boat start needs to run before character start

    protected void OnStart(int buoyancyTotal) {

        //SortCompLayerChange(Consts.DrawLayers.BoatLevel1);

        buoyancy.CurrentValue = buoyancyTotal;

        // Default boat, no load, max buoyancy
        weightLoad.CurrentValue = 0;
        weightTotal.CurrentValue = 0;

        distLeakAbove.CurrentValue = 0;
        distLeakBelow.CurrentValue = 0;

        // Add people - increasing load weight & reducing buoyancy
        foreach (SpriteTossable sprite in spriteTossableSet) {
            // Items held/worn by a character have their weight added to the total with the character, so ignore them here.
            if(sprite is ItemBase) {
                if ((sprite as ItemBase).CharHeldBy != null)
                    continue;
            }
            weightLoad.CurrentValue += sprite.Weight;
        }        
        weightTotal.CurrentValue += weightLoad.CurrentValue;

        // Account current water load
        weightWater.CurrentValue = waterStartWeight;
        weightTotal.CurrentValue += weightWater.CurrentValue;

        // Set height of boat based on current buoyancy and water's surface (usually set to 0)
        sinkHeightIncr = SubmergableAreaRef.height / buoyancyTotal;
        AdjustBoatDepth();

        // Put hole(s) in boat
        // Use width of Submergable area reference to determine x position
        float xWidth = SubmergableAreaRef.XMax - SubmergableAreaRef.XMin;
        for (var i = 0; i < holeDataSet.Length; i++) {
            // Calulate proper height
            float yPos = SubmergableAreaRef.YMin + (holeDataSet[i].yByBuoyancy * sinkHeightIncr);

            GameObject holeObj = Instantiate(hole, new Vector3(SubmergableAreaRef.XMin + (holeDataSet[i].xByPercent * xWidth), yPos, transform.position.z - 0.1f), transform.rotation, transform) as GameObject;
            Hole newHole = holeObj.GetComponent<Hole>();
            newHole.Init(lvlMngr, (int)holeDataSet[i].leakType, holeDataSet[i].yByBuoyancy);

            if (yPos <= lvlMngr.WaterSurfaceYPos)
                holesSubm.Add(newHole);
            else
                holesSurf.Add(newHole);

            if (holeDataSet[i].leakType == Consts.LeakTypesAndRates.Pinhole)
                Pinholes.Add(newHole);
        }

        AllLeaksToWaterUIUpdate();
        uiUpdate.RaiseEvent();
    }

    private void AllLeaksToWaterUIUpdate() {

        numLeaksAbove.CurrentValue = holesSurf.Count;
        numLeaksBelow.CurrentValue = holesSubm.Count;

        // "weightTotal.CurrentValue" is perfectly representative of where the water level is relative to the depth the boat can sink by buoyancy/weight.
        if (holesSurf.Count > 0) {
            distLeakAbove.CurrentValue = holesSurf[holesSurf.Count - 1].HeightByBuoyancy - weightTotal.CurrentValue;
        }
        // In this case, not a "Leak" above, but rather, the edge of that given boat level. When this hits zero, the entire floor is lost (game over if it's the last/only floor)
        else {
            levelMsg.RaiseEvent("Level Flood Imminent!");
            distLeakAbove.CurrentValue = buoyancy.CurrentValue - weightTotal.CurrentValue;
        }

        if (holesSubm.Count > 0)
            distLeakBelow.CurrentValue = weightTotal.CurrentValue - holesSubm[holesSubm.Count - 1].HeightByBuoyancy;
        else
            distLeakBelow.CurrentValue = -1;
    }

    private void CheckHolesBoatRaised() {
        // TODO: When data is first read, have the holes organized in these lists, so the closest holes are always at the end of the list, furthest at the beginning.
        for (var i = holesSubm.Count - 1; i > -1; i--) {
            if (holesSubm[i].transform.position.y > lvlMngr.WaterSurfaceYPos) {
                // TODO: Do something with hole.obj reference - change animation to show it's no longer taking in water (change obj reference to a script reference if needed)
                holesSurf.Add(holesSubm[i]);
                holesSubm.RemoveAt(i);
                lvlMngr.CheckLevelEnd();
            }
        }
    }
    private void CheckHolesBoatLowered() {
        for (var i = 0; i < holesSurf.Count; i++) {
            if (holesSurf[i].transform.position.y <= lvlMngr.WaterSurfaceYPos) {
                // TODO: Do something with hole.obj reference - change animation to show it's taking in water (change obj reference to a script reference if needed)
                holesSubm.Add(holesSurf[i]);
                holesSurf.RemoveAt(i);
            }
        }
    }
    protected void AdjustBoatDepth() {
        // Set the boat to where the top of the submergable area is directly on the water's surface, and raise by the buoyancy minus current weight
        float newYPos = lvlMngr.WaterSurfaceYPos - (Mathf.Abs(transform.position.y - SubmergableAreaRef.YMax)) + (sinkHeightIncr * (buoyancy.CurrentValue - weightTotal.CurrentValue));
        transform.position = new Vector3(transform.position.x, newYPos, transform.position.z);
    }

    public virtual void AddWater() {
        if (holesSubm.Count == 0)
            return;

        // Calculate water change value
        int waterWeightChange = 0;
        for (var i = 0; i < holesSubm.Count; i++)
            waterWeightChange += holesSubm[i].LeakRate;

        // Each hole eaqually contributes to water gain / buoyancy loss
        weightWater.CurrentValue += waterWeightChange;
        weightTotal.CurrentValue += waterWeightChange;

        AdjustBoatDepth();
        CheckHolesBoatLowered();

        AllLeaksToWaterUIUpdate();
        uiUpdate.RaiseEvent();
    }

    private void Surface(int weight) {
        weightTotal.CurrentValue -= weight;

        AdjustBoatDepth();
        CheckHolesBoatRaised();

        AllLeaksToWaterUIUpdate();
        uiUpdate.RaiseEvent();
    }
    public void RemoveWater(int weight) {
        weightWater.CurrentValue -= weight;
        Surface(weight);
    }
    public void RemoveLoad(int weight) {
        weightLoad.CurrentValue -= weight;
        Surface(weight);
    }
    public void Repair(Hole hole) {
        hole.Repair();
        Pinholes.Remove(hole);

        foreach(Hole h in holesSubm) {
            if (h == hole) {
                holesSubm.Remove(h);
                AllLeaksToWaterUIUpdate();
                uiUpdate.RaiseEvent();
                lvlMngr.CheckLevelEnd();          
                return;
            }
        }
        foreach(Hole h in holesSurf) {
            if (h == hole) {
                holesSurf.Remove(h);
                AllLeaksToWaterUIUpdate();
                uiUpdate.RaiseEvent();
                return;
            }
        }        
    }

    /// <summary>
    /// Creates a leak randomly in the boat
    /// </summary>
    public void GenerateRandomLeak()
    {
        int randomCollider = Random.Range(0, myColliders.Length);

        Collider2D selectedCollider = myColliders[randomCollider];

        Debug.Log(selectedCollider.bounds.center);
        float randomX = Random.Range(selectedCollider.bounds.min.x, selectedCollider.bounds.max.x);
        float randomY = Random.Range(selectedCollider.bounds.min.y, selectedCollider.bounds.max.y);

        Vector3 position = new Vector3(randomX, randomY, transform.position.z);
        
        Leak newLeak = Instantiate(LeakPrefab, position, Quaternion.identity);
        newLeak.AttachLeak(myRigidbody);

        activeLeaks.Add(newLeak);
    }
}
