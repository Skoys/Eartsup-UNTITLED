using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField, Range(1, 501)] int FPS = 60;

    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = FPS;
    }
    void Start()
    {
        
    }

    void Update()
    {
        TargetFPS();
    }

    private void TargetFPS()
    {
        if (Application.targetFrameRate == 501 && Application.targetFrameRate != FPS)
        {
            Application.targetFrameRate = 999999;
        }
        else if (Application.targetFrameRate != FPS)
        {
            Application.targetFrameRate = FPS;
        }
    }
}
