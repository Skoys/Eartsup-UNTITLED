using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscoverNewMiniMapTiles : MonoBehaviour
{
    [SerializeField] private string _saveTag = "Respawn";
    [SerializeField] private SaveSystem _saveSystem;

    private void Start()
    {
        _saveSystem = GameObject.FindGameObjectWithTag(_saveTag).GetComponent<SaveSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.layer == other.gameObject.layer)
        {

            MeshRenderer goMeshrenderer = other.gameObject.GetComponent<MeshRenderer>();

            if (!goMeshrenderer.enabled) 
            {
                goMeshrenderer.enabled = true;
                _saveSystem.AddToMinimapCurrent(other.transform.position);
            }
        }
    }
}
