using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScriptt : MonoBehaviour
{
    private int menuSlide = 0;

    [SerializeField] private GameObject startScreen;
    [SerializeField]  private GameObject mainMenu;
    [SerializeField] private GameObject filesMenu;
    [SerializeField] private GameObject settingsMenu;

    private Graphic[] startObjects;
    private Graphic[] mainObjects;
    private Graphic[] filesObjects;
    private Graphic[] settingsObjects;

    [SerializeField] string gameSceneName;

    void Start()
    {
        menuSlide = 0;

        startObjects = startScreen.GetComponentsInChildren<Graphic>();
        mainObjects = mainMenu.GetComponentsInChildren<Graphic>();
        filesObjects = filesMenu.GetComponentsInChildren<Graphic>();
        settingsObjects = settingsMenu.GetComponentsInChildren<Graphic>();

        IsStartShown(true);
        IsMainShown(false);
        IsFilesShown(false);
        IsParametersShown(false);

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    private void Update()
    {
        if (menuSlide == 0 && Input.anyKey) 
        { 
            menuSlide = 1;
            IsStartShown(false);
            IsMainShown(true);
            IsFilesShown(false);
            IsParametersShown(false);
        }
    }

    public void mainStartClick()
    {
        IsStartShown(false);
        IsMainShown(false);
        IsFilesShown(true);
        IsParametersShown(false);
    }
    public void mainContinueClick()
    {
        IsStartShown(false);
        IsMainShown(false);
        IsFilesShown(true);
        IsParametersShown(false);
    }
    public void mainSettingsClick()
    {
        IsStartShown(false);
        IsMainShown(false);
        IsFilesShown(false);
        IsParametersShown(true);
    }
    public void mainExitClick()
    {
        IsStartShown(false);
        IsMainShown(false);
        IsFilesShown(false);
        IsParametersShown(false);
        QuitGame();
    }
    public void filesOneClick()
    {
        IsStartShown(false);
        IsMainShown(false);
        IsFilesShown(false);
        IsParametersShown(false);
        LoadGame();
    }
    public void filesTwoClick()
    {
        IsStartShown(false);
        IsMainShown(false);
        IsFilesShown(false);
        IsParametersShown(false);
        LoadGame();
    }
    public void filesThreeClick()
    {
        IsStartShown(false);
        IsMainShown(false);
        IsFilesShown(false);
        IsParametersShown(false);
        LoadGame();
    }
    public void filesReturnClick()
    {
        IsStartShown(false);
        IsMainShown(true);
        IsFilesShown(false);
        IsParametersShown(false);
    }
    public void parametersReturnClick()
    {
        IsStartShown(false);
        IsMainShown(true);
        IsFilesShown(false);
        IsParametersShown(false);
    }


    private void IsStartShown(bool isShown)
    {
        foreach (Graphic obj in startObjects) { obj.gameObject.SetActive(isShown); }
    }
    private void IsMainShown(bool isShown)
    {
        foreach (Graphic obj in mainObjects) { obj.gameObject.SetActive(isShown); }
    }
    private void IsFilesShown(bool isShown)
    {
        foreach (Graphic obj in filesObjects) { obj.gameObject.SetActive(isShown); }
    }
    private void IsParametersShown(bool isShown)
    {
        foreach (Graphic obj in settingsObjects) { obj.gameObject.SetActive(isShown); }
    }

    private void LoadGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }
    private void QuitGame()
    {
        Application.Quit();
    }
}
