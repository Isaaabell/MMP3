using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Alteruna;

public class ItemManager : AttributesSync
{
    [SerializeField] private TextMeshProUGUI _itemDisplayTxt;
    [SynchronizableField] public float _totalItemAmount = 0;
    void Start()
    {
        _itemDisplayTxt.text = "0€";
    }

    public void AddItem(float amount)
    {
        BroadcastRemoteMethod("SyncAddItem", amount);
    }

    [SynchronizableMethod]
    private void SyncAddItem(float amount)
    {
        _totalItemAmount += amount;
        Debug.Log("Item collected! Total value: " + _totalItemAmount);
        _itemDisplayTxt.text = _totalItemAmount.ToString("0.00") + "€";
    }
}
