using System;
using System.Collections;
using System.Collections.Generic;
using Alteruna;
using UnityEngine;

[Serializable]
public class PlayerManager : AttributesSync
{
    public static PlayerManager Instance { get; private set; }

    public List<GameObject> _players = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddPlayer(GameObject player)
    {
        _players.Add(player);
    }

    public void ActivateDeactivateGameObject(GameObject gameObject, bool active)
    {
        gameObject.SetActive(active);
        BroadcastRemoteMethod("SyncActivateDeactivateGameObject", gameObject.name, active);
    }

    public void DisableCanvas(Canvas canvas)
    {
        canvas.enabled = false;
        BroadcastRemoteMethod("SyncDisableCanvas", canvas.name);
    }


    // Synchronizable Methods
    [SynchronizableMethod]
    private void SyncActivateDeactivateGameObject(string gameObjectName, bool active)
    {
        GameObject gameObject = GameObject.Find(gameObjectName);

        if (gameObject == null)
        {
            return;
        }

        gameObject.SetActive(active);
    }

    [SynchronizableMethod]
    private void SyncDisableCanvas(string canvasName)
    {
        Canvas canvas = GameObject.Find(canvasName).GetComponent<Canvas>();

        if (canvas == null)
        {
            return;
        }

        canvas.enabled = false;
    }
}