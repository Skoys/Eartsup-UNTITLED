using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class env_ending : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something in the way");
        if (other.tag == "Player")
        {
            Debug.Log("Player Ending");
            other.GetComponent<Player_main>().Ending();
        }
    }
}
