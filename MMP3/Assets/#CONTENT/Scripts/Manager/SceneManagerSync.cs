using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Alteruna;
using System;
using UnityEngine.UI;



public class SceneManagerSync : AttributesSync
{
    private const int _STREETSCENEINDEX = 1;
    private const int _HOUSESCENEINDEX = 2;


    void Start()
    {
    }
    void Update()
    {
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
