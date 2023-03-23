using System.Collections.Generic;
using UnityEngine;

public class EnemyCore { }
public class AircraftCore { }
public class SituationCore
{
    public ObjectiveState ObjectiveGiven { get; set; }
}

public class MentalState
{
    public int fearLevel { get; set; }
    public int angerLevel { get; set; }
    public int anxietyLevel { get; set; }
    public int focusLevel { get; set; }
    public int energyLevel { get; set; }
    public int confusion { get; set; }
    public int anger { get; set; }
    public int depression { get; set; }
}

public class ObjectiveState
{
    [SerializeField] private float scout;
    [SerializeField] private float escape;
    [SerializeField] private float patrol;
    [SerializeField] private float fight;
    [SerializeField] private float follow;
}

public class Brain : MonoBehaviour
{
    public int BrainLightness { get; set; }

    [SerializeField] private EnemyCore myEnemyCore;
    [SerializeField] private AircraftCore myAircraftCore;
    [SerializeField] private SituationCore mySituationCore;

    private MentalState myMentalState = new MentalState();
    private ObjectiveState myObjectiveState = new ObjectiveState();


    private void Awake()
    {
        P1InitialStateFiller();
    }

    private void P1InitialStateFiller()
    {
        myObjectiveState = mySituationCore.ObjectiveGiven;

        //Calculate by reading most of the enemyCores
        myMentalState = null;
    }

    public Vector3 GetNewWayPoint()
    {
        return Vector3.zero;
    }
}

public class Controller : MonoBehaviour
{
    private int currentLightness;
    Brain myBrain;
    public Vector3 currentWayPoint { get; set; }
    public List<Vector3> wayPointsGenerated { get; set; }
    private void Awake()
    {
        myBrain = this.GetComponent<Brain>();
    }

    private void FixedUpdate()
    {
        if (currentLightness == myBrain.BrainLightness) { AskBrainForWayPoint(); }
        else { currentLightness += 1; }
    }

    private void AskBrainForWayPoint()
    {
        currentWayPoint = myBrain.GetNewWayPoint();
        currentLightness = 0;
        wayPointsGenerated.Add(currentWayPoint);
    }

    //If Waypoint reached askBrainForWayPoint

    private void LateUpdate()
    {
        DriveToWayPoint();
    }

    public void DriveToWayPoint()
    {

    }
}