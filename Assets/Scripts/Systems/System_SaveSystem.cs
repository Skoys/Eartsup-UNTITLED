using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.LowLevel;
using static Ennemy;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem instance;

    [SerializeField] private int _currentObjective = 0;
    [SerializeField] private List<int> _interactedObjects;
    [SerializeField] private GameObject _objectiveList;
    [SerializeField] private GameObject _canvas;

    [Header("MINIMAP")]
    [SerializeField] private LayerMask _minimapLayer;
    [SerializeField] private List<GameObject> _minimapObjectives;
    [SerializeField] private List<Vector3> _minimapPosSaved;
    [SerializeField] private List<Vector3> _minimapPosCurrent;

    public bool _gotGun = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            instance.LoadTheMinimap();
            Destroy(gameObject);
        }

        NextMinimapObjective();
    }

    public void SetCurrentObjective(int nextObjective)
    {
        _currentObjective = nextObjective;
        _objectiveList.GetComponent<Objective_List>().ChangeCurrentObjective(nextObjective);
        _interactedObjects.Clear();
        NextMinimapObjective();
        SaveTheMinimap();

        Env_interact[] buttons = FindObjectsOfType<Env_interact>();
        foreach(Env_interact button in buttons)
        {
            button.IsItMyTurn(_currentObjective);
        }
    }

    public int GetCurrentObjective()
    {
        return _currentObjective; 
    }

    public bool HasBeenInteracted(int id)
    {
        return _interactedObjects.Contains(id);
    }

    public void ObjectInteracted(int id)
    {
        _interactedObjects.Add( id);
        _objectiveList.GetComponent<Objective_List>().ChangeProgression();
    }

    public void AnnonceNewObjectiveList(GameObject gameObject)
    {
        _objectiveList = gameObject;
        SetCurrentObjective(_currentObjective);
    }

    public void NextMinimapObjective()
    {
        foreach (GameObject obj in _minimapObjectives)
        {
            obj.SetActive(false);
        }
        _minimapObjectives[_currentObjective].SetActive(true);
    }

    public void AddToMinimapCurrent(Vector3 position)
    {
        _minimapPosCurrent.Add(position);
    }

    private void SaveTheMinimap()
    {
        foreach(Vector3 pos in _minimapPosCurrent)
        {
            _minimapPosSaved.Add(pos);
        }
        _minimapPosCurrent.Clear();
    }

    private void LoadTheMinimap()
    {
        Debug.Log("Loading the map");
        _minimapPosCurrent.Clear();
        foreach (Vector3 pos in _minimapPosSaved)
        {
            Debug.Log("Next Position");
            Collider[] minimapObjects = Physics.OverlapSphere(pos, 0.5f, _minimapLayer);
            foreach (var obj in minimapObjects)
            {
                Debug.Log("Getting Minimap");
                MeshRenderer goMeshRenderer = obj.gameObject.GetComponent<MeshRenderer>();
                goMeshRenderer.enabled = true;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
