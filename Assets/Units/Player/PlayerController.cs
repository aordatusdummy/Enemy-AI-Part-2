using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : AircraftController
{
    #region Basic
    private void Start()
    {
        //Setup speeds
        maxSpeed = myAircraftCore.DefaultSpeed;
        currentSpeed = myAircraftCore.DefaultSpeed;

        //Get and set rigidbody
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

        SetupColliders(crashCollidersRoot);
    }

    private void Update()
    {
        AudioSystem();

        //Airplane move only if not dead
        if (!planeIsDead)
        {
            Movement(); 
            Dyanmics();

            //Rotate propellers if any
            if (propellers.Length > 0)
            {
                RotatePropellers(propellers);
            }
        }
        else
        {
            ChangeWingTrailEffectThickness(0f);
        }

        //Control lights if any
        if (turbineLights.Length > 0)
        {
            ControlEngineLights(turbineLights, currentEngineLightIntensity);
        }

        //Crash
        if (!planeIsDead && HitSometing())
        {
            Crash();
        }
    }

    public void CoreUpdate(AircraftCore newAircraftCore)
    {
        myAircraftCore = newAircraftCore;
    }
    #endregion

    #region Movement
    private void Movement()
    {
        //Move forward
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

        //Rotate airplane by inputs
        transform.Rotate(Vector3.forward * -Input.GetAxis("Horizontal") * currentRollSpeed * Time.deltaTime);
        transform.Rotate(Vector3.right * Input.GetAxis("Vertical") * currentPitchSpeed * Time.deltaTime);

        //Rotate yaw
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(Vector3.up * currentYawSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(-Vector3.up * currentYawSpeed * Time.deltaTime);
        }
    }
    #endregion

    #region Dyanamics & Turbo
    private void Dyanmics()
    {
        //Accelerate and deacclerate
        if (currentSpeed < maxSpeed)
        {
            currentSpeed += myAircraftCore.Accelerating * Time.deltaTime;
        }
        else
        {
            currentSpeed -= myAircraftCore.Deaccelerating * Time.deltaTime;
        }

        //Turbo
        if (Input.GetKey(KeyCode.LeftShift))
        {
            //Set speed to turbo speed and rotation to turbo values
            maxSpeed = myAircraftCore.TurboSpeed;

            currentYawSpeed = myAircraftCore.YawSpeed * myAircraftCore.YawTurboMultiplier;
            currentPitchSpeed = myAircraftCore.PitchSpeed * myAircraftCore.PitchTurboMultiplier;
            currentRollSpeed = myAircraftCore.RollSpeed * myAircraftCore.RollTurboMultiplier;

            //Engine lights
            currentEngineLightIntensity = myAircraftCore.TurbineLightTurbo;

            //Effects
            ChangeWingTrailEffectThickness(myAircraftCore.TrailThickness);

            //Audio
            currentEngineSoundPitch = myAircraftCore.TurboSoundPitch;
        }
        else
        {
            //Speed and rotation normal
            maxSpeed = myAircraftCore.DefaultSpeed;

            currentYawSpeed = myAircraftCore.YawSpeed;
            currentPitchSpeed = myAircraftCore.PitchSpeed;
            currentRollSpeed = myAircraftCore.RollSpeed;

            //Engine lights
            currentEngineLightIntensity = myAircraftCore.TurbineLightDefault;

            //Effects
            ChangeWingTrailEffectThickness(0f);

            //Audio
            currentEngineSoundPitch = myAircraftCore.DefaultSoundPitch;
        }

    }
    #endregion


}
