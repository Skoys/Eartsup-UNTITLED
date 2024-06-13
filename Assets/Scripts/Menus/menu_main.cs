using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScriptt : MonoBehaviour
{
    private int menuSlide = 0;

    [SerializeField] private GameObject startScreen;
    [SerializeField]  private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;

    private Graphic[] startObjects;
    private Graphic[] mainObjects;
    private Graphic[] settingsObjects;

    [SerializeField] private string gameSceneName;
    [SerializeField] private string _buttonMenu;

    [SerializeField] private Settings _settingsScript;

    void Start()
    {
        menuSlide = 0;

        startObjects = startScreen.GetComponentsInChildren<Graphic>();
        mainObjects = mainMenu.GetComponentsInChildren<Graphic>();
        settingsObjects = settingsMenu.GetComponentsInChildren<Graphic>();

        _settingsScript = Settings.instance;

        IsStartShown(true);
        IsMainShown(false);
        IsParametersShown(false);

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        SetSettings();
    }

    private void SetSettings()
    {
        foreach (Graphic obj in settingsObjects)
        {
            if (obj.name == "Fun Mode")
            {
                obj.GetComponent<Image>().fillCenter = _settingsScript.funMode;
            }
        }
    }


    private void Update()
    {
        if (menuSlide == 0 && Input.anyKey) 
        { 
            menuSlide = 1;
            IsStartShown(false);
            IsMainShown(true);
            IsParametersShown(false);
        }
    }

    public void mainStartClick()
    {
        IsStartShown(false);
        IsMainShown(false);
        IsParametersShown(false);
        StartGame();
    }
    public void mainContinueClick()
    {
        IsStartShown(false);
        IsMainShown(false);
        IsParametersShown(false);
        ContinueGame();
    }
    public void mainSettingsClick()
    {
        IsStartShown(false);
        IsMainShown(false);
        IsParametersShown(true);
    }
    public void mainExitClick()
    {
        IsStartShown(false);
        IsMainShown(false);
        IsParametersShown(false);
        QuitGame();
    }

    public void parametersFunModeClick()
    {
        _settingsScript.funMode = !_settingsScript.funMode;
        foreach (Graphic obj in settingsObjects)
        {
            if(obj.name == "Fun Mode")
            {
                obj.GetComponent<Image>().fillCenter = _settingsScript.funMode;
            }
        }
    }

    public void parametersReturnClick()
    {
        IsStartShown(false);
        IsMainShown(true);
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
    private void IsParametersShown(bool isShown)
    {
        foreach (Graphic obj in settingsObjects) { obj.gameObject.SetActive(isShown); }
    }


    private void StartGame()
    {
        Destroy(GameObject.FindGameObjectWithTag("Respawn"));
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    private void ContinueGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }
    private void QuitGame()
    {
        Application.Quit();
    }
}
