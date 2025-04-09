using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMovePlayer : MonoBehaviour
{

    public float moveSpeed = 3.0f; // Bewegungsgeschwindigkeit
    public float gravity = -9.81f; // Schwerkraft

    private CharacterController characterController;
    private Vector3 velocity;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }


    public void MovePlayer()
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

}
