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
        maxSpeed = aircraftBase.DefaultSpeed;
        currentSpeed = aircraftBase.DefaultSpeed;

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
        EnemyBase randomEnemyBase = EnemyBase.CreateRandomEnemyBase();
        this.enemyBase = randomEnemyBase;
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
    #endregion

    #region Movement
    [SerializeField] SituationBase situationBase;
    [SerializeField] EnemyBase enemyBase;
    private EnemyBrain enemyBrain;
    private int currentWaypointIndex = 0;
    public float sphereRadius = 2.0f; // Add this line
    private bool isTurbo = false;
    private void StartMovement()
    {
        enemyBrain = this.GetComponent<EnemyBrain>();
        enemyBrain.Init(situationBase, enemyBase, aircraftBase);
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
            currentSpeed += aircraftBase.Accelerating * Time.deltaTime;
        }
        else
        {
            currentSpeed -= aircraftBase.Deaccelerating * Time.deltaTime;
        }

        //Turbo
        if (isTurbo)
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