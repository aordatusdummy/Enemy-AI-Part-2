using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Aircraft", menuName = "ScriptableObjects/Aircraft", order = 0)]
public class AircraftBase : ScriptableObject
{
    #region Controller
    #region Variables to be assigned
    [Header("Wing trail effects")] 
    [Range(0.01f, 1f)] [SerializeField] private float trailThickness = 0.045f;

    [Header("Rotating speeds")]
    [Range(5f, 500f)] [SerializeField] private float yawSpeed = 50f;

    [Range(5f, 500f)] [SerializeField] private float pitchSpeed = 100f;

    [Range(5f, 500f)] [SerializeField] private float rollSpeed = 200f;

    [Header("Rotating speeds multiplers when turbo is used")]
    [Range(0.1f, 5f)] [SerializeField] private float yawTurboMultiplier = 0.3f;

    [Range(0.1f, 5f)] [SerializeField] private float pitchTurboMultiplier = 0.5f;

    [Range(0.1f, 5f)] [SerializeField] private float rollTurboMultiplier = 1f;

    [Header("Moving speed")]
    [Range(5f, 100f)] [SerializeField] private float defaultSpeed = 10f;

    [Range(10f, 200f)] [SerializeField] private float turboSpeed = 20f;

    [Range(0.1f, 50f)] [SerializeField] private float accelerating = 10f;

    [Range(0.1f, 50f)] [SerializeField] private float deaccelerating = 5f;

    [Header("Engine sound settings")]
    [SerializeField] private float defaultSoundPitch = 1f;

    [SerializeField] private float turboSoundPitch = 1.5f;

    [Header("Engine propellers settings")]
    [Range(10f, 10000f)] [SerializeField] private float propelSpeedMultiplier = 100f;

    [Header("Turbine light settings")]
    [Range(0.1f, 20f)] [SerializeField] private float turbineLightDefault = 1f;

    [Range(0.1f, 20f)] [SerializeField] private float turbineLightTurbo = 5f;
    #endregion

    #region Variables to be accessed
    public float TrailThickness { get { return trailThickness; } }
    public float YawSpeed { get { return yawSpeed; } }
    public float PitchSpeed { get { return pitchSpeed; } }
    public float RollSpeed { get { return rollSpeed; } }
    public float YawTurboMultiplier { get { return yawTurboMultiplier; } }
    public float PitchTurboMultiplier { get { return pitchTurboMultiplier; } }
    public float RollTurboMultiplier { get { return rollTurboMultiplier; } }
    public float DefaultSpeed { get { return defaultSpeed; } }
    public float TurboSpeed { get { return turboSpeed; } }
    public float Accelerating { get { return accelerating; } }
    public float Deaccelerating { get { return deaccelerating; } }
    public float DefaultSoundPitch { get { return defaultSoundPitch; } }
    public float TurboSoundPitch { get { return turboSoundPitch; } }
    public float PropelSpeedMultiplier { get { return propelSpeedMultiplier; } }
    public float TurbineLightDefault { get { return turbineLightDefault; } }
    public float TurbineLightTurbo { get { return turbineLightTurbo; } }
    #endregion
    #endregion

    #region Values to be assigned
    [SerializeField] private float maxSafeAltitude;
    [SerializeField] private float searchDistance;

    #endregion

    #region Values to be accesseds
    public float MaxSafeAltitude { get { return maxSafeAltitude; } }
    public float SearchDistance { get { return searchDistance; } }

    #endregion
}
