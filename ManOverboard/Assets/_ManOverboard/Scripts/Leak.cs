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

    private void Awake()
    {
        boatConnection = this.GetComponentIfNull(boatConnection);

        myRigidbody = this.GetComponentIfNull(myRigidbody);
    }

    public void AttachLeak(Rigidbody2D BoatRigid)
    {
        boatConnection.connectedBody = BoatRigid;
    }

}
