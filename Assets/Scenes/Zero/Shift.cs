using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Shift : MonoBehaviour
{
    public void ShiftScene(string name)
    {
        SceneManager.LoadScene(name);
    }
}
