using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static Settings instance;

    public bool funMode = false;

    [Range(0.01f, 10f)] public float _xSensitivity = 1f;
    [Range(0.01f, 10f)] public float _ySensitivity = 1f;
    [Range(0f, 100f)] public float _sound = 100f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}