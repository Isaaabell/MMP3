using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna; 
using System;

public class DebugHelper : MonoBehaviour
{
    public void Start() { Debug.Log("Start method called."); } // Update is called once per frame 
    public void Update() { }

    

    public void OnPacketSent(Multiplayer multiplayer, IAdaptiveSerializableUniqueID id) { Debug.Log("Packet sent."); }
    public void OnPacketReceived(Multiplayer multiplayer, IAdaptiveSerializableUniqueID id) { Debug.Log("Packet received."); }
    public void OnLockRequested(Multiplayer multiplayer, IAdaptiveSerializableUniqueID id) { Debug.Log("Lock requested."); }
    public void OnLockAcquired(Multiplayer multiplayer, IAdaptiveSerializableUniqueID id) { Debug.Log("Lock acquired."); }
    public void OnLockDenied(Multiplayer multiplayer, IAdaptiveSerializableUniqueID id) { Debug.Log("Lock denied."); }
    public void OnLockUnlocked(Multiplayer multiplayer, IAdaptiveSerializableUniqueID id) { Debug.Log("Lock unlocked."); }
    public void OnForceSynced(Multiplayer multiplayer, UInt16 value) { Debug.Log("Force synced."); }
    public void OnRpcReceived(Multiplayer multiplayer, string message, UInt16 value) { Debug.Log("RPC received."); }
    public void OnRpcSent(Multiplayer multiplayer, string message, UInt16 value, bool flag) { Debug.Log("RPC sent."); }
    public void OnRpcRegistered(Multiplayer multiplayer, string message) { Debug.Log("RPC registered."); }
}
