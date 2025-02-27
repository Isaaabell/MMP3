using UnityEngine;
using Fusion;

public class Item : NetworkBehaviour
{
    public enum ItemType { Small, Big }
    public enum Weight { Light, Medium, Heavy }

    public ItemType itemType;
    public Weight weight; //ToDo: Implement
    public int priceLevel; //ToDo: Implement

    // private bool isCarried = false;
    private ItemDropZone dropZone = null;
    public bool tutorialboolSmallItem = false;
    public bool tutorialboolBigItem = false;


    [Networked]
    public NetworkBool IsCarried { get; set; }
     [Networked]
    public Vector3 NetworkedPosition { get; set; }


    public void PickUp(Transform player)
    {
        // Debug.Log("Picking up item: " + gameObject.name);
        if (itemType == ItemType.Small)
        {
            gameObject.SetActive(false); // End Point Small items
            tutorialboolSmallItem = true;
        }
        else if (itemType == ItemType.Big)
        {
            IsCarried = true;
            transform.SetParent(player);
            transform.localPosition = new Vector3(0, 1, 0);
        }
    }

    public void Drop()
    {
        if (!IsCarried)
        {
            return;
        }

        // Debug.Log("Dropping item: " + gameObject.name);
        
        IsCarried = false;
        transform.SetParent(null);
        NetworkedPosition = transform.position; // Update the networked position
        gameObject.SetActive(true);
    }

     public override void FixedUpdateNetwork()
    {
        if (IsCarried)
        {
            // Update the position and rotation of the item to match the player's transform
            transform.position = transform.parent.position + new Vector3(0, 1, 0);
            transform.rotation = transform.parent.rotation;
        }
        else
        {
            // Ensure the item's position is synchronized across the network
            transform.position = NetworkedPosition;
        }
    }
}
