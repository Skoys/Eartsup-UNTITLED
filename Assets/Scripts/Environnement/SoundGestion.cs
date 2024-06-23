using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundGestion : MonoBehaviour
{
    [Header("Player")]
    public AudioSource footsteps;
    public AudioSource flickingLights;
    public AudioSource music;

    [Header("Monster")]
    public AudioSource screech;
    public AudioSource funScreech;
    public AudioSource teleportation;
    public AudioSource stun;
    public AudioSource death;
    public AudioSource randomSound;
    public AudioSource funRandomSound;

    [Header("Misc")]
    public AudioSource generator;
    public AudioSource freightElevator;
    public AudioSource ending;
    public AudioSource funEnding;
    public AudioSource uiClick;
    public AudioSource uiSwoosh;

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
