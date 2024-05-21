using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuScript : MonoBehaviour
{
    private bool pause = false;
    private int currentMenu = 0;

    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject settingsMenu;

    [SerializeField] private player_main player;

    private Graphic[] pauseObjects;
    private Graphic[] settingsObjects;

    [SerializeField] string menuSceneName;

    void Start()
    {
        pause = false;

        pauseObjects = pauseScreen.GetComponentsInChildren<Graphic>();
        settingsObjects = settingsMenu.GetComponentsInChildren<Graphic>();

        player = gameObject.GetComponent<player_main>();

        IsPauseShown(false);
        IsParametersShown(false);
    }

    public void EscapeKey()
    {
        if (!pause) 
        { 
            PauseGame(true); 
            currentMenu = 0;
            IsPauseShown(true);
            IsParametersShown(false);
            player.isCinematic = true;
            player.moveToward = player.transform;
        }
        else
        {
            if (currentMenu == 0) 
            { 
                PauseGame(false); 
                IsPauseShown(false);
                IsParametersShown(false);
                player.isCinematic = false;
            }
            else
            {
                IsPauseShown(true);
                IsParametersShown(false);
                currentMenu = 0;
            }
        }
    }


    public void pauseResumeClick()
    {
        IsPauseShown(false);
        IsParametersShown(false);
        player.isCinematic = false;
        PauseGame(false);
    }
    public void pauseSettingsClick()
    {
        IsPauseShown(false);
        IsParametersShown(true);
        currentMenu = 1;
    }
    public void pauseMainMenuClick()
    {
        IsPauseShown(false);
        IsParametersShown(false);
        LoadGame();
    }
    public void settingsResumeClick()
    {
        IsPauseShown(true);
        IsParametersShown(false);
        currentMenu = 0;
    }


    private void IsPauseShown(bool isShown)
    {
        foreach (Graphic obj in pauseObjects) { obj.gameObject.SetActive(isShown); }
    }
    private void IsParametersShown(bool isShown)
    {
        foreach (Graphic obj in settingsObjects) { obj.gameObject.SetActive(isShown); }
    }

    private void LoadGame()
    {
        SceneManager.LoadScene(menuSceneName);
    }
    private void PauseGame(bool isPaused)
    {
        if (isPaused) 
        { 
            pause = true;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else 
        { 
            pause = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
