using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitchZoneScript : MonoBehaviour
{
    private bool _isItemInTriggerZone;
    private SceneManagerSync _sceneManagerSync;


    void Start()
    {
        _sceneManagerSync = FindObjectOfType<SceneManagerSync>();
        if (_sceneManagerSync == null)
        {
            Debug.LogError("SceneManagerSync not found in the scene.");
        }
    }
    void Update()
    {
        DisplayCurrentScene();
    }

    void DisplayCurrentScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        Debug.Log("Current active scene: " + currentScene.name + " (Build Index: " + currentScene.buildIndex + ")");

        if (currentScene.buildIndex == 0)
        {
            if (_isItemInTriggerZone && Input.GetKeyDown(KeyCode.F) && _sceneManagerSync != null)
            {
                _sceneManagerSync.BroadcastRemoteMethod("SyncLoadStreetScene");
            }
        }

        if (currentScene.buildIndex == 1)
        {
            if (_isItemInTriggerZone && Input.GetKeyDown(KeyCode.F) && _sceneManagerSync != null)
            {
                _sceneManagerSync.BroadcastRemoteMethod("SyncLoadHouseScene");
            }
        }
        
        if (currentScene.buildIndex == 2)
        {
            if (_isItemInTriggerZone && Input.GetKeyDown(KeyCode.F) && _sceneManagerSync != null)
            {
                _sceneManagerSync.BroadcastRemoteMethod("SyncLoadStreetScene");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isItemInTriggerZone = true;
            Debug.Log("Player entered the trigger zone.");
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isItemInTriggerZone = false;
            Debug.Log("Player left the trigger zone.");
        }
    }
}
