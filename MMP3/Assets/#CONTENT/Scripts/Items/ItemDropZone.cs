using UnityEngine;

public class ItemDropZone : MonoBehaviour
{
    private bool playerInZone = false;
    
    private ItemInteraction itemInteraction;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Debug.Log("Player entered drop zone");
            playerInZone = true;
            itemInteraction = other.GetComponent<ItemInteraction>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Debug.Log("Player exited drop zone");
            playerInZone = false;
            itemInteraction = null;
        }
    }

    public void DeactivateItem(Item1 item)
    {
        if (item != null)
        {
            // Debug.Log("Deactivating item: " + item.name);
            item.gameObject.SetActive(false); // End Point Big items
            item.tutorialboolSmallItem = true;
        }
        else
        {
            Debug.LogWarning("Attempted to deactivate a null item.");
        }
    }

    public bool CanDropItem()
    {
        bool canDrop = playerInZone && itemInteraction != null && itemInteraction.HasCarriedItem();
        // Debug.Log("Can drop item: " + canDrop);
        return canDrop;
    }
}
