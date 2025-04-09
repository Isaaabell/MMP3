using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMoveCar : MonoBehaviour
{
    public float acceleration = 15f;  // Beschleunigung
    public float maxSpeed = 20f;      // Maximale Geschwindigkeit
    public float brakingForce = 30f;  // Bremskraft
    public float reverseSpeed = 10f;  // Rückwärtsgeschwindigkeit
    public float turnSpeed = 50f;     // Lenkgeschwindigkeit

    private float currentSpeed = 0f;  // Aktuelle Geschwindigkeit

    void Update()
    {
       
    }

    public void MoveCar()
    {
        float moveInput = Input.GetAxis("Vertical");   // W = 1, S = -1
        float turnInput = Input.GetAxis("Horizontal"); // A = -1, D = 1

        // **Beschleunigen & Bremsen**
        if (moveInput > 0) // W gedrückt -> Auto beschleunigt
        {
            currentSpeed += acceleration * Time.deltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, -reverseSpeed, maxSpeed);
        }
        else if (moveInput < 0) // S gedrückt -> Bremsen oder Rückwärtsfahren
        {
            if (currentSpeed > 0) // Falls wir uns vorwärts bewegen -> Bremsen
            {
                currentSpeed -= brakingForce * Time.deltaTime;
            }
            else // Falls wir stehen oder rückwärts sind -> Rückwärtsfahren
            {
                currentSpeed -= acceleration * Time.deltaTime;
                currentSpeed = Mathf.Clamp(currentSpeed, -reverseSpeed, maxSpeed);
            }
        }
        else // Kein Input -> Auto langsam stoppen
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, brakingForce * Time.deltaTime);
        }

        // **Bewegen**
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

        // **Lenken (nur wenn das Auto sich bewegt)**
        if (Mathf.Abs(currentSpeed) > 0.1f)
        {
            float turnAmount = turnInput * turnSpeed * Time.deltaTime * (currentSpeed / maxSpeed);
            transform.Rotate(0, turnAmount, 0);
        }
    }
}
