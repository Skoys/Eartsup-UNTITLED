using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Rendering.DebugUI;

public class Ennemy : MonoBehaviour
{

    [Header("Values")]
    [SerializeField] private float _speed = 1.5f;
    [SerializeField] private float _sprintSpeed = 3;
    [SerializeField] private float _attackSpeed = 2;
    [SerializeField] private int _maxLooseSec = 20;
    [SerializeField] private float _timeBtwRays = 0.5f;
    [SerializeField] private int _ligthLayer = 9;
    [SerializeField] private float  _stunTime = 2;

    [Header("Spheres Ranges")]
    [Range(1f, 100f)] public float _roamRange = 30;
    [SerializeField, Range(1f, 100f)] private float _playerSafeRange = 20;
    [SerializeField, Range(1f, 100f)] private float _dyingLightsRange = 20;
    [SerializeField, Range(1f, 100f)] private float _flickingLightsRange = 7.5f;
    [SerializeField, Range(1f, 100f)] private float _maxRange = 40;
    [SerializeField, Range(1f, 180f)] private float _maxVisionAngle = 45f;

    [Header("Debug Values - Don't assign anything here")]
    public MonsterState monsterState = MonsterState.Teleport;
    [SerializeField] Vector3 roamMoveTo = Vector3.zero;
    [SerializeField] private bool _seenPlayer = false;
    [SerializeField] private float _lastRays = 0;
    [SerializeField] private int _currentLooseSec = 0;
    [SerializeField] private int _currentTicks = 0;
    [SerializeField] private bool _canInteractWithLights = false;
    [SerializeField] private Color _playerLightColor;
    [SerializeField] private float _playerLightIntensity = 0;

    [Header("Debug Components - Don't assign anything here")] 
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Player_main _playerScript;
    [SerializeField] private int _playerLayer;
    [SerializeField] private LayerMask _playerLayerMask;
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private Collider[] _ligthsList;
    [SerializeField] private Collider[] _oldLigthsList;
    [SerializeField] private Light _monsterLight;
    [SerializeField] private Settings _settingsScript;

    public enum MonsterState
    {
        Inactive,
        Roam,
        Focus,
        Attack,
        Stun,
        Teleport
    }

    private void Awake()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _playerScript = _playerTransform.GetComponent<Player_main>();
        _playerLayer = _playerTransform.gameObject.layer;
        _playerLayerMask = 1 << _playerTransform.gameObject.layer;      // Switch the bit of the layer to make a layer mask. I dunno its chat GPT
        _playerLightIntensity = _playerTransform.GetComponentInChildren<Light>().intensity;
        _playerLightColor = _playerTransform.GetComponentInChildren<Light>().color;

        _navMeshAgent = GetComponent<NavMeshAgent>();

        _monsterLight = GetComponentInChildren<Light>();
        _monsterLight.color = Color.white;
        
        monsterState = MonsterState.Inactive;

        _settingsScript = Settings.instance;

