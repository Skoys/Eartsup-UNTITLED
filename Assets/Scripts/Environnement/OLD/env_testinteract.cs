using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TEST_Interact : MonoBehaviour
{
    bool interacted = false;
    public bool infinite = false;

    [SerializeField] new Light light;
    [SerializeField] GameObject playerMoveTo;

    public void Start()
    {
        light.color = Color.red;
    }

    public Transform IsInteracted()
    {
        if (!interacted)
        {
            interacted = true;
            Invoke(nameof(LightsOn), 1f);
            if (!infinite) { Invoke(nameof(LightsOff), 5f); }
            return playerMoveTo.transform;
        }

        return null;
    }

    private void LightsOn()
    {
        light.color = Color.green;
    } 

    private void LightsOff()
    {
        light.color = Color.red;
        interacted = false ;
    }
}
