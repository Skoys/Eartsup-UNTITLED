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
    [SerializeField] private SoundGestion sound;

    [Header ("Sliders")]
    [SerializeField] private Slider _masterVolume;
    [SerializeField] private Slider _musicVolume;
    [SerializeField] private Slider _ambianceVolume;
    [SerializeField] private Slider _effectVolume;

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

        _masterVolume.value = 0f;
        _masterVolume.onValueChanged.AddListener(MasterVolume);
        _musicVolume.value = 0f;
        _musicVolume.onValueChanged.AddListener(MusicVolume);
        _ambianceVolume.value = 0f;
        _ambianceVolume.onValueChanged.AddListener(AmbianceVolume);
        _effectVolume.value = -15f;
        _effectVolume.onValueChanged.AddListener(EffectVolume);

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
            sound.ActivateAudio(sound.uiSwoosh, true);
            menuSlide = 1;
            IsStartShown(false);
            IsMainShown(true);
            IsParametersShown(false);
        }
    }

    public void mainStartClick()
    {
        sound.ActivateAudio(sound.uiClick, true);
        IsStartShown(false);
        IsMainShown(false);
        IsParametersShown(false);
        StartGame();
    }
    public void mainContinueClick()
    {
        sound.ActivateAudio(sound.uiClick, true);
        IsStartShown(false);
        IsMainShown(false);
        IsParametersShown(false);
        ContinueGame();
    }
    public void mainSettingsClick()
    {
        sound.ActivateAudio(sound.uiClick, true);
        IsStartShown(false);
        IsMainShown(false);
        IsParametersShown(true);
    }
    public void mainExitClick()
    {
        sound.ActivateAudio(sound.uiClick, true);
        IsStartShown(false);
        IsMainShown(false);
        IsParametersShown(false);
        QuitGame();
    }

    public void parametersFunModeClick()
    {
        sound.ActivateAudio(sound.uiClick, true);
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
        sound.ActivateAudio(sound.uiClick, true);
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


    private void MasterVolume(float newValue)
    {
        _settingsScript._masterVolume = newValue;
        _settingsScript.audioMixer.SetFloat("Master", newValue);
    }
    private void MusicVolume(float newValue)
    {
        _settingsScript._musicVolume = newValue;
        _settingsScript.audioMixer.SetFloat("Music", newValue);
    }
    private void AmbianceVolume(float newValue)
    {
        _settingsScript._ambianceVolume = newValue;
        _settingsScript.audioMixer.SetFloat("Ambiance", newValue);
    }
    private void EffectVolume(float newValue)
    {
        _settingsScript._effectsVolume = newValue;
        _settingsScript.audioMixer.SetFloat("Effects", newValue);
    }
}
