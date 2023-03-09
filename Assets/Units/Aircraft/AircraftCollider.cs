using UnityEngine;

public class AircraftCollider : MonoBehaviour
{
    public bool collideSometing; // Flag to indicate if the aircraft has collided with something

    // This method is called when another collider enters this collider's trigger
    private void OnTriggerEnter(Collider other)
    {
        // If the other collider doesn't belong to the same AircraftCollider component, then it means the aircraft has collided with something
        if (other.gameObject.GetComponent<AircraftCollider>() == null)
        {
            collideSometing = true; // Set the flag to true
        }
    }
}
