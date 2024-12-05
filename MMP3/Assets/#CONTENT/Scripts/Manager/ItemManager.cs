using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;    

public class ItemManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _itemDisplayTxt;
    [SerializeField] private float _totalItemAmaount = 0;
    // Start is called before the first frame update
    void Start()
    {
        _itemDisplayTxt.text = "0€";
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddItem(float amount)
    {
        _totalItemAmaount += amount;
        Debug.Log("Item collected! Total value: " + _totalItemAmaount);
        _itemDisplayTxt.text = _totalItemAmaount.ToString("0.00") + "€";
    }

}
