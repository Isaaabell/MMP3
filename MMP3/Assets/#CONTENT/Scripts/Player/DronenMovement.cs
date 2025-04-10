using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(PlayerInput))]
public class DroneMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float verticalSpeed = 3f;

    // Zwischengespeicherte Input-Werte
    private float currentMoveInput = 0f;
    private float currentTurnInput = 0f;
    private float currentVerticalInput = 0f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
{
    // Bewegung vorwärts/rückwärts (unabhängig von der Rotation)
    Vector3 movement = Vector3.forward * currentMoveInput * moveSpeed * Time.fixedDeltaTime;
    // Rotation (weiterhin um die Y-Achse)
    float rotation = currentTurnInput * rotationSpeed * Time.fixedDeltaTime;
    // Vertikale Bewegung (auf/ab)
    Vector3 verticalMovement = Vector3.up * currentVerticalInput * verticalSpeed * Time.fixedDeltaTime;

    // Bewegung in Weltkoordinaten anwenden
    rb.MovePosition(rb.position + transform.TransformDirection(movement) + verticalMovement);
    rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, rotation, 0f));
}

    // === Input-Event-Methoden (werden automatisch vom PlayerInput aufgerufen) ===
    public void OnForward(InputAction.CallbackContext context)
    {
        if (context.performed)
            currentMoveInput = context.ReadValue<float>(); // 1 bei W
        else if (context.canceled && currentMoveInput > 0)
            currentMoveInput = 0; // Zurücksetzen wenn W losgelassen
    }

    public void OnBackward(InputAction.CallbackContext context)
    {
        if (context.performed)
            currentMoveInput = -context.ReadValue<float>(); // -1 bei S
        else if (context.canceled && currentMoveInput < 0)
            currentMoveInput = 0; // Zurücksetzen wenn S losgelassen
    }

    public void OnTurnRight(InputAction.CallbackContext context)
    {
        if (context.performed)
            currentTurnInput = context.ReadValue<float>(); // 1 bei D
        else if (context.canceled && currentTurnInput > 0)
            currentTurnInput = 0; // Zurücksetzen wenn D losgelassen
    }

    public void OnTurnLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
            currentTurnInput = -context.ReadValue<float>(); // -1 bei A
        else if (context.canceled && currentTurnInput < 0)
            currentTurnInput = 0; // Zurücksetzen wenn A losgelassen
    }

    public void OnFlyUp(InputAction.CallbackContext context)
    {
        if (context.performed)
            currentVerticalInput = context.ReadValue<float>(); // 1 bei Q
        else if (context.canceled && currentVerticalInput > 0)
            currentVerticalInput = 0; // Zurücksetzen wenn Q losgelassen
    }

    public void OnFlyDown(InputAction.CallbackContext context)
    {
        if (context.performed)
            currentVerticalInput = -context.ReadValue<float>(); // -1 bei E
        else if (context.canceled && currentVerticalInput < 0)
            currentVerticalInput = 0; // Zurücksetzen wenn E losgelassen
    }
}