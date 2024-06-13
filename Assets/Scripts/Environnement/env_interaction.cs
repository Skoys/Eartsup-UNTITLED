using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Env_interact : MonoBehaviour
{
    [Header("General")]
    public Utility utility = Utility.Door;
    [SerializeField] private GameObject _minimap;
    [SerializeField] private GameObject _cube;
    [SerializeField] private Color _baseColor = Color.red;
    public bool putPlayerInCinematic = true;
    [SerializeField] private Animation _animation;

    [Header("Door Button")]
    [SerializeField] private int _idNumber;
    [SerializeField] private int _saveNumber;
    [SerializeField] private GameObject _playerMoveTo;
    [SerializeField] private Env_door[] _isAffecting;

    [Header("Debug")]
    [SerializeField] private bool _interacted = false;
    [SerializeField] private TextMeshProUGUI _minimapText;
    [SerializeField] private string _saveTag = "Respawn";
    [SerializeField] private SaveSystem _saveSystem;

    public enum Utility
    {
        Door,
        Gun,
        Ammo
    }


    public void Start()
    {
        _cube.GetComponent<Renderer>().material.color = _baseColor;
        _saveSystem = GameObject.FindGameObjectWithTag(_saveTag).GetComponent<SaveSystem>();

        _animation = GetComponent<Animation>();

        _minimapText = _minimap.GetComponent<TextMeshProUGUI>();

        Reload();
    }

    public void Reload()
    {
        switch (utility)
        {
            case Utility.Door:
                if (_saveSystem.GetCurrentObjective() >= _saveNumber)
                {
                    _interacted = true;
                    _cube.GetComponent<Renderer>().material.color = Color.green;
                    _minimap.gameObject.SetActive(false);
                }
                else if (_saveSystem.HasBeenInteracted(_idNumber))
                {
                    _minimapText.gameObject.GetComponent<MeshRenderer>().enabled = true;
                }
                break;

            case Utility.Gun:
                if (_saveSystem._gotGun)
                {
                    _interacted = true;
                    Interacted();
                }
                break;
        }
    }

    public Transform IsInteracted(bool ammunition)
    {
        if(utility == Utility.Ammo && ammunition)
        {
            return null;
        }

        if (!_interacted)
        {
            _interacted = true;
            _animation.Play("OpenLever");
            Invoke(nameof(Interacted), 1f);
            return _playerMoveTo.transform;
        }

        return null;
    }

    private void Interacted()
    {
        _cube.GetComponent<Renderer>().material.color = Color.green;
        _minimap.gameObject.SetActive(false);

        switch (utility)
        {
            case Utility.Door:
                _saveSystem.ObjectInteracted(_idNumber);
                foreach (Env_door door in _isAffecting) { door.ButtonBeenInteracted(); }
                break;

            case Utility.Gun:
                _saveSystem.GotGun();
                break;

            case Utility.Ammo:
                break;
        }
    }
}
