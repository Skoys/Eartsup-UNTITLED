using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_inputs : MonoBehaviour
{
    [SerializeField] PlayerInput  InputAction;

    public bool menu = false;
    public bool map = false;
    public bool interact = false;
    public bool shoot = false;
    public bool flashlight = false;
    public Vector2 cam = Vector2.zero;
    public Vector2 move = Vector2.zero;

    public void Flashlight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            flashlight = true;
        }
        else if (context.canceled)
        {
            flashlight = false;
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            interact = true;
        }
        else if (context.canceled)
        {
            interact = false;
        }
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            shoot = true;
        }
        else if (context.canceled)
        {
            shoot = false;
        }
    }

    public void Menu(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            menu = true;
        }
        else if (context.canceled)
        {
            menu = false;
        }
    }

    public void Map(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            map = true;
        }
        else if (context.canceled)
        {
            map = false;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void ViewX(InputAction.CallbackContext context)
    {
        cam.x = context.ReadValue<float>();
    }

    public void ViewY(InputAction.CallbackContext context)
    {
        cam.y = context.ReadValue<float>();
    }

}
