using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Alteruna;
using System;

public class SceneManagerSync : AttributesSync
{
    private const int _STREETSCENEINDEX = 1;
    private const int _HOUSESCENEINDEX = 2; //TODO: for later
    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            BroadcastRemoteMethod("SyncLoadStreetScene");
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            BroadcastRemoteMethod("SyncLoadHouseScene");
        }

    }

    [SynchronizableMethod]
    private void SyncLoadStreetScene()
    {
        SceneManager.LoadSceneAsync(_STREETSCENEINDEX);
    }

        [SynchronizableMethod]
    private void SyncLoadHouseScene()
    {
        SceneManager.LoadSceneAsync(_HOUSESCENEINDEX);
    }
}
