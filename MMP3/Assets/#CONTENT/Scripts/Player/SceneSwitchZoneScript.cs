using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSwitchZoneScript : MonoBehaviour
{
    private bool _isPlayerInTriggerZone;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("III: Teleport entered the trigger zone.");
            _isPlayerInTriggerZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("III: Teleport exited the trigger zone.");
            _isPlayerInTriggerZone = false;
        }
    }

    void DisplayCurrentScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        // Debug.Log("Current active scene: " + currentScene.name + " (Build Index: " + currentScene.buildIndex + ")");

        if (currentScene.buildIndex == 0)
        {
            if (_isPlayerInTriggerZone && Input.GetKeyDown(KeyCode.F) && _sceneManagerSync != null)
            {
                _sceneManagerSync.BroadcastRemoteMethod("SyncLoadStreetScene");

            }
        }

        if (currentScene.buildIndex == 1)
        {
            if (_isPlayerInTriggerZone && Input.GetKeyDown(KeyCode.F) && _sceneManagerSync != null)
            {
                _sceneManagerSync.BroadcastRemoteMethod("SyncLoadHouseScene");
            }
        }

        if (currentScene.buildIndex == 2)
        {
            if (_isPlayerInTriggerZone && Input.GetKeyDown(KeyCode.F) && _sceneManagerSync != null)
            {
                // _sceneManagerSync.BroadcastRemoteMethod("SyncLoadStreetScene");
                ItemManager.Instance.WinGame();

            }
        }
    }
}
