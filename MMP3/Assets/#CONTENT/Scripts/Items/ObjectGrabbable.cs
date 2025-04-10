using UnityEngine;

public enum ItemType
{
    SmallItem,   // Collected and disappears
    LightItem,   // Carried with minimal speed reduction
    MediumItem,  // Carried with moderate speed reduction
    HeavyItem    // Carried with significant speed reduction
}

public class ObjectGrabbable : MonoBehaviour
{
    private Rigidbody rb;
    private Transform objectGrabPointTransform;
    
    [Header("Item Properties")]
    public ItemType itemType = ItemType.LightItem;
    
    [Header("Weight Properties")]
    [Tooltip("Speed modifier when carrying this item (1.0 = no effect, 0.5 = half speed)")]
    private float weightSpeedModifier = 1.0f;
    
    // Reference to the player that's carrying this object
    private MovementPlayer playerMovement;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        // Set default weight modifiers based on item type
        if (weightSpeedModifier == 1.0f) // Only auto-set if not manually configured
        {
            switch (itemType)
            {
                case ItemType.LightItem:
                    weightSpeedModifier = 0.8f;
                    break;
                case ItemType.MediumItem:
                    weightSpeedModifier = 0.6f;
                    break;
                case ItemType.HeavyItem:
                    weightSpeedModifier = 0.4f;
                    break;
            }
        }
    }
    
    public void Grab(Transform objectGrabPointTransform, MovementPlayer player)
    {
        if (itemType == ItemType.SmallItem)
        {
            CollectItem();
            return; // Exit if the item is small
        }
        
        this.objectGrabPointTransform = objectGrabPointTransform;
        rb.useGravity = false; // Disable gravity when grabbed
        rb.constraints = RigidbodyConstraints.FreezeRotation; // Freeze all constraints to prevent movement
        
        // Store reference to the player that's carrying this object
        playerMovement = player;
        
        // Apply speed modifier to the specific player
        if (playerMovement != null)
        {
            playerMovement.ApplySpeedModifier(weightSpeedModifier);
        }
    }

    public void Drop()
    {
        if (itemType != ItemType.SmallItem)
        {
            this.objectGrabPointTransform = null;
            rb.useGravity = true; // Enable gravity when dropped
            rb.constraints = RigidbodyConstraints.None; // Unfreeze constraints to allow movement
            
            // Reset player's speed when item is dropped
            if (playerMovement != null)
            {
                playerMovement.ResetSpeedModifier();
                playerMovement = null;
            }
        }
    }

    private void FixedUpdate()
    {
        if (objectGrabPointTransform != null)
        {
            // Move the object to the grab point
            float lerpSpeed = 10f; // Adjust this value for smoothness
            Vector3 newPosition = Vector3.Lerp(transform.position, objectGrabPointTransform.position, Time.fixedDeltaTime * lerpSpeed);
            rb.MovePosition(newPosition);
        }
    }

    private void CollectItem()
    {
        // Implement your item collection logic here
        // For example, you can destroy the object or add it to an inventory
        Debug.Log("Collected: " + gameObject.name);
        Destroy(gameObject); // Destroy the object after collecting
    }
}