using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyBrain))]
public class EnemyController : AircraftController
{
    #region Base
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
        StartMovement();
    }

    public void Init()
    {
        if (myEnemyCore == null || myEnemyCore.name == "Empty")
        {
            int coreSequenceOrder = CoreSequence.CreateRandomEnemyCore();
            myEnemyCore = CoreSequence.randomEnemyCores[coreSequenceOrder];
        }
    }

    private void Update()
    {
        AudioSystem();

        //Airplane move only if not dead
        if (!planeIsDead)
        {
            SimpleMovement();
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

    public void CoreUpdateCopy(EnemyController copyWhat)
    {
        return;
        CoreUpdate(copyWhat.myEnemyCore);
        CoreUpdate(copyWhat.mySituationCore);
        CoreUpdate(copyWhat.myAircraftCore);

    }

    public void CoreUpdate(AircraftCore newAircraftCore)
    {
        myAircraftCore = newAircraftCore;
    }

    public void CoreUpdate(EnemyCore newEnemyCore)
    {
        myEnemyCore = newEnemyCore;
    }

    public void CoreUpdate(SituationCore newSituationCore)
    {
        mySituationCore = newSituationCore;
    }
    #endregion

    #region Movement
    [SerializeField] SituationCore mySituationCore;
    [SerializeField] EnemyCore myEnemyCore;
    public SituationCore MySituationCore { get { return MySituationCore; } }
    public EnemyCore MyEnemyCore { get { return myEnemyCore; } }
    private EnemyBrain enemyBrain;
    private int currentWaypointIndex = 0;
    public float sphereRadius = 2.0f; // Add this line
    private bool isTurbo = false;
    private void StartMovement()
    {
        enemyBrain = this.GetComponent<EnemyBrain>();
        enemyBrain.Init(mySituationCore, myEnemyCore, myAircraftCore);
    }

    private void SimpleMovement()
    {
        CheckObstacle();
        isTurbo = enemyBrain.turboAnalyzing();

        // Get the current waypoint
        Vector3 currentWaypoint = enemyBrain.wayPoints[currentWaypointIndex];

        // Move towards the current waypoint
        Vector3 direction = currentWaypoint - transform.position;
        direction.Normalize();
        transform.position += direction * currentSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime);

        // Check if the enemy has reached the current waypoint
        float distance = Vector3.Distance(transform.position, currentWaypoint);
        if (distance < 0.1f) // Adjust this value to match your desired threshold for reaching a waypoint
        {
            // Increment the current waypoint index
            currentWaypointIndex++;

            // Check if all waypoints have been visited
            if (currentWaypointIndex >= enemyBrain.wayPoints.Count)
            {
                // All waypoints have been visited, generate a new set of waypoints
                enemyBrain.GenerateWayPoints();

                // Reset the current waypoint in dex
                currentWaypointIndex = 0;
            }
        }
    }

    private void CheckObstacle()
    {
        if (crashColliders == null) { SetupColliders(crashCollidersRoot); }
        foreach (var crashCollider in crashColliders)
        {
            Collider[] colliders = Physics.OverlapSphere(crashCollider.transform.position, crashCollider.bounds.size.magnitude);
            foreach (var collider in colliders)
            {
                if (collider.CompareTag("Obstacle"))
                {
                    enemyBrain.wayPoints[currentWaypointIndex] = enemyBrain.GenerateSingleRandomWayPoint();
                    break;
                }
            }
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
        if (isTurbo)
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