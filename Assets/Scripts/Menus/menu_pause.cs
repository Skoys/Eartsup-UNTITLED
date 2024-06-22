using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Cinemachine.CinemachineOrbitalTransposer;

public class PauseMenuScript : MonoBehaviour
{
    private bool _pause = false;
    public bool _ending = false;
    private int _currentMenu = 0;

    [SerializeField] private GameObject _pauseScreen;
    [SerializeField] private GameObject _settingsMenu;
    [SerializeField] private GameObject _minimapImage;
    [SerializeField] private GameObject _mapImage;
    [SerializeField] private GameObject _gameOverScreen;
    [SerializeField] private GameObject _crosshair;

    [SerializeField] private Player_main _player;

    [SerializeField] private string _menuSceneName;

    [Header("Debug")]
    [SerializeField] private Graphic[] _pauseObjects;
    [SerializeField] private Graphic[] _settingsObjects;
    [SerializeField] private Graphic[] _minimapObjects;
    [SerializeField] private Graphic[] _mapObjects;
    [SerializeField] private Graphic[] _gameOverObjects;
    [SerializeField] private Animation _animation;

    void Awake()
    {
        _pause = false;

        _pauseObjects = _pauseScreen.GetComponentsInChildren<Graphic>();
        _settingsObjects = _settingsMenu.GetComponentsInChildren<Graphic>();
        _minimapObjects = _minimapImage.GetComponentsInChildren<Graphic>();
        _mapObjects = _mapImage.GetComponentsInChildren<Graphic>();
        _gameOverObjects = _gameOverScreen.GetComponentsInChildren<Graphic>();

        _player = gameObject.GetComponent<Player_main>();

        IsParameterShown(false, _pauseObjects);
        IsParameterShown(false, _settingsObjects);
        IsParameterShown(true, _minimapObjects);
        IsParameterShown(false, _mapObjects);
        IsParameterShown(false, _gameOverObjects);
        _crosshair.SetActive(true);

        Time.timeScale = 1;

        GameObject[] Respawns = GameObject.FindGameObjectsWithTag(gameObject.tag);
        if (Respawns.Length > 1) { Destroy(gameObject); }
    }

    public void EscapeKey()
    {
        if (!_pause && !_ending)
        {
            PauseGame(true);
            _currentMenu = 0;
            IsParameterShown(true, _pauseObjects);
            IsParameterShown(false, _settingsObjects);
            IsParameterShown(false, _minimapObjects);
            IsParameterShown(false, _mapObjects);
            _player.isInCinematic = true;
            _player.moveToward = _player.transform.position;
        }
        else
        {
            if (_currentMenu == 0)
            {
                PauseGame(false);
                IsParameterShown(false, _pauseObjects);
                IsParameterShown(false, _settingsObjects);
                IsParameterShown(true, _minimapObjects);
                IsParameterShown(false, _mapObjects);
                _player.isInCinematic = false;
            }
            else if (_currentMenu == 1)
            {
                IsParameterShown(true, _pauseObjects);
                IsParameterShown(false, _settingsObjects);
                IsParameterShown(false, _minimapObjects);
                IsParameterShown(false, _mapObjects);
                _currentMenu = 0;
            }
        }
    }

    public void MapKey(bool isPressed)
    {
        if (!_pause && isPressed)
        {
            IsParameterShown(false, _minimapObjects);
            IsParameterShown(true, _mapObjects);
        }
        else
        {
            IsParameterShown(true, _minimapObjects);
            IsParameterShown(false, _mapObjects);
        }
    }

    public void PauseResumeClick()
    {
        IsParameterShown(false, _pauseObjects);
        IsParameterShown(false, _settingsObjects);
        IsParameterShown(true, _minimapObjects);
        IsParameterShown(false, _mapObjects);
        _player.isInCinematic = false;
        PauseGame(false);
    }
    public void PauseSettingsClick()
    {
        IsParameterShown(false, _pauseObjects);
        IsParameterShown(true, _settingsObjects);
        IsParameterShown(false, _minimapObjects);
        IsParameterShown(false, _mapObjects);
        _currentMenu = 1;
    }
    public void PauseMainMenuClick()
    {
        LoadMenu();
    }
    public void SettingsResumeClick()
    {
        IsParameterShown(true, _pauseObjects);
        IsParameterShown(false, _settingsObjects);
        IsParameterShown(false, _minimapObjects);
        IsParameterShown(false, _mapObjects);
        _currentMenu = 0;
    }
    public void GameOver()
    {
        IsParameterShown(false, _pauseObjects);
        IsParameterShown(false, _settingsObjects);
        IsParameterShown(false, _minimapObjects);
        IsParameterShown(false, _mapObjects);
        IsParameterShown(true, _gameOverObjects);
        _currentMenu = 2;
        PauseGame(true);
    }
    public void GetAllAsset(bool areEnabled)
    {
        IsParameterShown(areEnabled, _pauseObjects);
        IsParameterShown(areEnabled, _settingsObjects);
        IsParameterShown(areEnabled, _minimapObjects);
        IsParameterShown(areEnabled, _mapObjects);
        IsParameterShown(areEnabled, _gameOverObjects);
    }
    public void GameOverRetryClick()
    {
        LoadGame();
    }
    public void GameOverMenuClick()
    {
        LoadMenu();
    }

    private void IsParameterShown(bool isShown, Graphic[] _parametersObjects)
    {
        foreach (Graphic obj in _parametersObjects)
        {
            obj.gameObject.SetActive(isShown);
            if (obj.name == "TabLight")
            {
                obj.gameObject.SetActive(false);
            }
        }
    }

    private void LoadMenu()
    {
        SceneManager.LoadScene(_menuSceneName);
        IsParameterShown(false, _pauseObjects);
        IsParameterShown(false, _settingsObjects);
        IsParameterShown(false, _minimapObjects);
        IsParameterShown(false, _mapObjects);
        IsParameterShown(false, _gameOverObjects);
        _crosshair.SetActive(false);
        PauseGame(false);
    }
    private void LoadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        IsParameterShown(false, _pauseObjects);
        IsParameterShown(false, _settingsObjects);
        IsParameterShown(true, _minimapObjects);
        IsParameterShown(false, _mapObjects);
        IsParameterShown(false, _gameOverObjects);
    }

    private void PauseGame(bool isPaused)
    {
        if (isPaused)
        {
            _pause = true;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            Time.timeScale = 0;
             Debug.Log("Game  Paused");
        }
        else
        {
            _pause = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1;
        }
    }

    public void Ending()
    {
        _pause = true;
        _ending = true;
        _currentMenu = 4;
        IsParameterShown(false, _pauseObjects);
        IsParameterShown(false, _settingsObjects);
        IsParameterShown(true, _minimapObjects);
        IsParameterShown(false, _mapObjects);
        IsParameterShown(false, _gameOverObjects);
        IsParameterShown(false, _minimapObjects);
        _animation.Play("Ending Fade");
        Invoke(nameof(ActivateCredits), 2f);
    }

    private void ActivateCredits()
    {
        _animation.Play("Credits");
        Invoke(nameof(LoadMenu), 55f);
    }
}
