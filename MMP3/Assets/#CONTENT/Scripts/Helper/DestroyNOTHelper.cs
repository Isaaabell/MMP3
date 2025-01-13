using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyNOTHelper : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
