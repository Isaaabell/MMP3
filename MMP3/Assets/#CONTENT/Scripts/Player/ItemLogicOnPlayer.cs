using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Alteruna;

public class ItemLogicOnPlayer : AttributesSync
{
    [SerializeField] private GameObject _itemAnchor;
    private GameObject _bigItem;
    public bool _isBigItemCollected = false; //used in PlayerController.cs for movement speed
    [SynchronizableField] public bool _isItemInTriggerZone;

    public Alteruna.Avatar avatar;

    void Start()
    {
        // Ensure the local player only interacts with their own logic
        if (!avatar.IsMe)
        {
            enabled = false;
        }
    }
    void Update()
    {
        if (!avatar.IsMe)
            return;

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

    // Helper Methods
    private void CollectSmallItem(Collider smallItemCollider)
    {
        Debug.Log("Small item collected!");
        Items item = smallItemCollider.GetComponent<Items>();
        if (item != null)
        {
            // Broadcast small item collection to all clients using uniqueID
            BroadcastRemoteMethod("SyncCollectSmallItem", item._uniqueID, item._value);
        }
    }


    private void CollectBigItem(Transform bigItemParent)
    {
        if (_itemAnchor == null)
        {
            Debug.LogWarning("ItemAnker is not assigned in the Inspector!");
            return;
        }

        Debug.Log("Big item collected!");
        _bigItem = bigItemParent.gameObject;
        _bigItem.transform.SetParent(_itemAnchor.transform);
        _bigItem.transform.localPosition = Vector3.zero;
        _bigItem.transform.localScale = _bigItem.transform.localScale / 3f;

        _isBigItemCollected = true;

        // Synchronize big item collection across clients
        BroadcastRemoteMethod("SyncBigItemCollection", _bigItem.name);
    }
    private void UnloadBigItem(Collider loadingZoneCollider)
    {
        if (!_isBigItemCollected || _bigItem == null)
        {
            Debug.Log("No big item to unload.");
            return;
        }

        Transform unloadAnker = loadingZoneCollider.transform.Find("UnloadAnker");
        if (unloadAnker == null)
        {
            Debug.LogWarning("UnloadAnker is not found in the loading zone!");
            return;
        }

        Debug.Log("Unloading big item...");
        string itemName = _bigItem.name;

        // Update the item's position locally
        _bigItem.transform.SetParent(unloadAnker);
        _bigItem.transform.localPosition = Vector3.zero;

        // Reset local state
        _isBigItemCollected = false;

        // Broadcast unloading to all clients
        BroadcastRemoteMethod("SyncUnloadBigItem", itemName, unloadAnker.name);

        // Update the score (done only once via the main synchronizable method)
        Items itemComponent = unloadAnker.GetComponentInChildren<Items>();
        if (itemComponent != null)
        {
            BroadcastRemoteMethod("SyncUnloadBigItemAndUpdateScore", itemComponent._value);
        }

        _bigItem = null;
    }

    private GameObject FindItemByUniqueID(string uniqueID)
    {
        Items[] items = FindObjectsOfType<Items>();
        foreach (Items item in items)
        {
            if (item._uniqueID == uniqueID)
            {
                return item.gameObject;
            }
        }
        return null;
    }


    // Synchronizable Methods
    [SynchronizableMethod]
    private void SyncBigItemCollection(string itemName)
    {
        GameObject collectedItem = GameObject.Find(itemName);
        if (collectedItem == null) return;

        collectedItem.transform.SetParent(_itemAnchor.transform);
        collectedItem.transform.localPosition = Vector3.zero;

        _isBigItemCollected = true;
        Debug.Log("Big item synchronized as collected.");
    }

    [SynchronizableMethod]
    private void SyncUnloadBigItem(string itemName, string unloadAnchorName)
    {
        GameObject unloadedItem = GameObject.Find(itemName);
        if (unloadedItem == null)
        {
            Debug.LogWarning($"Big item '{itemName}' not found during synchronization.");
            return;
        }

        Transform unloadAnchor = GameObject.Find(unloadAnchorName)?.transform;
        if (unloadAnchor == null)
        {
            Debug.LogWarning($"UnloadAnchor '{unloadAnchorName}' not found during synchronization.");
            return;
        }

        unloadedItem.transform.SetParent(unloadAnchor);
        unloadedItem.transform.localPosition = Vector3.zero;

        _isBigItemCollected = false;

        Debug.Log($"Big item '{itemName}' synchronized as unloaded.");
    }

    [SynchronizableMethod]
    private void SyncCollectSmallItem(string uniqueID, float itemValue)
    {
        // Find the item using its unique ID
        GameObject collectedItem = FindItemByUniqueID(uniqueID);
        if (collectedItem == null)
        {
            Debug.LogWarning($"Small item with UniqueID '{uniqueID}' not found for synchronization.");
            return;
        }

        // Get the parent of the collected item
        Transform parent = collectedItem.transform.parent;
        if (parent == null)
        {
            Debug.LogWarning($"Parent of item with UniqueID '{uniqueID}' not found. Destroying the item itself.");
            parent = collectedItem.transform; // Fallback to the item itself if no parent is found
        }

        // Update the global score
        Items item = collectedItem.GetComponent<Items>();
        if (item != null)
        {
            ItemManager itemManager = FindObjectOfType<ItemManager>();
            if (itemManager != null)
            {
                itemManager.AddItem(itemValue);
                Debug.Log($"Small item value added to score: {itemValue}");
            }
            else
            {
                Debug.LogError("ItemManager not found!");
            }
        }

        // Destroy the parent object globally
        Destroy(parent.gameObject);
        Debug.Log($"Small item parent '{parent.name}' collected and destroyed globally.");
    }


    [SynchronizableMethod]
    private void SyncUnloadBigItemAndUpdateScore(float itemValue)
    {
        ItemManager itemManager = FindObjectOfType<ItemManager>();
        if (itemManager != null)
        {
            itemManager.AddItem(itemValue);
            Debug.Log("Big item value added to score: " + itemValue);
        }
        else
        {
            Debug.LogError("ItemManager not found!");
        }
    }
}