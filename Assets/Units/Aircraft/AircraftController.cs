using UnityEngine; // This namespace contains all the core functionality for Unity, including components, game objects, and transforms.
using System.Collections.Generic; // This namespace contains commonly used data structures such as lists, dictionaries and queues.

[RequireComponent(typeof(Rigidbody))]
public class AircraftController : MonoBehaviour
{
    #region Variables that are protected
    // List of all colliders of the aircraft
    protected List<AircraftCollider> airPlaneColliders = new List<AircraftCollider>();

    // Maximum speed of the aircraft
    protected float maxSpeed = 0.6f;

    // Current speed of the aircraft for yaw, pitch, and roll
    protected float currentYawSpeed;
    protected float currentPitchSpeed;
    protected float currentRollSpeed;
    protected float currentSpeed;

    // Current intensity and pitch of the engine sound
    protected float currentEngineLightIntensity;
    protected float currentEngineSoundPitch;

    // Boolean to check if the plane is dead
    protected bool planeIsDead;

    // Rigidbody component of the aircraft
    protected Rigidbody rb;
    #endregion

    #region Variables to be assigned
    // Wing trail effects that will be assigned in the inspector
    [SerializeField] protected TrailRenderer[] wingTrailEffects;

    // Engine sound source that will be assigned in the inspector
    [SerializeField] protected AudioSource engineSoundSource;

    // Array of propellers that will be assigned in the inspector
    [SerializeField] protected GameObject[] propellers;

    // Array of turbine lights that will be assigned in the inspector
    [SerializeField] protected Light[] turbineLights;

    // Root of the crash colliders that will be assigned in the inspector
    [SerializeField] protected Transform crashCollidersRoot;

    // Root of the camera that will be assigned in the inspector
    [SerializeField] Transform cameraRoot;

    // Aircraft core component that will be assigned in the inspector
    [SerializeField] protected AircraftCore myAircraftCore;
    #endregion

    #region Variables that are accessed
    // Getter for the camera root transform
    public Transform CameraRoot { get { return cameraRoot; } }

    public AircraftCore MyAicraftCore { get { return myAircraftCore; } }
    #endregion

    #region Audio
    // Update engine sound volume and pitch
    protected void AudioSystem()
    {
        // Smoothly change engine pitch
        engineSoundSource.pitch = Mathf.Lerp(engineSoundSource.pitch, currentEngineSoundPitch, 10f * Time.deltaTime);

        // Fade out engine sound when plane is dead
        if (planeIsDead)
        {
            engineSoundSource.volume = Mathf.Lerp(engineSoundSource.volume, 0f, 0.1f);
        }
    }
    #endregion

    #region Private methods
    //List of colliders for the plane
    protected List<Collider> crashColliders;

    //Setup colliders for plane
    protected void SetupColliders(Transform _root)
    {
        //Get all colliders from root transform
        Collider[] colliders = _root.GetComponentsInChildren<Collider>();

        //Loop through colliders and add components to them
        for (int i = 0; i < colliders.Length; i++)
        {
            //Change collider to trigger
            colliders[i].isTrigger = true;

            //Get current gameobject
            GameObject _currentObject = colliders[i].gameObject;

            //Add airplane collider to the current object and add it to the list
            AircraftCollider _airplaneCollider = _currentObject.AddComponent<AircraftCollider>();
            airPlaneColliders.Add(_airplaneCollider);

            //Add rigidbody to the current object
            Rigidbody _rb = _currentObject.AddComponent<Rigidbody>();
            _rb.useGravity = false;
            _rb.isKinematic = true;
            _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }

        //Get all colliders from the crash collider root transform
        crashColliders = new List<Collider>(crashCollidersRoot.GetComponentsInChildren<Collider>());
    }

    //Rotate the propellers
    protected void RotatePropellers(GameObject[] _rotateThese)
    {
        //Calculate the propel speed
        float _propelSpeed = currentSpeed * myAircraftCore.PropelSpeedMultiplier;

        //Loop through the gameobjects to rotate and rotate them
        for (int i = 0; i < _rotateThese.Length; i++)
        {
            _rotateThese[i].transform.Rotate(Vector3.forward * -_propelSpeed * Time.deltaTime);
        }
    }

    //Control the engine lights
    protected void ControlEngineLights(Light[] _lights, float _intensity)
    {
        //Calculate the propel speed
        float _propelSpeed = currentSpeed * myAircraftCore.PropelSpeedMultiplier;

        //Loop through the lights and control their intensity
        for (int i = 0; i < _lights.Length; i++)
        {
            if (!planeIsDead)
            {
                _lights[i].intensity = Mathf.Lerp(_lights[i].intensity, _intensity, 10f * Time.deltaTime);
            }
            else
            {
                _lights[i].intensity = Mathf.Lerp(_lights[i].intensity, 0f, 10f * Time.deltaTime);
            }
        }
    }

    //Change the wing trail effect thickness
    protected void ChangeWingTrailEffectThickness(float _thickness)
    {
        //Loop through the wing trail effects and change their thickness
        for (int i = 0; i < wingTrailEffects.Length; i++)
        {
            wingTrailEffects[i].startWidth = Mathf.Lerp(wingTrailEffects[i].startWidth, _thickness, Time.deltaTime * 10f);
        }
    }

    //Check if the plane has hit something
    protected bool HitSometing()
    {
        //Loop through the airplane colliders and check if any of them collide with something
        for (int i = 0; i < airPlaneColliders.Count; i++)
        {
            if (airPlaneColliders[i].collideSometing)
            {
                return true;
            }
        }

        return false;
    }


    protected void Crash()
    {
        //Set rigidbody to non cinematic
        rb.isKinematic = false;
        rb.useGravity = true;

        //Change every collider trigger state and remove rigidbodys
        for (int i = 0; i < airPlaneColliders.Count; i++)
        {
            airPlaneColliders[i].GetComponent<Collider>().isTrigger = false;
            Destroy(airPlaneColliders[i].GetComponent<Rigidbody>());
        }

        //Kill player
        planeIsDead = true;

        //What happens next?
        var controller = GameObject.FindGameObjectWithTag("GameController");
        if (controller)
        {
            controller.GetComponent<MasterController>().TerminationRun();
        }

        print("Crash");
    }
    #endregion

    #region Variables
    //Returns a percentage of how fast the current speed is from the maximum speed between 0 and 1
    public float PercentToMaxSpeed()
    {
        float _percentToMax = currentSpeed / myAircraftCore.TurboSpeed;

        return _percentToMax;
    }

    public bool PlaneIsDead()
    {
        return planeIsDead;
    }

    public bool UsingTurbo()
    {
        if (maxSpeed == myAircraftCore.TurboSpeed)
        {
            return true;
        }

        return false;
    }

    public float CurrentSpeed()
    {
        return currentSpeed;
    }
    #endregion
}
