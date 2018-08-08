using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Boat : MonoBehaviour {

    [SerializeField]
    [Tooltip("The rigid body of this object")]
    private Rigidbody2D myRigidbody;

    [SerializeField]
    [Tooltip("The colliders that make up this object")]
    private Collider2D[] myColliders;

    [SerializeField]
    private Leak LeakPrefab;

    /// <summary>
    /// The game objects that represent leaks in the boat
    /// </summary>
    private List<Leak> activeLeaks = new List<Leak>();

    private void Awake()
    {
        myRigidbody = this.GetComponentIfNull(myRigidbody);
        
        if(myColliders == null || myColliders.Length == 0)
            myColliders = GetComponentsInChildren<Collider2D>();
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
