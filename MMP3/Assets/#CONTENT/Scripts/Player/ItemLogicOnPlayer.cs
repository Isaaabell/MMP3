using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemLogicOnPlayer : MonoBehaviour
{
    [SerializeField] private GameObject _itemAnker;
    private GameObject _bigItem;
    public bool _isBigItemCollected = false; //used in PlayerController.cs for movement speed
    [SerializeField] private bool _isItemInTriggerZone;

    void Update()
    {
        if (_isItemInTriggerZone && Input.GetKeyDown(KeyCode.F))
        {
            CollectItem();
        }


    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            _isItemInTriggerZone = true;
        }
        else if (other.CompareTag("LoadingZone"))
        {
            UnloadBigItem(other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            _isItemInTriggerZone = false;
        }
    }
    private void CollectItem()
    {
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, 2f);
        foreach (Collider collider in nearbyColliders)
        {
            if (!collider.CompareTag("Item")) continue;

            Items item = collider.GetComponent<Items>();
            if (item == null) continue;

            if (item._isSmallItem)
            {
                CollectSmallItem(collider);
            }
            else
            {
                CollectBigItem(collider.transform.parent);
                collider.isTrigger = false;
                collider.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            }
        }
    }

    //Helper//
    private void CollectSmallItem(Collider smallItemCollider)
    {
        Debug.Log("Small item collected!");
        Items item = smallItemCollider.GetComponent<Items>();
        if (item != null)
        {
            Destroy(smallItemCollider.transform.parent.gameObject); // Destroy the entire small item
        }
    }
    private void CollectBigItem(Transform bigItemParent)
    {
        if (_itemAnker == null)
        {
            Debug.LogWarning("ItemAnker is not assigned in the Inspector!");
            return;
        }

        Debug.Log("Big item collected!");
        _bigItem = bigItemParent.gameObject;
        _bigItem.transform.SetParent(_itemAnker.transform);
        _bigItem.transform.localPosition = Vector3.zero;

        _isBigItemCollected = true;
    }
    private void UnloadBigItem(Collider loadingZoneCollider)
    {
        if (!_isBigItemCollected || _bigItem == null)
        {
            Debug.Log("No big item to unload.");
            return;
        }

        Transform unloadAnker = loadingZoneCollider.transform.Find("UnloadAnker"); // needs to be this exact name
        if (unloadAnker == null)
        {
            Debug.LogWarning("UnloadAnker is not found in the loading zone!");
            return;
        }

        Debug.Log("Unloading big item...");
        _bigItem.transform.SetParent(unloadAnker);
        _bigItem.transform.localPosition = Vector3.zero;


        // money money money
        Items itemComponent = unloadAnker.GetComponentInChildren<Items>();
        if (itemComponent != null)
        {
            itemComponent._isBigItemUnloaded = true;
        }

        // Reset big item state
        _isBigItemCollected = false;
        _bigItem = null;
    }

}
