using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Env_door : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private int _interactionsLeft = 1;
    [SerializeField] private bool _animate = true;
    [SerializeField] private int _saveNumber = 0;
    [SerializeField] private string _saveTag = "Respawn";

    [Header("Optionnal Objects")]
    [SerializeField] private Light _light;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private GameObject _minimapCube;

    [Header("New Transform")]
    [SerializeField] private Vector3 _newPosition = Vector3.zero;
    [SerializeField] private Vector3 _newRotation = Vector3.zero;
    [SerializeField] private float _transformationSpeed = 0.1f;

    private SaveSystem _saveSystem;

    void Start()
    {
        _light = GetComponent<Light>();
        _audioSource = GetComponent<AudioSource>();
        _saveSystem = GameObject.FindGameObjectWithTag(_saveTag).GetComponent<SaveSystem>();
        if(_saveSystem.GetCurrentObjective() >= _saveNumber)
        {
            transform.position = _newPosition;
            _minimapCube.SetActive(false);
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

        StartCoroutine(NewTransform());
        _saveSystem.SetCurrentObjective(_saveNumber);
    }

    private IEnumerator NewTransform()
    {
        GameObject.FindGameObjectWithTag("Player").transform.GetComponent<Player_main>().CameraNoise();

        if (_animate)
        {
            while (Vector3.Distance(_newPosition, transform.localPosition) > 0.01f && Vector3.Distance(_newRotation, transform.eulerAngles) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.localPosition, _newPosition, _transformationSpeed * Time.deltaTime);
                transform.eulerAngles = Vector3.MoveTowards(transform.eulerAngles, _newRotation, _transformationSpeed * Time.deltaTime);

                yield return null;
            }
            _minimapCube.GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            transform.position = _newPosition;
            _minimapCube.GetComponent<MeshRenderer>().enabled = false;
        }
        _minimapCube.SetActive(false);
    }
}
