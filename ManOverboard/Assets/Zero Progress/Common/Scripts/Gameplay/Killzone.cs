using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroProgress.Common
{
    [RequireComponent(typeof(Collider))]
    public class Killzone : MonoBehaviour
    { 
        private void OnTriggerEnter(Collider other)
        {
            Destroy(other.gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            Destroy(collision.collider.gameObject);
        }
    }
}