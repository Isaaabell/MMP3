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

    public void ParentPlayerToCar(GameObject player, GameObject carSeat)
    {
        player.transform.SetParent(carSeat.transform);
        player.transform.localPosition = Vector3.zero;
        player.transform.localRotation = Quaternion.identity;

        // Synchronize this action across all clients
        BroadcastRemoteMethod("SyncParentPlayerToCar", player.name, carSeat.name);
    }

    // Synchronizable Methods
    [SynchronizableMethod]
    private void SyncParentPlayerToCar(string playerName, string carSeatName)
    {
        GameObject player = GameObject.Find(playerName);
        GameObject carSeat = GameObject.Find(carSeatName);

        if (player == null || carSeat == null)
        {
            Debug.LogWarning($"Sync failed: Player '{playerName}' or CarSeat '{carSeatName}' not found.");
            return;
        }

        player.transform.SetParent(carSeat.transform);
        player.transform.localPosition = Vector3.zero;
        player.transform.localRotation = Quaternion.identity;

        Debug.Log($"Synced: {playerName} is now parented to {carSeatName}");
    }
}