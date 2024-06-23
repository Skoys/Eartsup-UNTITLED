using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class env_ending : MonoBehaviour
{
    [SerializeField] private SoundGestion soundGestion;
    [SerializeField] private Settings _settingsScript;

    private void Start()
    {
        _settingsScript = Settings.instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            soundGestion.ActivateAudio(soundGestion.generator, false);
            soundGestion.ActivateAudio(soundGestion.freightElevator, false);
            if (_settingsScript.funMode) { soundGestion.ActivateAudio(soundGestion.funEnding, true); }
            else { soundGestion.ActivateAudio(soundGestion.ending, true); }
            other.GetComponent<PauseMenuScript>().Ending();
        }
    }
}
