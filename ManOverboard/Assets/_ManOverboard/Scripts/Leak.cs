using System.Collections;
using UnityEngine;
using ZeroProgress.Common;

[RequireComponent(typeof(Rigidbody2D), typeof(Joint2D))]
public class Leak : MonoBehaviour {

    [SerializeField]
    [Tooltip("The joint that connects this leak to the boat")]
    private FixedJoint2D boatConnection;

    [SerializeField]
    [Tooltip("The rigidbody that represents the weight caused by the leak")]
    private Rigidbody2D myRigidbody;

    [SerializeField]
    [Tooltip("How much weight to apply to the leak")]
    private float fillRate = 0.001f;

    [SerializeField]
    [Tooltip("How often to apply the fill rate")]
    private float fillInterval = 0.5f;

    private bool isAttachedToBoat = false;

    private Coroutine fillCoroutine = null;
    
    private void Awake()
    {
        boatConnection = this.GetComponentIfNull(boatConnection);

        myRigidbody = this.GetComponentIfNull(myRigidbody);
    }

    private void OnEnable()
    {
        fillCoroutine = StartCoroutine(LeakFill());
    }

    private void OnDisable()
    {
        StopCoroutine(fillCoroutine);
        fillCoroutine = null;
    }

    public void AttachLeak(Rigidbody2D BoatRigid)
    {
        boatConnection.connectedBody = BoatRigid;
        isAttachedToBoat = true;
    }

    private IEnumerator LeakFill()
    {
        while (true)
        {
            if (isAttachedToBoat)
                myRigidbody.mass += fillRate;

            yield return new WaitForSeconds(fillInterval);
        }
    }



}
