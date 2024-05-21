using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class player_main : MonoBehaviour
{
    [Header("Player Parametres")]
    [SerializeField] GameObject vCamera;
    [SerializeField, Range(0.01f, 10f)] float xSensitivity = 1f;
    [SerializeField, Range(0.01f, 10f)] float ySensitivity = 1f;
    [Range(1, 100)] public int speed = 20;
    [Range(1f, 5f)] public float sprintMultiplier = 2f;
    [SerializeField] float interactionDist;
    [SerializeField] float shootDist;
    [SerializeField] Light lamp;

    [Header("Player Variables Visualisation")]
    [SerializeField] Vector2 move;
    public bool isCinematic;
    public bool isChased;
    public Transform moveToward;
    [SerializeField] bool interact;
    [SerializeField] bool hasInteracted;
    [SerializeField] bool flashlight;
    [SerializeField] bool hasflashed;
    [SerializeField] float range;
    [SerializeField] bool shoot;
    [SerializeField] bool hasShooted;
    [SerializeField] bool menu;
    [SerializeField] bool hasMenu;
    [SerializeField] Vector2 cam;
    [SerializeField] bool isGrounded;

    bool gamePaused = false;

    public player_inputs player_Inputs;

    [Header("Player Components")]
    [SerializeField] Rigidbody rb;
    [SerializeField] CinemachineVirtualCamera cinemachine;
    [SerializeField] PauseMenuScript pauseMenu;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cinemachine = vCamera.GetComponent<CinemachineVirtualCamera>();
        pauseMenu = GetComponent<PauseMenuScript>();

        range = lamp.range;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        GetInputs();
        if (isCinematic) { MoveToward(); }
        else
        {
            Move();
            Cam();
            Interaction();
            Flashlight();
            Shoot();
        }

        Menu();
    }

    void GetInputs()
    {
        move = player_Inputs.move;

        interact = player_Inputs .interact;

        flashlight = player_Inputs.flashlight;

        shoot = player_Inputs.shoot;

        menu = player_Inputs.menu;

        cam = player_Inputs.cam;
    }

    private void Move()
    {
        Vector3 direction = (vCamera.transform.forward * move.x) + (vCamera.transform.right * move.y);
        direction.y = 0;
        rb.velocity = direction * speed * Time.deltaTime * 10;
        if(isChased)
        {
            rb.velocity = rb.velocity * sprintMultiplier;
        }
    }

    private void Cam()
    {
        Vector3 _rotation = new Vector3(cam.x * xSensitivity / 10, -cam.y * ySensitivity / 10, 0);
        vCamera.transform.eulerAngles -= _rotation;
    }

    private void Interaction()
    {
        if (!hasInteracted && interact)
        {
            hasInteracted = true;

            Transform hit = SendRay(interactionDist, Color.red);
            if (hit == null) { return; }
            TEST_Interact test_Interact = hit.transform.GetComponent<TEST_Interact>();
            if (test_Interact != null)
            {
                Transform moveTo = test_Interact.IsInteracted();
                moveToward = moveTo;
                moveToward.position = new Vector3(moveToward.position.x, transform.position.y, moveToward.position.z);
                isCinematic = true;
                cinemachine.LookAt = hit;
            }

        }
        else if (hasInteracted && !interact)
        {
            hasInteracted = false;
        }
    }

    private void MoveToward()
    {
        rb.velocity = Vector3.zero;
        rb.position = Vector3.MoveTowards(rb.position, moveToward.position, speed * Time.deltaTime);
    }

    private void noMoreCinematic()
    {
        isCinematic = false;
        cinemachine.LookAt = null;
    }

    private void Flashlight()
    {
        if(flashlight && !hasflashed && lamp.range != 0)
        {
            lamp.range = 0;
            hasflashed = true;
        }
        else if (flashlight && !hasflashed && lamp.range == 0)
        {
            lamp.range = range;
            hasflashed = true;
        }
        else if (!flashlight && hasflashed)
        {
            hasflashed = false;
        }
    }

    private void Shoot()
    {
        if (!hasShooted && shoot)
        {
            hasShooted = true;

            Transform hit = SendRay(shootDist, Color.blue);
            if ( hit != null)
            {
                TEST_Shoot test_shoot = hit.transform.GetComponent<TEST_Shoot>();
                if (test_shoot != null)
                {
                    test_shoot.IsShot();
                }
            }

        }
        else if (hasShooted && !shoot)
        {
            hasShooted = false;
        }
    }

    private void Menu()
    {
        if (!hasMenu && menu)
        {
            hasMenu = true;

            pauseMenu.EscapeKey();

        }
        else if (hasMenu && !menu)
        {
            hasMenu = false;
        }
    }




    private Transform SendRay(float _distance, Color color)
    {
        Vector3 rayOrigin = new Vector3(0.5f, 0.5f, 0f);

        Ray ray = Camera.main.ViewportPointToRay(rayOrigin);
        Debug.DrawRay(ray.origin, ray.direction * _distance, color, 10f);

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
