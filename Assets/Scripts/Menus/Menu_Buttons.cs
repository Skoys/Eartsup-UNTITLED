using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonMenuScript : MonoBehaviour
{
    [SerializeField] private GameObject _pressEnter;
    private TextMeshProUGUI _pressEnterTMP;

    [SerializeField] private string _gameSceneName;

    private bool _canStart = false;

    void Start()
    {
        _pressEnterTMP = _pressEnter.GetComponent<TextMeshProUGUI>();
        _pressEnterTMP.enabled = false;
        StartCoroutine(ShowPressStart());
    }

    private void Update()
    {
        if (_canStart&& Input.anyKey)
        {
            LoadGame();
        }
    }

    private IEnumerator ShowPressStart()
    {
        yield return new WaitForSeconds(2f);
        _canStart = true;
        _pressEnterTMP.enabled = true;
    }

    private void LoadGame()
    {
        SceneManager.LoadScene(_gameSceneName);
    }
}
