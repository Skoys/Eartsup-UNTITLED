using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TEST_Shoot : MonoBehaviour
{
    bool shot = false;
    [SerializeField] new Light light;

    public void Start()
    {
        light.color = Color.blue;
    }

    public void IsShot()
    {
        if (!shot)
        {
            light.color = Color.green;
            shot = true;
            Invoke(nameof(LightsOff), 5f);
        }
    }

    public void LightsOff()
    {
        light.color = Color.blue;
        shot = false ;
    }
}
