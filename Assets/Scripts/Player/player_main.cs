using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

//using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;

public class Player_main : MonoBehaviour
{
    [Header("Player Variables")]
    [SerializeField, Range(0.01f, 10f)] private float _xSensitivity = 1f;
    [SerializeField, Range(0.01f, 10f)] private float _ySensitivity = 1f;
    [Range(1, 100)] public int _speed = 20;
    [Range(1f, 5f)] public float _sprintMultiplier = 2f;
    [SerializeField] private float _interactionDist;
    [SerializeField] private float _shootDist;

    [SerializeField] private float _bobbingSpeed;
    [SerializeField] private float _bobbingAmplitude;

    [SerializeField] private string _saveTag = "Respawn";

    [Header("Player Components")]
    [SerializeField] private Light _flashlight;
    [SerializeField] private Volume _globalVolume;
    [SerializeField] private GameObject _virtualCameraGameObject;
    [SerializeField] private GameObject _gun;

    [Header("Debug Variables - Don't assign anything here")]
    public bool gamePaused = false;
    public bool isInCinematic = false;
    public bool isBeingChased = false;
    public Transform moveToward = null;

    [SerializeField] private float _flashlightRange;
    [SerializeField] private bool _isInteracting;
    [SerializeField] private bool _hasInteracted;
    [SerializeField] private bool _isUsingFlashlight;
    [SerializeField] private bool _hasUsedFlashlight;
    [SerializeField] private bool _isShooting;
    [SerializeField] private bool _hasShot;
    [SerializeField] private bool _pressingMap;
    [SerializeField] private bool _hasPressedMap;
    [SerializeField] private bool _pressingMenu;
    [SerializeField] private bool _hasPressedMenu;
    [SerializeField] private Vector2 _nextMove;
    [SerializeField] private Vector2 _cameraRotation;
    [SerializeField] private float _currentBobbingTime;
    [SerializeField] private float _startingYCamPos;
    [SerializeField] private bool _hasGun;

    [Header("Debug Components - Don't assign anything here")]
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private CinemachineVirtualCamera _virtualCameraCinemachine;
    [SerializeField] private PauseMenuScript _pauseMenu;
    [SerializeField] private Player_inputs player_Inputs;
    [SerializeField] private Ennemy _ennemyScript;
    [SerializeField] private GunScript _gunScript;
    [SerializeField] private SaveSystem _saveSystem;

    [Header("Debug GV Profiles - Don't assign anything here")]
    [SerializeField] private ChromaticAberration _chromaticAberration;
    [SerializeField] private DepthOfField _depthOfField;

    void Start()
    {
        _saveSystem = GameObject.FindGameObjectWithTag(_saveTag).GetComponent<SaveSystem>();

        _rigidbody = GetComponent<Rigidbody>();
        _virtualCameraCinemachine = _virtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
        _pauseMenu = GetComponent<PauseMenuScript>();

        _flashlightRange = _flashlight.range;
        _startingYCamPos = _virtualCameraGameObject.transform.localPosition.y;

        _gunScript = _gun.GetComponent<GunScript>();

        if (_globalVolume.profile.TryGet(out ChromaticAberration chromaticAberration)) 
        { 
            _chromaticAberration = chromaticAberration;
        }
        if(_globalVolume.profile.TryGet(out DepthOfField depthOfField))
        {
            _depthOfField = depthOfField;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Reload();
    }

    public void Reload()
    {
        if (_saveSystem._gotGun)
        {
            _gunScript.DiscoverGun();
            _hasGun = true;
        }
    }

    void FixedUpdate()
    {
        GetInputs();
        if (isInCinematic) { MoveToward(); }
        else
        {
            Movements();
            CameraRotation();
            Interaction();
            Flashlight();
            Shoot();
            Map();
        }
        PlayerGlobalVolume();
        Menu();
    }

    void GetInputs()
    {
        _nextMove = player_Inputs.move;

        _isInteracting = player_Inputs .interact;

        //_isUsingFlashlight = player_Inputs.flashlight;

        _isShooting = player_Inputs.shoot;

        _pressingMap = player_Inputs.map;

        _pressingMenu = player_Inputs.menu;

        _cameraRotation.x = player_Inputs.cam.x * _xSensitivity;
        _cameraRotation.y = player_Inputs.cam.y * _ySensitivity;

        _currentBobbingTime = _nextMove == Vector2.zero ? 0 : _currentBobbingTime + _bobbingSpeed * Time.deltaTime;
    }

    private void Movements()
    {
        Vector3 direction = (_virtualCameraGameObject.transform.forward * _nextMove.x) + (_virtualCameraGameObject.transform.right * _nextMove.y);
        direction.y = 0;
        _rigidbody.velocity = direction * _speed * Time.deltaTime * 10;
        if(isBeingChased)
        {
            _rigidbody.velocity *= _sprintMultiplier;
        }
    }

    private void CameraRotation()
    {
        Vector3 currentEuler = _virtualCameraGameObject.transform.eulerAngles;
        float eulerX = -_cameraRotation.x / 10 + currentEuler.x;
        float eulerY =  _cameraRotation.y / 10 + currentEuler.y;

        if(eulerX > 89 && eulerX <=180)
        {
            eulerX = 89;
        }
        if (eulerX > 180 && eulerX < 271)
        {
            eulerX = 271;
        }

        _virtualCameraGameObject.transform.eulerAngles = new Vector3(eulerX, eulerY);

        float currentBobbingPos = (Mathf.Sin(_currentBobbingTime)*_bobbingAmplitude);
        _virtualCameraGameObject.transform.localPosition = new Vector3(0, _startingYCamPos - currentBobbingPos, 0);

        _flashlight.transform.localEulerAngles = new Vector3(-_cameraRotation.x / 10, _cameraRotation.y / 10);

    }

    private void Interaction()
    {
        if (!_hasInteracted && _isInteracting)
        {
            _hasInteracted = true;

            Transform hit = SendRay(_interactionDist, Color.red);

            if (hit == null) { return; }
            Env_interact test_Interact = hit.transform.GetComponent<Env_interact>();

            if (test_Interact == null) { return; }

            if (test_Interact.putPlayerInCinematic)
            {
                moveToward = test_Interact.IsInteracted(_gunScript.hasBullet);
                moveToward.position = new Vector3(moveToward.position.x, transform.position.y, moveToward.position.z);
                isInCinematic = true;
                _virtualCameraCinemachine.LookAt = hit;
                Invoke(nameof(noMoreCinematic), 1f);
            }

            switch (test_Interact.utility)
            {
                case Env_interact.Utility.Door:
                    break;

                case Env_interact.Utility.Gun:
                    _hasGun = true;
                    _gunScript.DiscoverGun();
                    break;

                case Env_interact.Utility.Ammo:
                    _gunScript.GetAmmo();
                    break;
            }

        }
        else if (_hasInteracted && !_isInteracting)
        {
            _hasInteracted = false;
        }
    }

    private void MoveToward()
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.position = Vector3.MoveTowards(_rigidbody.position, moveToward.position, _speed * Time.deltaTime);
    }

