using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class Objective_List : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _objectiveText;
    [SerializeField] private int _currentObjectiveProgression;
    [SerializeField] private int _maxObjectiveProgression;
    [SerializeField] private int _currentObjectiveIndex;
    [SerializeField] private GameObject _tabLight;

    private void Start()
    {
        GameObject respawn = GameObject.FindGameObjectWithTag("Respawn");
        respawn.GetComponent<SaveSystem>().AnnonceNewObjectiveList(gameObject);
        Debug.Log("Set Save System");
        SetText();
    }

    public void ChangeCurrentObjective(int index)
    {
        _currentObjectiveIndex = index;
        _currentObjectiveProgression = 0;
        SetText();
    }

    public void ChangeProgression()
    {
        _currentObjectiveProgression++;
        SetText();
    }

    private void SetText()
    {
        string objectiveName = GetCurrentObjectiveName();

        _objectiveText.SetText( objectiveName + "\n{0} / {1}",
                _currentObjectiveProgression,
                _maxObjectiveProgression
                );
    }

    private string GetCurrentObjectiveName()
    {
        switch (_currentObjectiveIndex)
        {
            case 0:
                _maxObjectiveProgression = 2;
                return "Find your way out.";

            case 1:
                _maxObjectiveProgression = 3;
                return "Get into the Command room.";

            case 2:
                _maxObjectiveProgression = 1;
                return "Activate the door to the exit.";

            case 3:
                _maxObjectiveProgression = 1;
                return "Find the Freight elevator.";

            case 4:
                _maxObjectiveProgression = 3;
                return "Get the fuel.";

            case 5:
                _maxObjectiveProgression = 1;
                return "Activate the Freight elevator.";

            case 6:
                _maxObjectiveProgression = 0;
                return "Game Finished :)";
        }
        return null;
    }
}
