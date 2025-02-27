using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
    // public float moveSpeed = 3.0f; // Bewegungsgeschwindigkeit
    public float lookSpeed = 2.0f; // Maus-Blickgeschwindigkeit
    // public float gravity = -9.81f; // Schwerkraft

    private CharacterController characterController;
    private Camera playerCamera;
    private Vector3 velocity;
    private float rotationX = 0;
    [SerializeField] private Canvas canvas;

    [SerializeField] public bool isCar;

    [Header("Script references")]
    [SerializeField] private TutorialMovePlayer tutorialMovePlayer;
    [SerializeField] private TutorialMoveCar tutorialMoveCar;

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

        if (isCar)
        {
            tutorialMoveCar.MoveCar();

        }
        else
        {
            tutorialMovePlayer.MovePlayer();
            LookAround();
        }

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
