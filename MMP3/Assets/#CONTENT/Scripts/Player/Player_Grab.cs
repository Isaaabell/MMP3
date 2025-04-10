using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Grab : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;  // Assign the camera child in inspector
    public Transform objectGrabPointTransform; // Assign the object grab point in inspector
    
    [Header("Settings")]
    public float pickupDistance = 2f;
    
    private ObjectGrabbable grabbable; // Reference to the grabbable object
    private bool isalreadygrabbing = false; // Flag to check if already grabbing
    
    // Reference to this player's movement component
    private MovementPlayer movementPlayer;

    private void Awake()
    {
        movementPlayer = GetComponent<MovementPlayer>();
        if (movementPlayer == null)
        {
            Debug.LogError("Player_Grab requires a MovementPlayer component on the same GameObject!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(movementPlayer.isGrabbing);
        if (movementPlayer == null) return;
        
        if (movementPlayer.isGrabbing && !isalreadygrabbing)
        {
            if (grabbable == null)
            {
                if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, pickupDistance))
                {
                    if (hit.transform.TryGetComponent<ObjectGrabbable>(out grabbable))
                    {
                        // Call Grab with the player reference
                        grabbable.Grab(objectGrabPointTransform, movementPlayer);

                        // Only set isalreadygrabbing if it's not a SmallItem
                        if (grabbable.itemType != ItemType.SmallItem)
                        {
                            isalreadygrabbing = true;
                            Debug.Log($"Player {gameObject.name} picked up: {hit.transform.name} ({grabbable.itemType})");
                        }
                        else
                        {
                            // If it was a SmallItem, it's already collected and destroyed, so reset grabbable
                            grabbable = null;
                        }
                    }
                }
            }
        }
        else if (!movementPlayer.isGrabbing && isalreadygrabbing)
        {
            if (grabbable != null)
            {
                grabbable.Drop();
                grabbable = null; // Reset the grabbable reference
                isalreadygrabbing = false; // Reset the flag when dropping
            }
        }
    }
}

