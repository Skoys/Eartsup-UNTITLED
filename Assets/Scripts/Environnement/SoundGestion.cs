using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundGestion : MonoBehaviour
{
    public AudioSource footsteps;
    public AudioSource flickingLights;
    public AudioSource music;

    public void ActivateAudio(AudioSource audioSource, bool activate)
    {
        if (activate)
        {
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }
    }

    public void VolumeAudio(AudioSource audioSource, float volume)
    {
        audioSource.volume = volume;
    }
}
