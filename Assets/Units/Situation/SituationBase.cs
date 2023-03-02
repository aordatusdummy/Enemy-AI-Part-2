using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Situation", menuName = "ScriptableObjects/Situation", order = 2)]
public class SituationBase : ScriptableObject
{
    #region Variables to be assigned
    [SerializeField] private TheRange roamRange;
    [SerializeField] private ObjectiveType objectiveGiven;
    #endregion

    #region Variables to be accessed
    public TheRange RoamRange { get { return roamRange; } }
    public ObjectiveType ObjectiveGiven { get { return objectiveGiven; } }

    #endregion

    public void MakeAircraftNucleus(TheRange whichRange, Transform aircraftPosition)
    {
        whichRange.ShapeNucleus = aircraftPosition.position;
    }
}
[System.Serializable]
public class TheRange
{
    #region Variables to be assigned
    [SerializeField] private RangeType rangeType;
    [SerializeField] private Vector3 shapeNucleus;
    [SerializeField] private bool aircraftNucleus = false;
    [SerializeField] private float st1;
    [SerializeField] private float st2;
    [SerializeField] private float st3;
    #endregion

    #region Variables to be accessed
    public RangeType RangeType { get { return rangeType; } }
    public Vector3 ShapeNucleus { get; set; }
    public bool AircraftNucleus { get { return aircraftNucleus; } }
    public float ST1 { get { return st1; } }
    public float ST2 { get { return st2; } }
    public float ST3 { get { return st3; } }
    #endregion

}

public enum RangeType
{
    Cube, //ST1 = Radius
    Cuboid, //ST1 = Length, ST2 = Width, ST3 = Height
    Sphere //STl = Radius
}

public enum ObjectiveType
{
    Escape,
    Follow,
    Fight
}



