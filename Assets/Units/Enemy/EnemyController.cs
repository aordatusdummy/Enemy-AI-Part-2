using UnityEngine;  // This code imports the UnityEngine library, which provides a set of tools for building Unity games

[RequireComponent(typeof(EnemyBrain))] // This code attaches an EnemyBrain component to the game object, which is required for this script

public class EnemyController : AircraftController
// This script inherits from AircraftController and is used to control the behavior of enemy aircraft
// It implements the general objectives for enemy AI
{
    #region Movement
    // Aircraft core is grabbed by the parent AircraftController
    // Grabbing other cores and important components

    // Assign
    [SerializeField] SituationCore mySituationCore;
    [SerializeField] EnemyCore myEnemyCore;
    public SituationCore MySituationCore { get { return mySituationCore; } }
    public EnemyCore MyEnemyCore { get { return myEnemyCore; } }

    // Assist
    private EnemyBrain enemyBrain;
    private int currentWaypointIndex = 0; // Last waypoint collected or New waypoint finding
    public float sphereRadius = 2.0f; // Obstacle collision
    private bool isTurbo = false;

    private void FixedUpdate()
    {
        if (planeIsDead) { return; }
        SimpleMovement();
    }

    // Important assignment, fixing some data and generating first waypoints to avoid error
    private void StartMovement()
    {
        enemyBrain = this.GetComponent<EnemyBrain>();
        enemyBrain.Init(mySituationCore, myEnemyCore, myAircraftCore);
    }

    // If plane is not dead, do simple movement in update
    private void SimpleMovement()
    {
        // CheckObstacle(); // Bad obstacle avoidance because it will create a random way point to escape the obstacle breaking the whole pattern of movement
        // isTurbo = enemyBrain.turboAnalyzing(); //Pretty bad

        // Get the current waypoint
        Vector3 currentWaypoint = enemyBrain.WayPoints[currentWaypointIndex];

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
            if (currentWaypointIndex >= enemyBrain.WayPoints.Count)
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
                    enemyBrain.WayPoints[currentWaypointIndex] = enemyBrain.GenerateSingleRandomWayPoint();
                    break;
                }
            }
        }
    }
    #endregion

    #region Base
    //Initialize planeIsDead variable to true in Awake() function
    private void Awake()
    {
        planeIsDead = true;
    }

    //Setup function to set up rigidbody, speeds, and colliders
    private void Setup()
    {
        //Setup speeds
        maxSpeed = myAircraftCore.DefaultSpeed;
        currentSpeed = myAircraftCore.DefaultSpeed;

        //Get and set rigidbody
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

        //Set up colliders
        SetupColliders(crashCollidersRoot);
    }

    //Init function to initialize the aircraft with the specified cores and start movement
    public void Init(AircraftCore mAC, SituationCore mSC, EnemyCore mEC)
    {
        myAircraftCore = mAC;
        mySituationCore = mSC;
        myEnemyCore = mEC;

        //If no enemy core was specified, create a random one
        if (myEnemyCore == null || myEnemyCore.name == "Empty")
        {
            int coreSequenceOrder = CoreSequence.CreateRandomEnemyCore();
            myEnemyCore = CoreSequence.randomEnemyCores[coreSequenceOrder];
        }

        Setup();
        planeIsDead = false;
        StartMovement();
    }

    //Update function to move the airplane and update its dynamics if it's not dead
    private void Update()
    {
        if (!planeIsDead)
        {
            Dyanmics();
        }
    }

    #region Core Updates

    //CoreUpdateCopy function to copy the cores from another enemy controller
    public void CoreUpdateCopy(EnemyController copyWhat)
    {
        CoreUpdate(copyWhat.myEnemyCore);
        CoreUpdate(copyWhat.mySituationCore);
        CoreUpdate(copyWhat.myAircraftCore);
    }

    //CoreUpdate function to update the aircraft core
    public void CoreUpdate(AircraftCore newAircraftCore)
    {
        myAircraftCore = newAircraftCore;
    }

    //CoreUpdate function to update the enemy core
    public void CoreUpdate(EnemyCore newEnemyCore)
    {
        myEnemyCore = newEnemyCore;
    }

    //CoreUpdate function to update the situation core
    public void CoreUpdate(SituationCore newSituationCore)
    {
        mySituationCore = newSituationCore;
    }
    #endregion

    #endregion
    
    #region Dyanamics
    private void Dyanmics()
    {
        AudioSystem();

        if (!planeIsDead)
        {
            //Rotate propellers if any
            if (propellers.Length > 0)
            {
                RotatePropellers(propellers);
            }

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
}