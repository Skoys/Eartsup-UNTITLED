using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class env_ending : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PauseMenuScript>().Ending();
        }
    }
}
