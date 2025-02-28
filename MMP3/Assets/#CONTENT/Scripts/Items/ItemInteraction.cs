using Fusion;
using UnityEngine;

public class ItemInteraction : MonoBehaviour
{
    private Item1 currentItem = null;
    private Item1 carriedItem = null; // For big items
    private ItemDropZone dropZone = null; 

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (carriedItem != null)
            {
                if (dropZone != null && dropZone.CanDropItem())
                {
                    // Debug.Log("Dropping item in drop zone");
                    carriedItem.Drop();
                    dropZone.DeactivateItem(carriedItem); // Pass carried item to drop zone
                }
                else
                {
                    // Debug.Log("Dropping item outside drop zone");
                    carriedItem.Drop();
                    carriedItem = null;
                }
            }
            else if (currentItem != null)
            {
                // Debug.Log("Picking up item");
                currentItem.PickUp(transform);
                if (currentItem.itemType == Item1.ItemType.Big)
                {
                    carriedItem = currentItem;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Item1 item))
        {
            // Debug.Log("Entered item: " + item.name);
            currentItem = item;
        }
        else if (other.TryGetComponent(out ItemDropZone zone))
        {
            // Debug.Log("Entered drop zone: " + zone.name);
            dropZone = zone;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Item1 item) && item == currentItem)
        {
            // Debug.Log("Exited item: " + item.name);
            currentItem = null;
        }
        else if (other.TryGetComponent(out ItemDropZone zone) && zone == dropZone)
        {
            // Debug.Log("Exited drop zone: " + zone.name);
            dropZone = null;
        }
    }

    // This method checks if the player is carrying an item
    public bool HasCarriedItem()
    {
        return carriedItem != null;
    }
}
