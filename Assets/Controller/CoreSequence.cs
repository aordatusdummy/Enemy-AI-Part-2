using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

[CreateAssetMenu(fileName = "SequenceName", menuName = "ScriptableObjects/Sequence", order = 1)]
public class CoreSequence : ScriptableObject
{
    public static List<EnemyCore> randomEnemyCores = new List<EnemyCore>();

    #region Randomize
    public static int CreateRandomEnemyCore()
    {
        EnemyCore EnemyCore = CreateInstance<EnemyCore>();

        PropertyInfo[] properties = typeof(EnemyCore).GetProperties();
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
                    property.SetValue(EnemyCore, ma);
                    Debug.Log(ma.StatBase);
                }
                else if (property.PropertyType == typeof(bool))
                {
                    property.SetValue(EnemyCore, Random.value > 0.5f);
                }
                // Add more property types if necessary
            }
        }

        randomEnemyCores.Add(EnemyCore);
        return randomEnemyCores.Count-1;
    }
    #endregion
}
