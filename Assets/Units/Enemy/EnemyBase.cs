using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyName", menuName = "ScriptableObjects/Enemy", order = 1)]
public class EnemyBase : ScriptableObject 
{
    public string enemyID { get; set; }

    #region Variables to be assigned
    [SerializeField] private MasterAttribute strategyTacticalAwareness;
    [SerializeField] private MasterAttribute strategyPositionalAwareness;
    [SerializeField] private MasterAttribute strategyStrategemAgility;
    [SerializeField] private MasterAttribute strategyCalculativeAgility;
    [SerializeField] private MasterAttribute strategyInitiative;

    [SerializeField] private MasterAttribute wisdomRiskTolerance;
    [SerializeField] private MasterAttribute wisdomStability;
    [SerializeField] private MasterAttribute wisdomCourage;
    [SerializeField] private MasterAttribute wisdomResilience;
    [SerializeField] private MasterAttribute wisdomExperience;

    [SerializeField] private MasterAttribute personalityCommunication;
    [SerializeField] private MasterAttribute personalityObedience;
    [SerializeField] private MasterAttribute personalityImagination;
    [SerializeField] private MasterAttribute personalityHonor;
    [SerializeField] private MasterAttribute personalityAgression;

    [SerializeField] private MasterAttribute skillRankOrder;
    [SerializeField] private MasterAttribute skillMemory;
    [SerializeField] private MasterAttribute skillControlMastery;
    [SerializeField] private MasterAttribute skillManeuverMastery;
    [SerializeField] private MasterAttribute skillGearMastery;

    [SerializeField] private List<Maneuver> behaviorManeuverPreference = new List<Maneuver>();
    [SerializeField] private MasterAttribute behaviorSpeedPreference;
    [SerializeField] private MasterAttribute behaviorAltitudePreference;
    #endregion

    #region Variables to be accessed 
    public MasterAttribute StrategyTacticalAwareness { get { return strategyTacticalAwareness; } set { strategyTacticalAwareness = value; } }
    public MasterAttribute StrategyPositionalAwareness { get { return strategyPositionalAwareness; } set { strategyPositionalAwareness = value; } }
    public MasterAttribute StrategyStrategemAgility { get { return strategyStrategemAgility; } set { strategyStrategemAgility = value; } }
    public MasterAttribute StrategyCalculativeAgility { get { return strategyCalculativeAgility; } set { strategyCalculativeAgility = value; } }
    public MasterAttribute StrategyInitiative { get { return strategyInitiative; } set { strategyInitiative = value; } }

    public MasterAttribute WisdomRiskTolerance { get { return wisdomRiskTolerance; } set { wisdomRiskTolerance = value; } }
    public MasterAttribute WisdomStability { get { return wisdomStability; } set { wisdomStability = value; } }
    public MasterAttribute WisdomCourage { get { return wisdomCourage; } set { wisdomCourage = value; } }
    public MasterAttribute WisdomResilience { get { return wisdomResilience; } set { wisdomResilience = value; } }
    public MasterAttribute WisdomExperience { get { return wisdomExperience; } set { wisdomExperience = value; } }

    public MasterAttribute PersonalityCommunication { get { return personalityCommunication; } set { personalityCommunication = value; } }
    public MasterAttribute PersonalityObedience { get { return personalityObedience; } set { personalityObedience = value; } }
    public MasterAttribute PersonalityImagination { get { return personalityImagination; } set { personalityImagination = value; } }
    public MasterAttribute PersonalityHonor { get { return personalityHonor; } set { personalityHonor = value; } }
    public MasterAttribute PersonalityAgression { get { return personalityAgression; } set { personalityAgression = value; } }

    public MasterAttribute SkillRankOrder { get { return skillRankOrder; } set { skillRankOrder = value; } }
    public MasterAttribute SkillMemory { get { return skillMemory; } set { skillMemory = value; } }
    public MasterAttribute SkillControlMastery { get { return skillControlMastery; } set { skillControlMastery = value; } }
    public MasterAttribute SkillManeuverMastery { get { return skillManeuverMastery; } set { skillManeuverMastery = value; } }
    public MasterAttribute SkillGearMastery { get { return skillGearMastery; } set { skillGearMastery = value; } }

    public List<Maneuver> BehaviorManeuverPreference { get { return behaviorManeuverPreference; } set { behaviorManeuverPreference = value; } }
    public MasterAttribute BehaviorSpeedPreference { get { return behaviorSpeedPreference; } set { behaviorSpeedPreference = value; } }
    public MasterAttribute BehaviorAltitudePreference { get { return behaviorAltitudePreference; } set { behaviorAltitudePreference = value; } }
    #endregion

    #region Randomize
    public static EnemyBase CreateRandomEnemyBase()
    {
        EnemyBase enemyBase = CreateInstance<EnemyBase>();

        PropertyInfo[] properties = typeof(EnemyBase).GetProperties();
        foreach (PropertyInfo property in properties)
        {
            if (property.CanWrite)
            {
                if (property.PropertyType == typeof(MasterAttribute))
                {
                    MasterAttribute ma = new MasterAttribute();
                    ma.StatBase = Random.Range(0f, 100f);
                    ma.StatCons = Random.Range(0f, 1f);
                    ma.StatMarg = Random.Range(0f, 1f);
                    property.SetValue(enemyBase, ma);

                }
                else if (property.PropertyType == typeof(bool))
                {
                    property.SetValue(enemyBase, Random.value > 0.5f);
                }
                // Add more property types if necessary
            }
        }
        return enemyBase;
    }
    #endregion
}

#region Base
public enum Maneuver
{
    None,
}

[System.Serializable]
public class MasterAttribute
{
    [Range(0, 100)]
    [SerializeField] float statBase; //Base Value
    [Range(0, 1)]
    [SerializeField] float statCons; //Consistency
    [Range(0, 1)]
    [SerializeField] float statMarg; //Margin

    public float StatBase { get { return statBase; } set { statBase = value; } }
    public float StatCons { get { return statCons; } set { statCons = value; } }
    public float StatMarg { get { return statMarg; } set { statMarg = value; } }

    public float StatGet
    {
        get
        {
            //Use all three and get the stat random
            return statBase;
        }
    }
}
#endregion