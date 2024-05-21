using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class Ennemy : MonoBehaviour
{
    [Header("IA")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform player;
    [SerializeField] player_main playerScript;
    private player_main playerController;
    [SerializeField] LayerMask playerMask;
    [SerializeField] int playerLayer;

    [Header("Attacking")]
    [SerializeField] float speed = 1.5f;
    [SerializeField] float sprintSpeed = 3;
    [SerializeField] float attackSpeed = 2;
    [SerializeField] int maxLooseSec = 20;
    [SerializeField] int currentLooseSec = 0;
    [SerializeField] bool playerInAttackRange = false;
    [SerializeField] float timeBtwRays = 0.5f;
    [SerializeField] float lastRays = 0;

    [Header("States")]
    [SerializeField] float RoamRange = 30;
    [SerializeField] float playerSafeRange = 30;
    [SerializeField] float maxRange = 40;
    [SerializeField] bool seenPlayer = false;
    [SerializeField] MonsterState monsterState = MonsterState.Roam;
    private enum MonsterState
    {
        Inactive,
        Roam,
        Focus,
        Attack,
        Stun,
        Teleport
    }

    [SerializeField] Vector3 roamMoveTo = Vector3.zero;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerScript = player.GetComponent<player_main>();
        agent = GetComponent<NavMeshAgent>();

        playerSafeRange = RoamRange / 2 + RoamRange - RoamRange / 2;
        attackSpeed = playerScript.speed * playerScript.sprintMultiplier;
    }

    private void Update()
    {
        if (!seenPlayer)
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
            case MonsterState.Stun:
                StunState();
                break;
            case MonsterState.Teleport:
                TeleportState();
                break;
        }
    }

    private void CheckForActivation()
    {
        if (!Physics.CheckSphere(transform.position, RoamRange, playerMask))
        {
            agent.speed = speed;
            ChangeState(MonsterState.Roam);
        }
    }

    private void RoamState()
    {
        if (Physics.CheckSphere(transform.position, RoamRange, playerMask))
        {
            agent.SetDestination(roamMoveTo);
            if (Vector3.Distance(transform.position, roamMoveTo) <= 1)
            {
                FindNewRoamPoint();
            }
        }
        else
        {
            Debug.Log("Monster - Teleporting");
            ChangeState(MonsterState.Teleport);
            agent.speed = sprintSpeed;
        }
    }

    private void FindNewRoamPoint()
    {
        roamMoveTo = Random.insideUnitSphere * RoamRange;

        roamMoveTo += transform.position;
        NavMesh.SamplePosition(roamMoveTo, out NavMeshHit hit, RoamRange, 1);
        roamMoveTo = hit.position;
    }

    private void FocusState()
    {
        transform.LookAt(player);
        if (Physics.CheckSphere(transform.position, RoamRange / 2, playerMask))
        {
            Debug.Log("Monster - Unfocussing");
            FindNewRoamPoint();
            ChangeState(MonsterState.Roam);
            agent.speed = speed;
        }

        agent.SetDestination(player.position);
    }

    private void AttackState()
    {
        transform.LookAt(player);
        agent.SetDestination(player.position);
    }

    private bool CheckPlayer()
    {
        if (playerInAttackRange && Time.realtimeSinceStartup - lastRays >= timeBtwRays)
        {
            Ray ray = new Ray(transform.position, player.position - transform.position);
            Debug.DrawRay(ray.origin, ray.direction * Vector3.Distance(transform.position, player.position), Color.red, 10);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, RoamRange))
            {
                if (hit.transform.gameObject.layer == playerLayer)
                {
                    Debug.Log("Monster - Player in sight");
                    return true;
                }
            }
        }
        return false;
    }

    private void StunState()
    {

    }

    private void TeleportState()
    {
        Vector3 tpTo = Random.insideUnitSphere * RoamRange;

        tpTo += player.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(tpTo, out hit, playerSafeRange, 1);
        if (Vector3.Distance(hit.position, player.position)> playerSafeRange &&  maxRange > Vector3.Distance(hit.position, player.position))
        {
            Debug.Log("Monster - Focussing");
            transform.position = hit.position;
            ChangeState(MonsterState.Focus);
        }
    }

    private void ChangeState(MonsterState state)
    {
        monsterState = state;
    }

    private void AttemptToFlee()
    {
        if (CheckPlayer())
        {
            Debug.Log("Monster - Attempt failed");
            currentLooseSec = 0;
        }
        else
        {
            Debug.Log("Monster - Attempt nbr: " + currentLooseSec);
            currentLooseSec += 1;
        }

        if (currentLooseSec > maxLooseSec)
        {
            ChangeState(MonsterState.Teleport);
            currentLooseSec = 0;
            seenPlayer = false;
            playerScript.isChased = false;
            return;
        }

        Invoke("AttemptToFlee", 1);
    }

    private void SearchPlayer()
    {
        if (CheckPlayer())
        {
            Debug.Log("Monster - Attacking");
            ChangeState(MonsterState.Attack);
            agent.speed = attackSpeed;
            seenPlayer = true;
            playerScript.isChased = true;
            AttemptToFlee();
        }
    }

    private void CanKillPlayer()
    {
        if (Physics.CheckSphere(transform.position, 1.5f, playerMask))
        {
            Debug.Log("Monster - Kill Player");
            SceneManager.LoadScene("MonstreTest");
        }
    }





    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == playerLayer)
        {
            Debug.Log("Monster - Player in attack range");
            playerInAttackRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == playerLayer)
        {
            Debug.Log("Monster - Player no longer in attack range");
            playerInAttackRange = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, RoamRange);
        Gizmos.DrawWireSphere(transform.position, RoamRange / 2);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1.5f);
    }
}