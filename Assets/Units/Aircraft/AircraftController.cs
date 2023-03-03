using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class AircraftController : MonoBehaviour
{
    #region Variables that are protected
    protected List<AircraftCollider> airPlaneColliders = new List<AircraftCollider>();
    protected float maxSpeed = 0.6f;
    protected float currentYawSpeed;
    protected float currentPitchSpeed;
    protected float currentRollSpeed;
    protected float currentSpeed;
    protected float currentEngineLightIntensity;
    protected float currentEngineSoundPitch;
    protected bool planeIsDead;
    protected Rigidbody rb;

    #endregion

    #region Variables to be assigned
    [SerializeField] protected TrailRenderer[] wingTrailEffects;

    [SerializeField] protected AudioSource engineSoundSource;

    [SerializeField] protected GameObject[] propellers;

    [SerializeField] protected Light[] turbineLights;

    [SerializeField] protected Transform crashCollidersRoot;

    [SerializeField] Transform cameraRoot;

    [SerializeField] protected AircraftCore AircraftCore;
    #endregion

    public Transform CameraRoot { get { return cameraRoot; } }


    #region Audio
    protected void AudioSystem()
    {
        engineSoundSource.pitch = Mathf.Lerp(engineSoundSource.pitch, currentEngineSoundPitch, 10f * Time.deltaTime);

        if (planeIsDead)
        {
            engineSoundSource.volume = Mathf.Lerp(engineSoundSource.volume, 0f, 0.1f);
        }
    }
    #endregion

    #region Private methods
    protected List<Collider> crashColliders;

    protected void SetupColliders(Transform _root)
    {
        //Get colliders from root transform
        Collider[] colliders = _root.GetComponentsInChildren<Collider>();

        //If there are colliders put components in them
        for (int i = 0; i < colliders.Length; i++)
        {
            //Change collider to trigger
            colliders[i].isTrigger = true;

            GameObject _currentObject = colliders[i].gameObject;

            //Add airplane collider to it and put it on the list
            AircraftCollider _airplaneCollider = _currentObject.AddComponent<AircraftCollider>();
            airPlaneColliders.Add(_airplaneCollider);

            //Add rigid body to it
            Rigidbody _rb = _currentObject.AddComponent<Rigidbody>();
            _rb.useGravity = false;
            _rb.isKinematic = true;
            _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }

        crashColliders = new List<Collider>(crashCollidersRoot.GetComponentsInChildren<Collider>());
    }

    protected void RotatePropellers(GameObject[] _rotateThese)
    {
        float _propelSpeed = currentSpeed * AircraftCore.PropelSpeedMultiplier;

        for (int i = 0; i < _rotateThese.Length; i++)
        {
            _rotateThese[i].transform.Rotate(Vector3.forward * -_propelSpeed * Time.deltaTime);
        }
    }

    protected void ControlEngineLights(Light[] _lights, float _intensity)
    {
        float _propelSpeed = currentSpeed * AircraftCore.PropelSpeedMultiplier;

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

    protected void ChangeWingTrailEffectThickness(float _thickness)
    {
        for (int i = 0; i < wingTrailEffects.Length; i++)
        {
            wingTrailEffects[i].startWidth = Mathf.Lerp(wingTrailEffects[i].startWidth, _thickness, Time.deltaTime * 10f);
        }
    }

    protected bool HitSometing()
    {
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

        //Here you can add your own code...
        var tester = GameObject.FindGameObjectWithTag("GameController");
        if (tester)
        {

            //tester.GetComponent<TestManager>().canRestart = true;
            tester.GetComponent<MasterController>().LowerConsoleUpdate("Press R to restart level", MasterController.LowerConsoleTaskType.StayDefaultOff);
        }
    }
    #endregion

    #region Variables
    //Returns a percentage of how fast the current speed is from the maximum speed between 0 and 1
    public float PercentToMaxSpeed()
    {
        float _percentToMax = currentSpeed / AircraftCore.TurboSpeed;

        return _percentToMax;
    }

    public bool PlaneIsDead()
    {
        return planeIsDead;
    }

    public bool UsingTurbo()
    {
        if (maxSpeed == AircraftCore.TurboSpeed)
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
