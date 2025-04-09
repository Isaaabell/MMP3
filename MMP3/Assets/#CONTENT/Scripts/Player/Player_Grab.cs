using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Grab : MonoBehaviour
{
    [Header("Camera")]
    public Transform cameraTransform;  // Assign the camera child in inspector
    public Transform objectGrabPointTransform; // Assign the object grab point in inspector
    private ObjectGrabbable grabbable; // Reference to the grabbable object

    private bool isalreadygrabbing = false; // Flag to check if already grabbing


    // Update is called once per frame
    void Update()
    {
        Debug.Log("isGrabbing in Object: " + MovementPlayer.isGrabbing);
        if (MovementPlayer.isGrabbing && !isalreadygrabbing)
        {
            if (grabbable == null)
            {
                float pickupDistance = 2f;
                if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, pickupDistance))
                {
                    if (hit.transform.TryGetComponent<ObjectGrabbable>(out grabbable))
                    {
                        grabbable.Grab(objectGrabPointTransform);
                        isalreadygrabbing = true; // Set the flag to true when grabbing
                        // Implement your pick-up logic here
                        Debug.Log("Picked up: " + hit.transform);
                    }
                }
            }

        }
        else if (!MovementPlayer.isGrabbing && isalreadygrabbing)
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
