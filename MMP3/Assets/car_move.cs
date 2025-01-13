using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class car_move : MonoBehaviour
{
    public float acceleration = 30f;  // Reduziere die Beschleunigung
    public float steeringAngle = 30f;
    public float maxSpeed = 200f;        // Setze eine realistische Maximalgeschwindigkeit
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void HandleMovement()
    {
        // Eingaben für Bewegung und Lenkung
        float verticalInput = Input.GetAxis("Vertical");   // W und S Tasten
        float horizontalInput = Input.GetAxis("Horizontal"); // A und D Tasten

        // Vorwärts/Rückwärtsbewegung
        Vector3 forwardForce = -transform.forward * verticalInput * acceleration;

        // Berechne die Geschwindigkeit des Autos und beschränke sie
        if (rb.velocity.magnitude < maxSpeed)
        {
            rb.AddForce(forwardForce, ForceMode.VelocityChange);
        }

        // Lenkung
        float steer = horizontalInput * steeringAngle * Time.deltaTime;
        transform.Rotate(0, steer, 0);

        // Debugging
        Debug.Log("vertical Input: " + Input.GetAxis("Vertical"));
        Debug.Log("Speed: " + rb.velocity.magnitude);
    }

    void Update()
    {
        HandleMovement();
    }
}
