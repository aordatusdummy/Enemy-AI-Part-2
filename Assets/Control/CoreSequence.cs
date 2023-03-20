using System.Collections.Generic; // Used for defining and manipulating generic lists
using UnityEngine; // Used for accessing Unity Engine features, including ScriptableObjects
using System.Reflection; // Used for accessing the properties of a class at runtime

// CoreSequence is a ScriptableObject class used to create and manage sequences of EnemyCores
// The [CreateAssetMenu] attribute allows users to create new instances of CoreSequence in the Unity Editor
public class CoreSequence : ScriptableObject
{
    // A static list of EnemyCore objects that have been randomly generated
    public static List<EnemyCore> randomEnemyCores = new List<EnemyCore>();

    #region Randomize
    // This method generates a new, random EnemyCore object and adds it to the list of randomEnemyCores
    public static int CreateRandomEnemyCore()
    {
        // Create a new instance of the EnemyCore class
        EnemyCore EnemyCore = CreateInstance<EnemyCore>();

        // Get all properties of the EnemyCore class
        PropertyInfo[] properties = typeof(EnemyCore).GetProperties();

        // Loop through each property of the EnemyCore class
        foreach (PropertyInfo property in properties)
        {
            // If the property is writeable
            if (property.CanWrite)
            {
                // If the property is of type MasterAttribute
                if (property.PropertyType == typeof(MasterAttribute))
                {
                    // Create a new instance of the MasterAttribute class with random values
                    MasterAttribute ma = new MasterAttribute();
                    ma.StatBase = Random.Range(0f, 100f);
                    ma.StatCons = Random.Range(0f, 1f);
                    ma.StatMarg = Random.Range(0f, 1f);

                    // Set the value of the property to the new MasterAttribute instance
                    property.SetValue(EnemyCore, ma);
                }
                // If the property is of type bool
                else if (property.PropertyType == typeof(bool))
                {
                    // Set the value of the property to a random boolean value
                    property.SetValue(EnemyCore, Random.value > 0.5f);
                }
                // Add more property types if necessary
            }
        }

        // Add the new EnemyCore object to the list of randomEnemyCores and return its index
        randomEnemyCores.Add(EnemyCore);
        return randomEnemyCores.Count - 1;
    }
    #endregion
}
