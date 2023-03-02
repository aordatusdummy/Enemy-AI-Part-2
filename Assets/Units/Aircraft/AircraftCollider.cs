using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AircraftCollider : MonoBehaviour
{
    public bool collideSometing;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<AircraftCollider>() == null)
        {
            collideSometing = true;
        }
    }
}
