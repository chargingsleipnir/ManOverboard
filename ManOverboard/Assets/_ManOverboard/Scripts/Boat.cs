using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;
using ZeroProgress.Common.Collections;
using Game2DWaterKit;

public class Hole {
    public GameObject obj;
    public bool submerged;

    public Hole(GameObject obj, bool submerged) {
        this.obj = obj;
        this.submerged = submerged;
    }
}

//[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Boat : MonoBehaviour {

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

    protected bool sinking = false;
    public bool Sinking {
        get { return sinking; }
        set { sinking = value; }
    }

    float sinkHeightIncr;

    public GameObject hole;
    [Tooltip("Use values in line with object weight, buoyancy, etc. from the bottom of the boat, upward.")]
    public int[] holeHeightsByBuoyancy;
    private List<Hole> holes;
    private int numLeaks;

    public IntReference buoyancy;
    public IntReference weightWater;
    public IntReference weightLoad; // Weight of people and items, things that can be tossed off by the player directly.
    public IntReference weightTotal;
    public int WeightTotal {
        get { return weightTotal.Value; }
    }
    public ComponentSet characterSet;

    /// <summary>
    /// The game objects that represent leaks in the boat
    /// </summary>
    protected List<Leak> activeLeaks = new List<Leak>();

    protected void Awake()
    {
        myRigidbody = this.GetComponentIfNull(myRigidbody);
        
        if(myColliders == null || myColliders.Length == 0)
            myColliders = GetComponentsInChildren<Collider2D>();

        SubmergableAreaRef = GetComponent<RefRect2D>();
        waterSurfaceYPos = (water.transform.position.y + (water.GetComponent<Game2DWater>().WaterSize.y * 0.5f));
        holes = new List<Hole>();
    }

    protected void OnStart(int buoyancyTotal) {

        buoyancy.Value = buoyancyTotal;

        // Default boat, no load, max buoyancy
        weightLoad.Value = 0;
        weightTotal.Value = 0;

        // Add people - increasing load weight & reducing buoyancy
        foreach (Character character in characterSet) {
            weightLoad.Value += character.weight;
        }
        weightTotal.Value += weightLoad.Value;
        // TODO: Add ITEMS - increasing load weight & reducing buoyancy

        // Account current water load
        weightWater.Value = weightWater.StraightValue;
        weightTotal.Value += weightWater.Value;

        // Set height of boat based on current buoyancy and water's surface (usually set to 0)
        sinkHeightIncr = SubmergableAreaRef.height / buoyancyTotal;
        AdjustSunkenDepth();

        // Put hole(s) in boat
        numLeaks = 0;
        for (var i = 0; i < holeHeightsByBuoyancy.Length; i++) {
            // Calulate proper height
            float yPos = SubmergableAreaRef.YMin + (holeHeightsByBuoyancy[i] * sinkHeightIncr);

            GameObject holeObj = Instantiate(hole, new Vector3(Random.Range(SubmergableAreaRef.XMin, SubmergableAreaRef.XMax), yPos, transform.position.z - 0.1f), transform.rotation, transform) as GameObject;
            if (yPos <= waterSurfaceYPos) {
                numLeaks++;
                holes.Add(new Hole(holeObj, true));
            }
            else {
                holes.Add(new Hole(holeObj, false));
            }
        }

        sinking = true;
    }

    public void AddNumLeaksCallback(NumLeaksDelegate CB) {
        NumLeaksCB = CB;
    }

    protected void AdjustSunkenDepth() {
        // Set the boat to where the top of the submergable area is directly on the water's surface, and raise by the buoyancy minus current weight
        float newYPos = waterSurfaceYPos - (Mathf.Abs(transform.position.y - SubmergableAreaRef.YMax)) + (sinkHeightIncr * (buoyancy.Value - weightTotal.Value));
        transform.position = new Vector3(transform.position.x, newYPos, transform.position.z);

        // TODO: Try to make this more efficient. Keep a reference to the next closest leaks above & below the water line?
        for (var i = 0; i < holes.Count; i++) {
            if (holes[i].submerged) {
                if (holes[i].obj.transform.position.y > waterSurfaceYPos) {
                    holes[i].submerged = false;
                    numLeaks--;
                    // TODO: Do something with hole.obj reference - change animation to show it's no longer taking in water
                    NumLeaksCB(numLeaks);
                }
            }
            else {
                if(holes[i].obj.transform.position.y <= waterSurfaceYPos) {
                    holes[i].submerged = true;
                    numLeaks++;
                    NumLeaksCB(numLeaks);
                }
            }
        }
    }

    public virtual void SinkInterval() {
        if (!sinking)
            return;

        // Each hole eaqually contributes to water gain / buoyancy loss
        weightWater.Value += numLeaks;
        weightTotal.Value += numLeaks;
        AdjustSunkenDepth();
    }

    public void LightenLoad(int weight) {
        weightLoad.Value -= weight;
        weightTotal.Value -= weight;
        AdjustSunkenDepth();
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

        Vector3 position = new Vector3(randomX, randomY, this.transform.position.z);
        
        Leak newLeak = Instantiate(LeakPrefab, position, Quaternion.identity);
        newLeak.AttachLeak(myRigidbody);

        activeLeaks.Add(newLeak);
    }
}
