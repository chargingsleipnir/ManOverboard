using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ZeroProgress.Common;
using ZeroProgress.Common.Collections;
using Game2DWaterKit;

public class Hole {
    public GameObject obj;
    public int leakRate;   

    public Hole(GameObject obj, int leakRate) {
        this.obj = obj;
        this.leakRate = leakRate;        
    }
}

[System.Serializable]
public struct HoleData {
    public int heightByBuoyancy;
    public Consts.LeakTypesAndRates leakType;
}

//[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Boat : SpriteBase {

    public delegate void NumLeaksDelegate(int numLeaks);
    NumLeaksDelegate NumLeaksCB;

    [SerializeField]
    [Tooltip("The rigid body of this object")]
    protected Rigidbody2D myRigidbody;

    [SerializeField]
    [Tooltip("The colliders that make up this object")]
    protected Collider2D[] myColliders;

    [SerializeField]
    protected Leak LeakPrefab;

    public GameObject water;
    private float waterSurfaceYPos;
    private RefRect2D SubmergableAreaRef;

    public float FloorY {
        get { return SubmergableAreaRef.YMax - Consts.BOAT_LEDGE_FLOOR_DIFF; }
    }

    float sinkHeightIncr;

    public GameObject hole;
    [Tooltip("Use values in line with object weight, buoyancy, etc. from the bottom of the boat, upward.")]
    public HoleData[] holeDataSet;
    private List<Hole> holesSubm;
    private List<Hole> holesSurf;

    public IntReference buoyancy;
    public IntReference weightWater;
    public IntReference weightLoad; // Weight of people and items, things that can be tossed off by the player directly.
    public IntReference weightTotal;
    public int WeightTotal {
        get { return weightTotal.Value; }
    }
    private SpriteTossableSet spriteTossableSet;

    // UI update event
    [SerializeField]
    private GameEvent uiUpdate;

    /// <summary>
    /// The game objects that represent leaks in the boat
    /// </summary>
    protected List<Leak> activeLeaks = new List<Leak>();

    protected override void Awake()
    {
        base.Awake();

        spriteTossableSet = AssetDatabase.LoadAssetAtPath<SpriteTossableSet>("Assets/_ManOverboard/Variables/Sets/SpriteTossableSet.asset");

        myRigidbody = this.GetComponentIfNull(myRigidbody);
        
        if(myColliders == null || myColliders.Length == 0)
            myColliders = GetComponentsInChildren<Collider2D>();

        SubmergableAreaRef = GetComponent<RefRect2D>();
        waterSurfaceYPos = (water.transform.position.y + (water.GetComponent<Game2DWater>().WaterSize.y * 0.5f));
        holesSubm = new List<Hole>();
        holesSurf = new List<Hole>();
    }

    protected void OnStart(int buoyancyTotal) {

        SortCompLayerChange(Consts.DrawLayers.BoatLevel1);

        buoyancy.Value = buoyancyTotal;

        // Default boat, no load, max buoyancy
        weightLoad.Value = 0;
        weightTotal.Value = 0;

        // Add people - increasing load weight & reducing buoyancy
        foreach (SpriteTossable sprite in spriteTossableSet) {
            weightLoad.Value += sprite.Weight;
        }
        weightTotal.Value += weightLoad.Value;
        // TODO: Add ITEMS - increasing load weight & reducing buoyancy

        // Account current water load
        weightWater.Value = weightWater.StraightValue;
        weightTotal.Value += weightWater.Value;

        uiUpdate.RaiseEvent();

        // Set height of boat based on current buoyancy and water's surface (usually set to 0)
        sinkHeightIncr = SubmergableAreaRef.height / buoyancyTotal;
        AdjustBoatDepth();

        // Put hole(s) in boat
        for (var i = 0; i < holeDataSet.Length; i++) {
            // Calulate proper height
            float yPos = SubmergableAreaRef.YMin + (holeDataSet[i].heightByBuoyancy * sinkHeightIncr);

            GameObject holeObj = Instantiate(hole, new Vector3(Random.Range(SubmergableAreaRef.XMin, SubmergableAreaRef.XMax), yPos, transform.position.z - 0.1f), transform.rotation, transform) as GameObject;
            if (yPos <= waterSurfaceYPos) {
                holesSubm.Add(new Hole(holeObj, (int)holeDataSet[i].leakType));
            }
            else {
                holesSurf.Add(new Hole(holeObj, (int)holeDataSet[i].leakType));
            }
        }
    }

    public void AddNumLeaksCallback(NumLeaksDelegate CB) {
        NumLeaksCB = CB;
    }

    private void CheckHolesBoatRaised() {
        for (var i = 0; i < holesSubm.Count; i++) {
            if (holesSubm[i].obj.transform.position.y > waterSurfaceYPos) {
                // TODO: Do something with hole.obj reference - change animation to show it's no longer taking in water (change obj reference to a script reference if needed)
                holesSurf.Add(holesSubm[i]);
                holesSubm.RemoveAt(i);
                NumLeaksCB(holesSubm.Count);
            }
        }
    }
    private void CheckHolesBoatLowered() {
        for (var i = 0; i < holesSurf.Count; i++) {
            if (holesSurf[i].obj.transform.position.y <= waterSurfaceYPos) {
                // TODO: Do something with hole.obj reference - change animation to show it's taking in water (change obj reference to a script reference if needed)
                holesSubm.Add(holesSurf[i]);
                holesSurf.RemoveAt(i);
            }
        }
    }
    protected void AdjustBoatDepth() {
        // Set the boat to where the top of the submergable area is directly on the water's surface, and raise by the buoyancy minus current weight
        float newYPos = waterSurfaceYPos - (Mathf.Abs(transform.position.y - SubmergableAreaRef.YMax)) + (sinkHeightIncr * (buoyancy.Value - weightTotal.Value));
        transform.position = new Vector3(transform.position.x, newYPos, transform.position.z);
    }

    public virtual void AddWater() {
        // Calculate water change value
        int waterWeightChange = 0;
        for (var i = 0; i < holesSubm.Count; i++)
            waterWeightChange += holesSubm[i].leakRate;

        // Each hole eaqually contributes to water gain / buoyancy loss
        weightWater.Value += waterWeightChange;
        weightTotal.Value += waterWeightChange;
        uiUpdate.RaiseEvent();

        AdjustBoatDepth();
        CheckHolesBoatLowered();
    }

    private void Surface(int weight) {
        weightTotal.Value -= weight;
        uiUpdate.RaiseEvent();

        AdjustBoatDepth();
        CheckHolesBoatRaised();
    }
    public void RemoveWater(int weight) {
        weightWater.Value -= weight;
        Surface(weight);
    }
    public void RemoveLoad(int weight) {
        weightLoad.Value -= weight;
        Surface(weight);
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
