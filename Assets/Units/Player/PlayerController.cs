using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : AircraftController
{
    #region Base
    private void Start()
    {
        //Setup speeds
        maxSpeed = aircraftBase.DefaultSpeed;
        currentSpeed = aircraftBase.DefaultSpeed;

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
            currentSpeed += aircraftBase.Accelerating * Time.deltaTime;
        }
        else
        {
            currentSpeed -= aircraftBase.Deaccelerating * Time.deltaTime;
        }

        //Turbo
        if (Input.GetKey(KeyCode.LeftShift))
        {
            //Set speed to turbo speed and rotation to turbo values
            maxSpeed = aircraftBase.TurboSpeed;

            currentYawSpeed = aircraftBase.YawSpeed * aircraftBase.YawTurboMultiplier;
            currentPitchSpeed = aircraftBase.PitchSpeed * aircraftBase.PitchTurboMultiplier;
            currentRollSpeed = aircraftBase.RollSpeed * aircraftBase.RollTurboMultiplier;

            //Engine lights
            currentEngineLightIntensity = aircraftBase.TurbineLightTurbo;

            //Effects
            ChangeWingTrailEffectThickness(aircraftBase.TrailThickness);

            //Audio
            currentEngineSoundPitch = aircraftBase.TurboSoundPitch;
        }
        else
        {
            //Speed and rotation normal
            maxSpeed = aircraftBase.DefaultSpeed;

            currentYawSpeed = aircraftBase.YawSpeed;
            currentPitchSpeed = aircraftBase.PitchSpeed;
            currentRollSpeed = aircraftBase.RollSpeed;

            //Engine lights
            currentEngineLightIntensity = aircraftBase.TurbineLightDefault;

            //Effects
            ChangeWingTrailEffectThickness(0f);

            //Audio
            currentEngineSoundPitch = aircraftBase.DefaultSoundPitch;
        }

    }
    #endregion
}
