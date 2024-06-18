using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Env_door : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private int _interactionsLeft = 1;
    [SerializeField] private bool _rumble = true;
    [SerializeField] private int _saveNumber = 0;

    [Header("Optionnal Objects")]
    [SerializeField] private Light _light;
    [SerializeField] private Animation _animation;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private GameObject _minimapCube;

    private SaveSystem _saveSystem;

    void Start()
    {
        _light = GetComponent<Light>();
        _audioSource = GetComponent<AudioSource>();
        _animation = GetComponent<Animation>();
        _saveSystem = SaveSystem.instance;
        if(_saveSystem.GetCurrentObjective() >= _saveNumber)
        {
            _animation.Play();
            if (_minimapCube != null) { _minimapCube.SetActive(false); }
        }
    }

    public void ButtonBeenInteracted()
    {
        _interactionsLeft--;
        if (_interactionsLeft == 0) { OpenTheDoor(); }
    }

    private void OpenTheDoor()
    {
        if(_light != null) 
        { 
            _light.color = Color.green; 
        }

        if (_audioSource != null)
        {
            _audioSource.Play();
        }

        NewTransform();
        _saveSystem.SetCurrentObjective(_saveNumber);
    }

    private void NewTransform()
    {
        Debug.Log("Opening");
        if (_rumble)
        {
            GameObject.FindGameObjectWithTag("Player").transform.GetComponent<Player_main>().CameraNoise();
        }

        Debug.Log("Animating");
        _animation.Play();
        if (_minimapCube != null) 
        { 
            _minimapCube.GetComponent<MeshRenderer>().enabled = false;
            _minimapCube.SetActive(false);
        }
    }
}
