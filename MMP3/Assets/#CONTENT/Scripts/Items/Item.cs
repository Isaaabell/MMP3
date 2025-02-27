using UnityEngine;
using Fusion;

public class Item : NetworkBehaviour
{
    public enum ItemType { Small, Big }
    public enum Weight { Light, Medium, Heavy }

    public ItemType itemType;
    public Weight weight; //ToDo: Implement
    public int priceLevel; //ToDo: Implement

    private ItemDropZone dropZone = null;
    public bool tutorialboolSmallItem = false;
    public bool tutorialboolBigItem = false;

    [Networked]
    public NetworkBool IsCarried { get; set; }

    [Networked]
    public Vector3 NetworkedPosition { get; set; }

    [Networked]
    public Quaternion NetworkedRotation { get; set; }

    [Networked]
    public PlayerMovement Carrier { get; set; }

    public override void Spawned()
    {
        // Initialize the item's position and rotation
        NetworkedPosition = transform.position;
        NetworkedRotation = transform.rotation;
    }

    public void PickUp(Transform player)
    {
        Debug.Log("Picking up item: " + gameObject.name);
        if (itemType == ItemType.Small)
        {
            gameObject.SetActive(false); // End Point Small items
            tutorialboolSmallItem = true;
        }
        else if (itemType == ItemType.Big)
        {
            IsCarried = true;
            transform.SetParent(player);
            transform.localPosition = new Vector3(0, 1, 0f);
        }
    }

    public void PickUp2(PlayerMovement player)
    {
        Debug.Log("Picking up item: " + gameObject.name);
        if (itemType == ItemType.Small)
        {
            gameObject.SetActive(false); // End Point Small items
            tutorialboolSmallItem = true;
        }
        else if (itemType == ItemType.Big)
        {
            IsCarried = true;
            Carrier = player; // Set the carrier
        }
    }


    public void Drop()
    {
        if (!IsCarried)
        {
            return;
        }

        Debug.Log("Dropping item: " + gameObject.name);

        IsCarried = false;
        transform.SetParent(null);

        // Update the networked position and rotation immediately
        NetworkedPosition = transform.position;
        NetworkedRotation = transform.rotation;
        gameObject.SetActive(true);
    }

    public void Drop2()
    {
        if (!IsCarried)
        {
            return;
        }

        Debug.Log("Dropping item: " + gameObject.name);

        IsCarried = false;
        Carrier = null; // Clear the carrier

        // Update the networked position and rotation immediately
        NetworkedPosition = transform.position;
        NetworkedRotation = transform.rotation;

        gameObject.SetActive(true);
    }

    public override void FixedUpdateNetwork()
    {
        if (IsCarried && Carrier != null)
        {
             // Update the position and rotation of the item relative to the player
            Vector3 carryOffset = Carrier.transform.forward * 1.5f + new Vector3(0, 0, 0);
            NetworkedPosition = Carrier.transform.position + carryOffset;
            NetworkedRotation = Carrier.transform.rotation;

            // Apply the networked position and rotation
            transform.position = NetworkedPosition;
            transform.rotation = NetworkedRotation;
        }
        else
        {
            // Ensure the item's position and rotation are synchronized across the network
            transform.position = NetworkedPosition;
            transform.rotation = NetworkedRotation;
        }
    }
}