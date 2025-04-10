using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementPlayer : MonoBehaviour
{
    [Header("Movement")]
    public float baseSpeed = 5f;
    private float currentSpeed;
    public float lookSensitivity = 1.5f;
    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float cameraPitch = 0f;
    private float deadzone = 0.1f; // Deadzone for input
    
    [Header("Grabbing")]
    [HideInInspector] public bool isGrabbing = false; // Per-player grabbing state
    
    // Speed modification properties
    private float speedModifier = 1.0f;
    
    [Header("Camera")]
    public Transform cameraTransform;  // Assign the camera child in inspector


    private void Awake()
    {

        DontDestroyOnLoad(gameObject);

       rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        
        // Initialize current speed
        currentSpeed = baseSpeed;

        // If camera not assigned, try to find it automatically
        if (cameraTransform == null)
        {
            // Try to find a camera in children
            Camera childCamera = GetComponentInChildren<Camera>();
            if (childCamera != null)
            {
                cameraTransform = childCamera.transform;
            }
            else
            {
                Debug.LogError("No camera assigned and couldn't find one in children!");
            }
        }

    }

    private void FixedUpdate()
    {
        // Move based on input, using the currentSpeed which might be modified
        Vector3 move = transform.forward * moveInput.y + transform.right * moveInput.x;
        rb.MovePosition(rb.position + move * currentSpeed * Time.fixedDeltaTime);
    }

    private void LateUpdate()
    {
        // Rotate player horizontally
        transform.Rotate(Vector3.up * lookInput.x * lookSensitivity);

        // Rotate camera vertically
        cameraPitch -= lookInput.y * lookSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -80f, 80f);

        // Apply camera rotation
        if (cameraTransform != null)
        {
            cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        }
    }

    // Apply a speed modifier when carrying objects
    public void ApplySpeedModifier(float modifier)
    {
        speedModifier = Mathf.Clamp(modifier, 0.1f, 1.0f);
        UpdateCurrentSpeed();
    }
    
    // Reset speed modifier when dropping objects
    public void ResetSpeedModifier()
    {
        speedModifier = 1.0f;
        UpdateCurrentSpeed();
    }
    
    // Calculate current speed based on base speed and modifier
    private void UpdateCurrentSpeed()
    {
        currentSpeed = baseSpeed * speedModifier;
    }

    // Input System Actions
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        // Apply deadzone
        if (moveInput.magnitude < deadzone)
        {
            moveInput = Vector2.zero;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
        // Apply deadzone
        if (lookInput.magnitude < deadzone)
        {
            lookInput = Vector2.zero;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (IsGrounded())
        {
            rb.AddForce(Vector3.up * 120, ForceMode.Impulse);
        }
    }

    public void OnPickUp(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isGrabbing = true; // Toggle grabbing state for this player only
        }
        else if (context.canceled)
        {
            isGrabbing = false; // Reset grabbing state when input is released
        }
    }

    private bool IsGrounded()
    {
        Ray ray = new Ray(this.transform.position + Vector3.up * 0.25f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 1.4f))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
