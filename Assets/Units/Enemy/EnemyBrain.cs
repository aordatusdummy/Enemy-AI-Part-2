using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    #region Base Data
    private EnemyCore eBase;
    private SituationCore sBase;
    private AircraftCore aBase;

    public void Init(SituationCore sb, EnemyCore eb, AircraftCore ab)
    {
        eBase = eb;
        sBase = sb;
        aBase = ab;

        FixingBaseData();
        GenerateWayPoints();

    }

    private void FixingBaseData()
    {
        sBase.MakeAircraftNucleus(sBase.RoamRange, this.transform);
    }
    #endregion

    #region WayPoints
    [SerializeField] private bool randomWayPoints;
    [SerializeField] private float v1 = 10; //number of way points
    [SerializeField] private float v2 = 10; //distance between wayPoints
    public List<Vector3> wayPoints { get; set; }
    public void GenerateWayPoints()
    {
        if(randomWayPoints)
        {
            GenerateRandomWayPoints();
            return;
        }

        Vector3 currentWayPoint = this.transform.position;
        wayPoints = new List<Vector3>();
        for (int i = 0; i < v1; i++)
        {
            Vector3 newWayPoint = new Vector3();
            Vector3 lastWayPoint = (i == 0) ? currentWayPoint : wayPoints[i-1];

            //v1
            newWayPoint.z = lastWayPoint.z + v2;

            //v2
            newWayPoint.y = lastWayPoint.y + v3Analyzing(wayPoints, lastWayPoint);

            //v3
            newWayPoint.z = lastWayPoint.x + Random.Range(-1, 1);
            wayPoints.Add(newWayPoint);
        }
    }
    public void GenerateRandomWayPoints()
    {
        wayPoints = new List<Vector3>();
        for (int i = 0; i < v1; i++)
        {
            wayPoints.Add(GenerateSingleRandomWayPoint());
        }
    }
    public Vector3 GenerateSingleRandomWayPoint()
    {
        return BrainUtilities.RandomVectorInRange(sBase.RoamRange);
    }
    #endregion

    #region Analyzing Base Classes

    private float v3Analyzing(List<Vector3> currentWayPointsList, Vector3 lastWayPoint)
    {
        var msa = aBase.MaxSafeAltitude;
        var bap = eBase.BehaviorAltitudePreference.StatGet;
        var zin = eBase.BehaviorAltitudePreference.StatBase;

        int direction = (lastWayPoint.y >= bap*msa/100) ? 0 : 1;

        return direction*v2;
    }

    private float sensitivity = 1f;
    private System.Random random = new System.Random();
    public bool turboAnalyzing()
    {
        float offenseValue = eBase.PersonalityAgression.StatGet;
        float normalizedValue = offenseValue / 20f;
        float threshold = (float)random.NextDouble();
        if (threshold <= normalizedValue * sensitivity)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    /*
    1. How to get 32 wishes in 8 days... How to get 4 wishes everyday... 
    2. 4x160=640 needed 
    3. How to get 28 wishes in 9 days... How to get 3 wishes everyday...
    4. 
     */
    #endregion

    #region Utility
    public static class BrainUtilities
    {
        public static Vector3 RandomVectorInRange(TheRange tRange)
        {
            Vector3 res;
            if (tRange.RangeType == RangeType.Cube)
            {
                Vector3 n = tRange.ShapeNucleus;
                float st = tRange.ST1;
                res = new Vector3(UnityEngine.Random.Range(n.x + st / 2, n.x - st / 2), UnityEngine.Random.Range(n.y + st / 2, n.y - st / 2), UnityEngine.Random.Range(n.z + st / 2, n.z - st / 2));
            }
            else if (tRange.RangeType == RangeType.Cuboid)
            {
                Vector3 n = tRange.ShapeNucleus; 
                float st1 = tRange.ST1;
                float st2 = tRange.ST2;
                float st3 = tRange.ST3;
                res = new Vector3(UnityEngine.Random.Range(n.x + st1 / 2, n.x - st1 / 2), UnityEngine.Random.Range(n.y + st3 / 2, n.y - st3 / 2), UnityEngine.Random.Range(n.z + st2 / 2, n.z - st2 / 2));
            }
            else if (tRange.RangeType == RangeType.Sphere)
            {
                Vector3 n = tRange.ShapeNucleus;
                float r = tRange.ST1;
                float phi = UnityEngine.Random.Range(0, 2 * Mathf.PI);
                float theta = UnityEngine.Random.Range(0, Mathf.PI);
                float x = r * Mathf.Sin(theta) * Mathf.Cos(phi) + n.x;
                float y = r * Mathf.Sin(theta) * Mathf.Sin(phi) + n.y;
                float z = r * Mathf.Cos(theta) + n.z;
                res = new Vector3(x, y, z);
            }
            else
            {
                print("What is this range type bruh");
                return new Vector3(0, 0, 0);
            }

            return res;
        }
    }
    #endregion
}