    private void noMoreCinematic()
    {
        isInCinematic = false;
        _virtualCameraCinemachine.LookAt = null;
    }

    private void Flashlight()
    {
        if(_isUsingFlashlight && !_hasUsedFlashlight && _flashlight.range != 0)
        {
            _flashlight.range = 0;
            _hasUsedFlashlight = true;
        }
        else if (_isUsingFlashlight && !_hasUsedFlashlight && _flashlight.range == 0)
        {
            _flashlight.range = _flashlightRange;
            _hasUsedFlashlight = true;
        }
        else if (!_isUsingFlashlight && _hasUsedFlashlight)
        {
            _hasUsedFlashlight = false;
        }
    }

    private void Shoot()
    {
        if (!_hasShot && _isShooting && _hasGun && _gunScript.hasBullet)
        {
            _hasShot = true;

            Transform hit = SendRay(_shootDist, Color.blue);
            if ( hit != null)
            {
                _gunScript.GunShot();
            }
            if (hit.gameObject.tag == "Monster")
            {
                Debug.Log("Stunning");
                Ennemy monster = hit.gameObject.GetComponentInParent<Ennemy>();
                StartCoroutine(monster.StunState());
            }
        }
        else if (_hasShot && !_isShooting)
        {
            _hasShot = false;
        }
    }

    private void Menu()
    {
        if (!_hasPressedMenu && _pressingMenu)
        {
            _hasPressedMenu = true;

            _pauseMenu.EscapeKey();

        }
        else if (_hasPressedMenu && !_pressingMenu)
        {
            _hasPressedMenu = false;
        }
    }

    private void Map()
    {
        if (!_hasPressedMap && _pressingMap)
        {
            _hasPressedMap = true;

            _pauseMenu.MapKey(true);
        }
        else if (_hasPressedMap && !_pressingMap)
        {
            _hasPressedMap = false;

            _pauseMenu.MapKey(false);
        }
    }


    public void CameraNoise()
    {
        _virtualCameraCinemachine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 2;
        Invoke(nameof(StopNoise), 2);
    }

    public void IsKilled()
    {
        _pauseMenu.GameOver();
    }

    private void StopNoise()
    {
        _virtualCameraCinemachine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
    }

    public void EnnemyGlobalVolume(Transform ennemy)
    {
        if(_ennemyScript == null) { _ennemyScript = ennemy.GetComponent<Ennemy>(); }

        float ennDistance = Vector3.Distance(transform.position, ennemy.position);

        _chromaticAberration.intensity.value = 1 - ennDistance /  _ennemyScript._roamRange;
    }

    private void PlayerGlobalVolume()
    {
        Transform ray = SendRay(100, Color.blue, 0.1f);
        float rayDistance = Vector3.Distance(transform.position, ray.position);

        _depthOfField.focusDistance.value = rayDistance;

    }

    private Transform SendRay(float _distance, Color color, float debugTime = 10f)
    {
        Vector3 rayOrigin = new Vector3(0.5f, 0.5f, 0f);

        Ray ray = Camera.main.ViewportPointToRay(rayOrigin);
        Debug.DrawRay(ray.origin, ray.direction * _distance, color, debugTime);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _distance))
        {
            return hit.transform;
        }
        else
        {
            return null;
        }
    }
}
