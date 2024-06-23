using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class Settings : MonoBehaviour
{
    public static Settings instance;

    public bool funMode = false;

    public AudioMixer audioMixer;

    [Range(-80f, 20)] public float _masterVolume = 0f;
    [Range(-80f, 20)] public float _musicVolume = 0f;
    [Range(-80f, 20)] public float _ambianceVolume = 0f;
    [Range(-80f, 20)] public float _effectsVolume = 0f;
    [Range(0.01f, 10)] public float _sensitivity = 1f;

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