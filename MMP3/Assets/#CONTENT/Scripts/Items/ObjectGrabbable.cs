using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ObjectGrabbable : MonoBehaviour
{
    private Rigidbody rb;
    private Transform objectGrabPointTransform;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void Grab(Transform objectGrabPointTransform)
    {
        this.objectGrabPointTransform = objectGrabPointTransform;
        rb.useGravity = false; // Disable gravity when grabbed
        rb.constraints = RigidbodyConstraints.FreezeRotation; // Freeze all constraints to prevent movement

    }

    public void Drop()
    {
        this.objectGrabPointTransform = null;
        rb.useGravity = true; // Enable gravity when dropped
        rb.constraints = RigidbodyConstraints.None; // Unfreeze constraints to allow movement
    }

    private void FixedUpdate()
    {
        if (objectGrabPointTransform != null)
        {
            // Move the object to the grab point
            float lerpSpeed = 15f; // Adjust this value for smoothness
            Vector3 newPosition = Vector3.Lerp(transform.position, objectGrabPointTransform.position, Time.fixedDeltaTime * lerpSpeed);
            rb.MovePosition(newPosition);
        }
    }
}
