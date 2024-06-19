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
    [SerializeField] private GameObject[] _visuals;
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
    [SerializeField] private SaveSystem _saveSystem;

    public enum Utility
    {
        Door,
        Gun,
        Ammo
    }


    public void Start()
    {
        if(_cube != null) { _cube.GetComponent<Renderer>().material.color = _baseColor; }
        _saveSystem = SaveSystem.instance;

        _animation = GetComponent<Animation>();

        if (_minimap != null) { _minimapText = _minimap.GetComponent<TextMeshProUGUI>(); }

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
                    if (_cube != null) { _cube.GetComponent<Renderer>().material.color = Color.green; }
                    if (_minimap != null) { _minimap.gameObject.SetActive(false); }
                    _animation.Play();
                    gameObject.GetComponent<Collider>().enabled = false;
                }
                else if (_saveSystem.HasBeenInteracted(_idNumber))
                {
                    _minimapText.gameObject.GetComponent<MeshRenderer>().enabled = true;
                }
                break;

            case Utility.Gun:
                if (_saveSystem._gotGun)
                {
                    _animation.Play();
                    _interacted = true;
                    Interacted();
                    gameObject.GetComponent<Collider>().enabled = false;
                }
                break;
        }
    }

    public void IsItMyTurn(int currentObjective)
    {
        if(_visuals.Length == 0) {  return; }
        if (currentObjective + 1 == _saveNumber)
        {
            foreach(GameObject visual in _visuals)
            {
                visual.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
            }
        }
        else
        {
            foreach (GameObject visual in _visuals)
            {
                visual.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
            }
        }
    }

    public Vector3 IsInteracted(bool ammunition)
    {
        if(utility == Utility.Ammo && ammunition)
        {
            return Vector3.zero;
        }

        if (!_interacted)
        {
            _interacted = true;
            _animation.Play();
            Invoke(nameof(Interacted), 1f);
            return _playerMoveTo.transform.position;
        }

        return Vector3.zero;
    }

    private void Interacted()
    {
        if (_cube != null) { _cube.GetComponent<Renderer>().material.color = Color.green; }
        if (_minimap != null) { _minimap.gameObject.SetActive(false); }

        switch (utility)
        {
            case Utility.Door:
                _saveSystem.ObjectInteracted(_idNumber);
                gameObject.GetComponent<Collider>().enabled = false;
                foreach (Env_door door in _isAffecting) { door.ButtonBeenInteracted(); }
                break;

            case Utility.Gun:
                _saveSystem._gotGun = true;
                gameObject.GetComponent<Collider>().enabled = false;
                break;

            case Utility.Ammo:
                break;
        }
    }
}