        if (_settingsScript.funMode)
        {
            foreach (Transform child in gameObject.transform)
            {
                switch (child.name)
                {
                    case "Graphic":
                        child.gameObject.SetActive(false);
                        break;
                    case "Fun":
                        child.gameObject.SetActive(true);
                        break;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (!_seenPlayer)
        {
            SearchPlayer();
        }

        CanKillPlayer();

        switch (monsterState)
        {
            case MonsterState.Inactive:
                CheckForActivation();
                break;
            case MonsterState.Roam:
                RoamState();
                break;
            case MonsterState.Focus:
                FocusState();
                break;
            case MonsterState.Attack:
                AttackState();
                break;
            case MonsterState.Teleport:
                TeleportState();
                break;
            case MonsterState.Stun:
                break;
        }

        if(EveryXTicks(_currentTicks, 5) && _canInteractWithLights) 
        { 
            LightInteraction();
            _playerScript.EnnemyGlobalVolume(transform);
        }

        _currentTicks++;
    }

    private void CheckForActivation()
    {
        if (!Physics.CheckSphere(transform.position, _roamRange, _playerLayerMask))
        {
            Debug.Log("Monster - Starting");
            _navMeshAgent.speed = _speed;
            ChangeState(MonsterState.Teleport);
        }
    }

    private void RoamState()
    {
        if (Physics.CheckSphere(transform.position, _roamRange, _playerLayerMask))
        {
            _navMeshAgent.SetDestination(roamMoveTo);
            if (Vector3.Distance(transform.position, roamMoveTo) <= 1)
            {
                FindNewRoamPoint();
            }
        }
        else 
        {
            Debug.Log("Monster - Teleporting");
            ChangeState(MonsterState.Teleport);
            _navMeshAgent.speed = _sprintSpeed;
        }
    }

    private void FindNewRoamPoint()
    {
        roamMoveTo = Random.insideUnitSphere * _roamRange;

        roamMoveTo += transform.position;
        NavMesh.SamplePosition(roamMoveTo, out NavMeshHit hit, _roamRange, 1);
        roamMoveTo = hit.position;
    }

    private void FocusState()
    {
        transform.LookAt(_playerTransform);

        if (Physics.CheckSphere(transform.position, _roamRange / 2, _playerLayerMask) && monsterState != MonsterState.Stun)
        {
            Debug.Log("Monster - Unfocussing");
            ChangeState(MonsterState.Roam);
            FindNewRoamPoint();
            _navMeshAgent.speed = _speed;
        }

        if (!Physics.CheckSphere(transform.position, _maxRange, _playerLayerMask))
        {
            Debug.Log("Monster - Teleporting from focus : " + Vector3.Distance(transform.position, _playerTransform.position)+ " : " + Physics.CheckSphere(transform.position, _maxRange, _playerLayerMask));
            ChangeState(MonsterState.Teleport);
        }

        _navMeshAgent.SetDestination(_playerTransform.position);
    }

    private void AttackState()
    {
        transform.LookAt(_playerTransform);
        NavMesh.SamplePosition(_playerTransform.position, out NavMeshHit hit, _roamRange, 1);
        Vector3 hitPos = hit.position;
        if (Vector3.Distance(_playerTransform.position, hitPos) < 5)
        {
            _navMeshAgent.SetDestination(_playerTransform.position);
        }
        ChangeState(MonsterState.Teleport);
    }

    private bool CheckPlayer(Color color)
    {
        if (Time.realtimeSinceStartup - _lastRays >= _timeBtwRays)
        {
            _lastRays = Time.realtimeSinceStartup;

            Ray ray = new Ray(transform.position, _playerTransform.position - transform.position);
            Debug.DrawRay(ray.origin, ray.direction * Vector3.Distance(transform.position, _playerTransform.position), color, 10);

            if (Quaternion.Angle(transform.rotation, Quaternion.LookRotation(ray.direction)) > _maxVisionAngle)
            {
                return false;
            }

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, _roamRange))
            {
                if (hit.transform.gameObject.layer == _playerLayer)
                {
                    Debug.Log("Monster - Player in sight");
                    return true;
                }
            }
        }
        return false;
    }

    private void TeleportState()
    {
        Vector3 tpTo = Random.insideUnitSphere * _roamRange + _playerTransform.position;

        NavMeshHit hit;
        NavMesh.SamplePosition(tpTo, out hit, _maxRange, 1);
        float distanceToPlayer = Vector3.Distance(hit.position, _playerTransform.position);

        if (distanceToPlayer > _playerSafeRange && _maxRange > distanceToPlayer)
        {
            Debug.Log("Monster - Focussing");
            _navMeshAgent.Warp(hit.position);

            ChangeState(MonsterState.Focus);
        }
    }

    

    private void ChangeState(MonsterState state)
    {
        monsterState = state;
    }

    private void AttemptToFlee()
    {
        if (CheckPlayer(Color.red))
        {
            Debug.Log("Monster - Attempt failed");
            _currentLooseSec = 0;
        }
        else
        {
            Debug.Log("Monster - Attempt nbr: " + _currentLooseSec);
            _currentLooseSec += 1;
        }

        if (_currentLooseSec > _maxLooseSec)
        {
            ChangeState(MonsterState.Teleport);
            _currentLooseSec = 0;
            _seenPlayer = false;
            _playerScript.isBeingChased = false;
            return;
        }

        Invoke("AttemptToFlee", 1);
    }

