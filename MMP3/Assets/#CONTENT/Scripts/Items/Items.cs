using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Alteruna;

public class Items : AttributesSync
{
    [SerializeField] private TextMeshProUGUI _valueTxt;
    [SerializeField] private TextMeshProUGUI _interactTxt;
    public float _value = 0;
    public bool _isSmallItem = false;
    public bool _hasValueBeenAdded = false;
    [SynchronizableField] public string _uniqueID;

    
    private void Start()
    {
        _interactTxt.alpha = 0;
        _valueTxt.text = _value.ToString("0.00") + "€";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _interactTxt.alpha = 1;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _interactTxt.alpha = 0;
        }
    }

    // public void SetUniqueID(string id)
    // {
    //     uniqueID = id;
    // }

}