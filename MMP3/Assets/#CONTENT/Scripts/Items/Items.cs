using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Items : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _valueTxt;
    [SerializeField] private TextMeshProUGUI _interactTxt;
    [SerializeField] private float _value = 0;
    [SerializeField] private ItemManager _itemManager;
    public bool _isSmallItem = false;
    [SerializeField] private bool _isPlayerInTriggerZone;
    [SerializeField] private bool _hasValueBeenAdded = false;
    public bool _isBigItemUnloaded = false;


    void Start()
    {
        _interactTxt.alpha = 0;
        _valueTxt.text = _value.ToString("0.00") + "€";

    }

    void Update()
    {
        if (_isSmallItem)
        {
            if (_isPlayerInTriggerZone && Input.GetKeyDown(KeyCode.F))
            {
                AddToScore();
            }
        }
        else if (!_isSmallItem)
        {
            if (_isBigItemUnloaded && !_hasValueBeenAdded)
            {
                AddToScore();
                _hasValueBeenAdded = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _interactTxt.alpha = 1;
            _isPlayerInTriggerZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _interactTxt.alpha = 0;
            _isPlayerInTriggerZone = false;
        }
    }

    private void AddToScore()
    {
        _itemManager.AddItem(_value);
    }
}
