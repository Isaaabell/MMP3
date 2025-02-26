using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
public float moveSpeed = 3.0f; // Bewegungsgeschwindigkeit
    public float lookSpeed = 2.0f; // Maus-Blickgeschwindigkeit
    public float gravity = -9.81f; // Schwerkraft

    private CharacterController characterController;
    private Camera playerCamera;
    private Vector3 velocity;
    private float rotationX = 0;
    [SerializeField] private Canvas canvas;

    void Start()
    {
        // Holen des CharacterControllers und der Kamera
        characterController = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        if (canvas.gameObject.activeSelf)
        {
            // Wenn das Canvas aktiv ist, Bewegung und Blicksteuerung deaktivieren
            return; //ignore rest of the code
        }

        // Spieler bewegen
        MovePlayer();
        
        // Mausbewegung für Blicksteuerung
        LookAround();
    }

    void MovePlayer()
    {
        // Bewegung des Spielers
        float moveDirectionX = Input.GetAxis("Horizontal");
        float moveDirectionZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveDirectionX + transform.forward * moveDirectionZ;

        // Schwerkraft anwenden
        if (characterController.isGrounded)
        {
            velocity.y = -2f; // Kleine negative Y-Bewegung, um auf dem Boden zu bleiben
        }
        else
        {
            velocity.y += gravity * Time.deltaTime; // Schwerkraft anwenden
        }

        // Bewegung anwenden
        characterController.Move((move * moveSpeed + velocity) * Time.deltaTime);
    }

    void LookAround()
    {
        // Mausbewegung horizontal (links / rechts)
        float rotationY = Input.GetAxis("Mouse X") * lookSpeed;
        
        // Mausbewegung vertikal (hoch / runter)
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f); // Begrenzung, damit der Blick nicht zu weit nach oben oder unten geht

        // Rotieren des Spielers (links/rechts)
        transform.Rotate(Vector3.up * rotationY);

        // Rotieren der Kamera (hoch/runter)
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
    }
}
