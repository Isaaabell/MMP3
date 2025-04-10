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
    
    [Header("Value Properties")]
    [Tooltip("The monetary value of this item")]
    public int monetaryValue = 100;
    [Tooltip("Show the value above this item")]
    public bool showValue = true;
    
    [Header("Weight Properties")]
    [Tooltip("Speed modifier when carrying this item (1.0 = no effect, 0.5 = half speed)")]
    private float weightSpeedModifier = 1.0f;
    
    [Header("Outline Properties")]
    public bool enableOutline = true;
    private float outlineWidth = 10.0f; // Default outline width
    private Outline outlineComponent;
    private ValueDisplay valueDisplay;
    
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
        
        // Setup outline based on item type
        if (enableOutline)
        {
            SetupOutline();
        }
        
        // Setup value display if enabled
        if (showValue)
        {
            SetupValueDisplay();
        }
    }
    
    private void SetupOutline()
    {
        // Get or add the outline component
        outlineComponent = GetComponent<Outline>();
        if (outlineComponent == null)
        {
            outlineComponent = gameObject.AddComponent<Outline>();
        }
        
        // Set outline properties
        outlineComponent.OutlineMode = Outline.Mode.OutlineVisible;
        outlineComponent.OutlineWidth = outlineWidth;
        
        // Set color based on item type
        switch (itemType)
        {
            case ItemType.SmallItem:
                outlineComponent.OutlineColor = new Color(0.0f, 1f, 0.0f); // Green for collectibles
                break;
            case ItemType.LightItem:
                outlineComponent.OutlineColor = new Color(1f, 0.92f, 0.016f);   // Yellow for light items
                break;
            case ItemType.MediumItem:
                outlineComponent.OutlineColor = new Color(1f, 0.5f, 0.0f);   // Orange for medium items
                break;
            case ItemType.HeavyItem:
                outlineComponent.OutlineColor = new Color(1f, 0.0f, 0.0f);   // Red for heavy items
                break;
        }
    }
    
    private void SetupValueDisplay()
    {
        // Add or get ValueDisplay component
        valueDisplay = GetComponent<ValueDisplay>();
        if (valueDisplay == null)
        {
            valueDisplay = gameObject.AddComponent<ValueDisplay>();
        }
        
        // Set the monetary value
        valueDisplay.itemValue = monetaryValue;
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
        
        // Hide value display when grabbed
        if (valueDisplay != null)
        {
            valueDisplay.enabled = false;
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
            
            // Show value display again
            if (valueDisplay != null)
            {
                valueDisplay.enabled = true;
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
        Debug.Log("Collected: " + gameObject.name + " (Value: $" + monetaryValue + ")");
        
        // If you have a money manager, add the value
        MoneyManager moneyManager = FindObjectOfType<MoneyManager>();
        if (moneyManager != null)
        {
            moneyManager.AddMoney(monetaryValue);
        }
        
        Destroy(gameObject); // Destroy the object after collecting
    }
    
    // Allow other scripts to disable outline if needed
    public void SetOutlineEnabled(bool enabled)
    {
        if (outlineComponent != null)
        {
            outlineComponent.enabled = enabled;
        }
    }
}