    private void SearchPlayer()
    {
        if (CheckPlayer(Color.blue) && monsterState != MonsterState.Stun)
        {
            Debug.Log("Monster - Attacking");
            ChangeState(MonsterState.Attack);
            _navMeshAgent.speed = _attackSpeed;
            _seenPlayer = true;
            _playerScript.isBeingChased = true;
            AttemptToFlee();
        }
    }

    private void CanKillPlayer()
    {
        if (Physics.CheckSphere(transform.position, 1.5f, _playerLayerMask) && monsterState != MonsterState.Stun)
        {
            Debug.Log("Monster - Kill Player");
            _playerScript.IsKilled();
            _navMeshAgent.enabled = false;
            monsterState = MonsterState.Stun;
            transform.position = Vector3.zero;
        }
    }

    private void LightInteraction()
    {

        _ligthsList = Physics.OverlapSphere(transform.position, _flickingLightsRange, LayerMask.GetMask(LayerMask.LayerToName(_ligthLayer)));

        foreach(Collider oldLight in _oldLigthsList)
        {
            if (!_ligthsList.Contains(oldLight))
            {
                oldLight.GetComponent<Light>().enabled = true;
                oldLight.transform.GetChild(0).gameObject.SetActive(true);
                oldLight.GetComponent<Light>().color = _playerLightColor;
                if (oldLight.name == "Flashlight")
                {
                    oldLight.GetComponent<Light>().intensity = _playerLightIntensity;
                }
            }
        }

        foreach (Collider light in _ligthsList)
        {
            Light lumi = light.GetComponent<Light>();
            float distance = Vector3.Distance(transform.position, light.transform.position);

            if (distance <= _dyingLightsRange && light.name != "Flashlight")
            {
                lumi.enabled = false;
                //light.transform.GetChild(0).gameObject.SetActive(false);
            }
            else if (distance <= _flickingLightsRange && light.name != "Flashlight")
            {
                bool random = Random.value > 0.5f;

                lumi.enabled = random;
                light.transform.GetChild(0).gameObject.SetActive(random);
                float clampedDistance = Mathf.Clamp(distance, _dyingLightsRange, _flickingLightsRange) / _flickingLightsRange - _dyingLightsRange / _flickingLightsRange;
                //lumi.color = Color.Lerp(_playerLightColor, Color.red, 1 - clampedDistance);
                
            }
            else if(light.name == "Flashlight")
            {
                float random = Random.Range(_playerLightIntensity / 2, _playerLightIntensity);

                lumi.intensity = random;
                lumi.color = Color.red;
            }
        }

        _oldLigthsList = _ligthsList;
    }





    public bool EveryXTicks(int currentTick, int eachTicks)
    {
        return (currentTick % eachTicks) == 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "NoLightMagic")
        {
            _canInteractWithLights = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "NoLightMagic")
        {
            _canInteractWithLights = true;
        }
    }

    public IEnumerator StunState()
    {
        ChangeState(MonsterState.Stun);
        _navMeshAgent.SetDestination(transform.position);
        _monsterLight.color = Color.red;
        yield return new WaitForSeconds(_stunTime);
        ChangeState(MonsterState.Focus);
        _monsterLight.color = Color.white;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, _roamRange);
        Gizmos.DrawWireSphere(transform.position, _roamRange / 2);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1.5f);

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, _maxRange);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, _playerSafeRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _flickingLightsRange);
        Gizmos.DrawWireSphere(transform.position, _dyingLightsRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.forward * _roamRange);
        Vector3 offsetDirection = Quaternion.Euler(0, _maxVisionAngle, 0) * transform.forward;
        Gizmos.DrawLine(transform.position,  transform.position +  offsetDirection * _roamRange);
        offsetDirection = Quaternion.Euler(0 , -_maxVisionAngle, 0) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + offsetDirection * _roamRange);
    }
}