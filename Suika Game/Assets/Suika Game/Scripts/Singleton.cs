using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton <T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance;

    public static T instance
    {
        get
        {
            if (Instance == null)
            {
                Instance = FindObjectOfType<T>();
            }
            return Instance;
        }
    }
}
